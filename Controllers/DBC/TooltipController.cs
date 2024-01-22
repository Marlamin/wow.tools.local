using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/tooltip")]
    [ApiController]
    public class TooltipController() : ControllerBase
    {
        [Route("file/{fileDataID}")]
        [HttpGet]
        public Dictionary<string, string> File(int fileDataID)
        {
            var returnValues = new Dictionary<string, string>
            {
                { "fileDataID", fileDataID.ToString() },
                { "filename", CASC.Listfile.TryGetValue(fileDataID, out var filename) ? filename : "Unknown" },
                { "type", CASC.Types.TryGetValue(fileDataID, out var type) ? type : "Unknown" }
            };
            return returnValues;
        }

        [Route("wex/{expression}")]
        [HttpGet]
        public Dictionary<string, string> WorldStateExpression(string expression)
        {
            var worldState = new WSExpressionParser(expression);
            var humanReadable = new HumanReadableWorldStateExpression();
            var result = humanReadable.StateToString([.. worldState.state.Values]);
            var returnValues = new Dictionary<string, string>
            {
                { "expression", expression },
                { "result", result }
            };
            return returnValues;
        }
    }
}