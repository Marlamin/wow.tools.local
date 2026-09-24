using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using wow.tools.local.Managers;
using wow.tools.local.Services;
using WoWFormatLib.FileProviders;
using WoWFormatLib.FileReaders;
using WoWFormatLib.Structs.BLS;

namespace wow.tools.local.Controllers
{
    [Route("shader/")]
    [ApiController]
    public class ShaderController : ControllerBase
    {
        private static Dictionary<(uint fileDataID, string API), BLS> shaderCache = []; // 1 fdid can have multiple apis because gfat exists

        private BLS GetShader(uint fileDataID, string api = "", string build = "", string overrideCKey = "")
        {
            if(shaderCache.TryGetValue((fileDataID, api), out BLS cachedShader))
                return cachedShader;

            if (string.IsNullOrWhiteSpace(build))
                build = CASC.BuildName;

            if (!FileProvider.HasProvider(build))
            {
                if (build == CASC.BuildName)
                {
                    if (CASC.IsTACTSharpInit)
                    {
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(CASC.buildInstance);
                        FileProvider.SetProvider(tact, CASC.BuildName);
                    }
                }
                else
                {
                    try
                    {
                        var dbBuild = BuildManager.GetBuildByVersion(build);
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(dbBuild);
                        FileProvider.SetProvider(tact, build);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error initializing TACTSharp for build " + build + ": " + e.Message);
                        var wago = new WagoFileProvider();
                        wago.SetBuild(build);
                        FileProvider.SetProvider(wago, build);
                    }
                }
            }

            FileProvider.SetDefaultBuild(build);

            try
            {
                var blsReader = new BLSReader();

                if (!string.IsNullOrEmpty(overrideCKey))
                    blsReader.LoadBLS(Convert.FromHexString(overrideCKey));
                else
                    blsReader.LoadBLS(fileDataID);

                shaderCache[(fileDataID, api)] = blsReader.shaderFile;
                return shaderCache[(fileDataID, api)];
            }
            catch (Exception e)
            {
                if (e.Message == "Unsupported shader file: GFAT")
                {
                    var notBLSReader = new GFATReader();
                    var notBLS = notBLSReader.LoadGFAT(fileDataID);

                    if (!string.IsNullOrEmpty(overrideCKey))
                        notBLS = notBLSReader.LoadGFAT(Convert.FromHexString(overrideCKey));

                    shaderCache[(fileDataID, api)] = notBLS.shaderPerGFX[api];
                    return shaderCache[(fileDataID, api)];
                }
                else
                {
                    throw;
                }
            }
        }

        [Route("clearCache")]
        public string ClearCache()
        {
            shaderCache.Clear();
            return "Shader cache cleared";
        }

        [Route("dumpPermutationHex")]
        public string DumpPermutationHex(uint fileDataID, int permutation, string api = "", string build = "", string overrideCKey = "")
        {
            var shader = GetShader(fileDataID, api, build, overrideCKey);

            if (permutation > shader.nCompressedChunks)
                return "Permutation " + permutation + " out of range for shader (" + shader.nCompressedChunks + " permutations)";

            var permutationBytes = shader.decompressedShaders[permutation];

            using (var stream = new MemoryStream(permutationBytes))
            {
                var hex = new StringBuilder();
                var ascii = new StringBuilder();
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < stream.Length; i++)
                    {
                        // Cut off at 1MB
                        if (i > 1024 * 1024)
                        {
                            hex.AppendLine().Append("... (file too large to display fully)");
                            break;
                        }

                        if (i % 16 == 0)
                        {
                            if (i != 0)
                            {
                                hex.Append("  ").Append(ascii).AppendLine();
                                ascii.Clear();
                            }

                            hex.Append($"{i:X8}: ");
                        }

                        var rawByte = reader.ReadByte();
                        hex.Append($"{rawByte:X2} ");
                        ascii.Append(rawByte >= 32 && rawByte <= 126 ? (char)rawByte : '.');
                    }

                    // append leftovers
                    if (ascii.Length > 0)
                    {
                        int padding = 16 - (int)(stream.Length % 16);
                        if (padding < 16)
                        {
                            hex.Append(new string(' ', padding * 3));
                        }
                        hex.Append("  ").Append(ascii);
                    }
                }
                return hex.ToString();
            }
        }


        [Route("decompilePermutation")]
        public IActionResult DecompilePermutation(uint fileDataID, int permutation, string api = "", string build = "", string overrideCKey = "")
        {
            if (string.IsNullOrEmpty(api))
                api = "DX60";

            if (api != "DX60")
                return BadRequest("Unsupported API");

            var dxcLoc = "External/dxc/dxc.exe";
            if (!System.IO.File.Exists(dxcLoc))
                return BadRequest("DXC not found, please grab it from https://github.com/microsoft/DirectXShaderCompiler/releases and extract it to External/dxc in the WTL folder");

            var shader = GetShader(fileDataID, api, build, overrideCKey);

            if (permutation > shader.nCompressedChunks)
                return BadRequest("Permutation " + permutation + " out of range for shader (" + shader.nCompressedChunks + " permutations)");

            var permutationBytes = shader.decompressedShaders[permutation];

            Directory.CreateDirectory("temp");
            System.IO.File.WriteAllBytes("temp/permutation_" + fileDataID + "_" + permutation + ".bin", permutationBytes);

            // run dxc dumpbin and capture output
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = dxcLoc;
            process.StartInfo.Arguments = "dumpbin /dumpbin temp/permutation_" + fileDataID + "_" + permutation + ".bin";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            var outputBuilder = new System.Text.StringBuilder();
            process.OutputDataReceived += (sender, args) => outputBuilder.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => outputBuilder.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            var output = outputBuilder.ToString();

            return Ok(output);
        }
    }
}
