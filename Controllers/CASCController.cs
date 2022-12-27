using Microsoft.AspNetCore.Mvc;
using System.Text;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController : Controller
    {
        [Route("fdid")]
        [HttpGet]
        public ActionResult File(uint fileDataID, string filename = "")
        {
            if (!CASC.FileExists(fileDataID))
                return NotFound();

            if (string.IsNullOrEmpty(filename))
            {
                // TODO: Guess extension
                filename = fileDataID.ToString();
            }

            var file = CASC.GetFileByID(fileDataID);
            if (file == null)
                return NotFound();

            return new FileStreamResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        [Route("buildname")]
        [HttpGet]
        public string BuildName()
        {
            return CASC.BuildName;
        }

        [Route("builds")]
        [HttpPost]
        public DataTablesResult Builds()
        {
            var result = new DataTablesResult();

            if (Request.Method == "POST" && Request.Form.TryGetValue("draw", out var draw))
            {
                result.draw = int.Parse(draw);
                result.data = new List<List<string>>();
            }

            if (SettingsManager.wowFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.wowFolder, ".build.info")))
            {
                foreach (var line in System.IO.File.ReadAllLines(Path.Combine(SettingsManager.wowFolder, ".build.info")))
                {
                    var splitLine = line.Split("|");
                    if (splitLine[0] == "Branch!STRING:0")
                        continue;

                    var buildConfig = splitLine[2];
                    var cdnConfig = splitLine[3];
                    var version = splitLine[12];
                    var product = splitLine[13];

                    var splitVersion = version.Split(".");
                    var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                    var build = splitVersion[3];

                    var isActive = CASC.BuildName == version;

                    result.data.Add(new List<string>() { patch, build, product, buildConfig, cdnConfig, isActive.ToString() });
                }

                result.data.OrderBy(x => x[0]).ToList();
                result.recordsTotal = result.data.Count;
                result.recordsFiltered = result.data.Count;
            }

            return result;
        }

        [Route("switchProduct")]
        [HttpGet]
        public bool SwitchProduct(string product)
        {
            CASC.InitCasc(SettingsManager.wowFolder, product);
            return true;
        }

        [Route("analyzeUnknown")]
        [HttpGet]
        public bool AnalyzeUnknown()
        {
            var unknownFiles = CASC.Listfile.Where(x => x.Value == "").OrderByDescending(x => x.Key);
            Console.WriteLine("Analyzing " + unknownFiles.Count() + " unknown files");
            var numFilesTotal = unknownFiles.Count();
            var numFilesDone = 0;
            Parallel.ForEach(unknownFiles, unknownFile =>
            {
                try
                {
                    var file = CASC.GetFileByID((uint)unknownFile.Key);
                    if (file == null)
                    {
                        return;
                    }

                    using (var bin = new BinaryReader(file))
                    {
                        if (bin.BaseStream.Length < 4)
                            return;

                        var magic = bin.ReadBytes(4);
                        var type = "unk";
                        var magicString = Encoding.ASCII.GetString(magic);
                        switch (magicString)
                        {
                            case "MD21":
                            case "MD20":
                                type = "m2";
                                break;
                            case "SKIN":
                                type = "skin";
                                break;
                            case "OggS":
                                type = "ogg";
                                break;
                            case "BLP2":
                                type = "blp";
                                break;
                            case "REVM":
                                type = "wmoadt";
                                break;
                            case "AFM2":
                            case "AFSA":
                            case "AFSB":
                                type = "anim";
                                break;
                            case "WDC3":
                                type = "db2";
                                break;
                            case "RIFF":
                                type = "avi";
                                break;
                            default:
                                Console.WriteLine((uint)unknownFile.Key + " - Unknown magic " + magicString + " (" + Convert.ToHexString(magic) + ")");
                                break;
                        }

                        if (type != "unk")
                        {
                            //Console.WriteLine("Detected " + unknownFile.Key + " as " + type);
                            CASC.Listfile[unknownFile.Key] = "unknown/" + unknownFile.Key + "." + type;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("nknown keyname"))
                    {
                        Console.WriteLine("Failed to guess type for file " + unknownFile.Key + ": " + e.Message + "\n" + e.StackTrace);
                    }

                }

                if (numFilesDone % 1000 == 0)
                    Console.WriteLine("Analyzed " + numFilesDone + "/" + numFilesTotal + " files");
                
                numFilesDone++;
            });
            return true;
        }
    }
}
