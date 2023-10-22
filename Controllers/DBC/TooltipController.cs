using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/tooltip")]
    [ApiController]
    public class TooltipController : ControllerBase
    {
        private readonly DBDProvider dbdProvider;

        public TooltipController(IDBDProvider dbdProvider, IDBCManager dbcManager)
        {
            this.dbdProvider = dbdProvider as DBDProvider;
        }

        [Route("file/{fileDataID}")]
        [HttpGet]
        public Dictionary<string, string> File(int fileDataID)
        {
            var returnValues = new Dictionary<string, string>();
            returnValues.Add("fileDataID", fileDataID.ToString());
            returnValues.Add("filename", CASC.Listfile.TryGetValue(fileDataID, out string filename) ? filename : "Unknown");
            returnValues.Add("type", CASC.Types.TryGetValue(fileDataID, out string type) ? type : "Unknown");
            return returnValues;
        }

        [Route("wex/{expression}")]
        [HttpGet]
        public Dictionary<string, string> WorldStateExpression(string expression)
        {
            var worldState = new WSExpressionParser(expression);
            var humanReadable = new HumanReadableWorldStateExpression();
            var result = humanReadable.StateToString(worldState.state.Values.ToList());
            var returnValues = new Dictionary<string, string>();
            returnValues.Add("expression", expression);
            returnValues.Add("result", result);
            return returnValues;
        }
    }
}