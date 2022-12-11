using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController : Controller
    {
        [Route("fdid")]
        [HttpGet]
        public ActionResult File(uint fileDataID)
        {
            if (!CASC.FileExists(fileDataID))
                return NotFound();

            return new FileStreamResult(CASC.GetFileByID(fileDataID), "application/octet-stream")
            {
                FileDownloadName = fileDataID.ToString()
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

            if(SettingsManager.wowFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.wowFolder, ".build.info")))
            {
                foreach(var line in System.IO.File.ReadAllLines(Path.Combine(SettingsManager.wowFolder, ".build.info")))
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
    }
}
