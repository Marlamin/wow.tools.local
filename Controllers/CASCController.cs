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
            var splitName= CASC.BuildName.Replace("WOW-", "").Split("patch");
            var buildName = splitName[1].Split("_")[0] + "." + splitName[0];
            return buildName;
        }
    }
}
