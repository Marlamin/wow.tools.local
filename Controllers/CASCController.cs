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
            return CASC.BuildName;
        }
    }
}
