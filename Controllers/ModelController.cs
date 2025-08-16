using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using wow.tools.local.Services;
using WoWFormatLib.FileProviders;
using WoWFormatLib.FileReaders;

namespace wow.tools.local.Controllers
{
    [Route("model/")]
    public class ModelController() : Controller
    {
        [Route("info")]
        [HttpGet]
        public string Info(int fileDataID)
        {
            if (!FileProvider.HasProvider(CASC.BuildName))
            {
                if (CASC.IsCASCLibInit)
                {
                    var casc = new CASCFileProvider();
                    casc.InitCasc(CASC.cascHandler);
                    FileProvider.SetProvider(casc, CASC.BuildName);
                }
                else if (CASC.IsTACTSharpInit)
                {
                    var tact = new TACTSharpFileProvider();
                    tact.InitTACT(CASC.buildInstance);
                    FileProvider.SetProvider(tact, CASC.BuildName);
                }
            }

            FileProvider.SetDefaultBuild(CASC.BuildName);

            var returnString = "";

            if (!Listfile.Types.TryGetValue(fileDataID, out var type))
                type = "unk";

            if (type == "wmo")
            {
                var wmoReader = new WMOReader();
                var wmo = wmoReader.LoadWMO((uint)fileDataID);

                returnString += "<h3>Groups</h3>";
                returnString += "<table class='table table-striped'>";
                for (var i = 0; i < wmo.groupInfo.Length; i++)
                {
                    var group = wmo.groupInfo[i];
                    returnString += "<tr>";
                    returnString += "<td><b>Group</b></td>";
                    returnString += "<td>Flags: " + group.flags + "</td>";

                    if(wmo.groupInfo2 != null)
                    {
                        returnString += "<td>Flags2: " + wmo.groupInfo2[i].flags2 + "</td>";
                        returnString += "<td>'lodIndex' " + wmo.groupInfo2[i].lodIndex + "</td>";
                    }

                    returnString += "</tr>";
                }

                returnString += "</table>";

                returnString += "<h3>Group names</h3>";
                returnString += "<table class='table table-striped'>";
                for (var gn = 0; gn < wmo.groupNames.Length; gn++)
                {
                    returnString += "<tr><td>" + wmo.groupNames[gn].name + "</td></tr>";
                }
                returnString += "</table>";

                returnString += "<h3>Group FileDataIDs</h3>";
                returnString += "<table class='table table-striped table-sm'>";
                foreach (var gfdid in wmo.groupFileDataIDs)
                {
                    returnString += "<tr>";
                    var groupFilename = Listfile.NameMap.TryGetValue((int)gfdid, out var gfilename) ? gfilename.ToString() : "";
                    returnString += "<td>" + gfdid + "</td><td>" + groupFilename + "</td>";
                    returnString += "</tr>";
                }
                returnString += "</table>";

                returnString += "<h3>Materials</h3>";
                returnString += "<table class='table table-striped'>";
                for (var i = 0; i < wmo.materials.Length; i++)
                {
                    var material = wmo.materials[i];

                    returnString += "<tr>";
                    returnString += "<td><b>Material " + i + "</b></td>";
                    returnString += "<td>Flags: " + material.flags + "</td>";
                    returnString += "<td>BlendMode: " + material.blendMode + "</td>";
                    returnString += "<td>GroundType: " + material.groundType + "</td>";
                    returnString += "<td>Shader: " + GetEnumMemberAttrValue(material.shader) + "</td></tr>";
                    returnString += "<tr><td colspan=5>";
                    returnString += "<table class='table table-striped'>";
                    returnString += "<tr><td>Texture1</td><td>" + material.texture1 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.texture1, out var texture1Filename) ? texture1Filename : "Unknown filename") + "</td></tr>";
                    returnString += "<tr><td>Color (<attr title='Self Illuminated Day Night'>SIDN</attr>)</td><td>" + material.color1 + "</td><td>" + ARGBToDiv(material.color1) + "</td></tr>";
                    returnString += "<tr><td>Frame color (<attr title='Self Illuminated Day Night'>SIDN</attr>)</td><td>" + material.color1b + "</td><td>" + ARGBToDiv(material.color1b) + "</td></tr>";
                    returnString += "<tr><td>Texture 2</td><td>" + material.texture2 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.texture2, out var texture2Filename) ? texture2Filename : "Unknown filename") + "</td></tr>";
                    returnString += "<tr><td>Diff color</td><td>" + material.color2 + "</td><td>" + ARGBToDiv(material.color2) + "</td></tr>";
                    returnString += "<tr><td>Texture 3</td><td>" + material.texture3 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.texture3, out var texture3Filename) ? texture3Filename : "Unknown filename") + "</td></tr>";

                    if ((uint)material.shader == 23)
                    {
                        returnString += "<tr><td>Texture 4</td><td>" + material.color3 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.color3, out var t4n) ? t4n : "Unknown filename") + "</td></tr>";
                        returnString += "<tr><td>Texture 5</td><td>" + material.flags3 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.flags3, out var t5n) ? t5n : "Unknown filename") + "</td></tr>";
                        returnString += "<tr><td>Texture 6</td><td>" + material.runtimeData0 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.runtimeData0, out var t6n) ? t6n : "Unknown filename") + "</td></tr>";
                        returnString += "<tr><td>Texture 7</td><td>" + material.runtimeData1 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.runtimeData1, out var t7n) ? t7n : "Unknown filename") + "</td></tr>";
                        returnString += "<tr><td>Texture 8</td><td>" + material.runtimeData2 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.runtimeData2, out var t8n) ? t8n : "Unknown filename") + "</td></tr>";
                        returnString += "<tr><td>Texture 9</td><td>" + material.runtimeData3 + "</td><td>" + (Listfile.NameMap.TryGetValue((int)material.runtimeData3, out var t9n) ? t9n : "Unknown filename") + "</td></tr>";
                    }
                    else
                    {
                        returnString += "<tr><td>Unk color</td><td>" + material.color3 + "</td><td>" + ARGBToDiv(material.color3) + "</td></tr>";
                        returnString += "<tr><td>Flags 2</td><td>" + material.flags3 + "</td></tr>";
                        returnString += "<tr><td>Runtime data 0</td><td>" + material.runtimeData0 + "</td><td></td></tr>";
                        returnString += "<tr><td>Runtime data 1</td><td>" + material.runtimeData1 + "</td><td></td></tr>";
                        returnString += "<tr><td>Runtime data 2</td><td>" + material.runtimeData2 + "</td><td></td></tr>";
                        returnString += "<tr><td>Runtime data 3</td><td>" + material.runtimeData3 + "</td><td></td></tr>";
                    }

                    returnString += "</table></td>";
                    returnString += "</tr>";
                }
                returnString += "</table>";
            }
            return returnString;
        }

        private static string GetEnumMemberAttrValue<T>(T enumVal)
        {
            var enumType = typeof(T);
            var memInfo = enumType.GetMember(enumVal!.ToString()!);
            var attr = memInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null && attr.Value != null)
            {
                return attr.Value;
            }

            return string.Empty;
        }

        private static string ARGBToDiv(uint argb)
        {
            var a = (byte)(argb >> 24);
            var r = (byte)(argb >> 16);
            var g = (byte)(argb >> 8);
            var b = (byte)(argb >> 0);

            return "<div style='width: 50px; background-color: rgba(" + r + "," + g + "," + b + "," + a + ")'>&nbsp;</div>";
        }
    }
}
