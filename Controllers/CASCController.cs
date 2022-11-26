using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController : Controller
    {
        [Route("fdid")]
        [HttpGet]
        public FileStreamResult File(uint fileDataID)
        {
            return new FileStreamResult(CASC.GetFileByID(fileDataID), "application/octet-stream")
            {
                FileDownloadName = fileDataID.ToString()
            };
        }

        [Route("buildname")]
        [HttpGet]
        public string BuildName()
        {
            // WOW-46801patch10.0.2_PTR
            var splitName= CASC.BuildName.Replace("WOW-", "").Split("patch");
            var buildName = splitName[1].Split("_")[0] + "." + splitName[0];
            Console.WriteLine("Set modelviewer build name as " + buildName);
            return buildName;
        }
    }
}
