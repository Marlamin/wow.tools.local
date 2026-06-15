using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Managers;
using wow.tools.local.Services;
using static wow.tools.local.Services.SQLiteDB;

namespace wow.tools.local.Controllers
{
    [Route("build/")]
    [ApiController]
    public class BuildController : Controller
    {
        private static readonly Dictionary<string, string> RibbitCache = new();

        [Route("list")]
        [HttpGet]
        public List<BuildMetaData> List()
        {
            return SQLiteDB.GetBuilds();
        }

        [Route("table")]
        [HttpPost]
        public DataTablesResult Builds(string mode = "local")
        {
            var result = new DataTablesResult();

            var availableBuilds = new List<(string product, string version, string buildConfig, string cdnConfig, bool isRemote)>();

            var showLocal = true;
            var showOnline = false;
            var showArchived = false;

            var orderCol = 1; // Build
            var orderDir = "desc";
            var search = "";

            if (Request.Method == "POST")
            {
                if (Request.Form.TryGetValue("draw", out var drawValue) && int.TryParse(drawValue, out var draw))
                    result.draw = draw;

                _ = Request.Form.TryGetValue("showLocal", out var showLocalString) && bool.TryParse(showLocalString, out showLocal);
                _ = Request.Form.TryGetValue("showOnline", out var showOnlineString) && bool.TryParse(showOnlineString, out showOnline);
                _ = Request.Form.TryGetValue("showArchived", out var showArchivedString) && bool.TryParse(showArchivedString, out showArchived);

                _ = Request.Form.TryGetValue("order[0][column]", out var orderColString) && int.TryParse(orderColString, out orderCol);
                _ = Request.Form.TryGetValue("order[0][dir]", out var orderDirString) && (orderDirString == "asc" || orderDirString == "desc") ? orderDir = orderDirString : orderDir = "desc";

                _ = Request.Form.TryGetValue("search[value]", out var searchString) && !string.IsNullOrWhiteSpace(searchString) ? search = searchString : search = "";
                result.data = [];
            }

            if (showLocal && SettingsManager.WoWFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.WoWFolder, ".build.info")))
                foreach (var availableBuild in CASC.AvailableBuilds)
                    availableBuilds.Add((availableBuild.Product, availableBuild.Version, availableBuild.BuildConfig, availableBuild.CDNConfig, false));

            if (showOnline)
            {
                var httpClient = new HttpClient();
                RibbitCache["v2/summary"] = httpClient.GetStringAsync($"https://{SettingsManager.Region}.version.battle.net/v2/summary").Result;

                List<(string buildConfig, string cdnConfig)> availableRemoteBuilds = new();

                foreach (var summaryLine in RibbitCache["v2/summary"].Split("\n"))
                {
                    if (summaryLine.StartsWith('#') || summaryLine.StartsWith("Product") || string.IsNullOrWhiteSpace(summaryLine))
                        continue;

                    var product = summaryLine.Split('|');

                    // Skip products with no versions
                    if (product[2] != "" || !product[0].StartsWith("wow"))
                        continue;

                    var endPoint = "v2/products/" + product[0] + "/versions";
                    if (!RibbitCache.TryGetValue(endPoint, out var cachedResult))
                    {
                        cachedResult = httpClient.GetStringAsync($"https://{SettingsManager.Region}.version.battle.net/" + endPoint).Result;
                        RibbitCache[endPoint] = cachedResult;
                    }

                    foreach (var line in cachedResult.Split("\n"))
                    {
                        var splitLine = line.Split('|');

                        if (splitLine[0] != "us")
                            continue;

                        availableBuilds.Add((product[0], splitLine[5], splitLine[1], splitLine[2], true));
                    }
                }
            }

            if (showArchived)
            {
                var archivedBuilds = SQLiteDB.GetBuilds();

                foreach (var archivedBuild in archivedBuilds)
                    availableBuilds.Add((archivedBuild.product, archivedBuild.version, archivedBuild.buildConfig, archivedBuild.cdnConfig, true));
            }

            // Force show manual build
            if (CASC.IsOnline && CASC.IsTACTSharpInit && !availableBuilds.Any(x => x.buildConfig == CASC.buildInstance!.Settings.BuildConfig))
                availableBuilds.Add((CASC.CurrentProduct, CASC.BuildName, CASC.buildInstance!.Settings.BuildConfig!, CASC.buildInstance!.Settings.CDNConfig!, true));

            // Unique by product, buildConfig (prefering local over remote to not show both)
            availableBuilds = availableBuilds
                .GroupBy(x => (x.product, x.buildConfig))
                .Select(g => g.Any(x => !x.isRemote) ? g.First(x => !x.isRemote) : g.First())
                .ToList();

            foreach (var availableBuild in availableBuilds)
            {
                var splitVersion = availableBuild.version.Split(".");
                var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                var build = splitVersion[3];

                var isActive = CASC.CurrentProduct == availableBuild.product;

                if (isActive && CASC.IsTACTSharpInit)
                    isActive = availableBuild.buildConfig == CASC.buildInstance!.Settings.BuildConfig;

                var hasManifest = ManifestManager.ExistsForBuild(patch, build);
                var hasDBCs = Directory.Exists(Path.Combine(SettingsManager.DBCFolder, patch + "." + build, "dbfilesclient"));

                result.data.Add([patch, build, availableBuild.product, availableBuild.buildConfig, availableBuild.cdnConfig, isActive.ToString(), hasManifest.ToString(), hasDBCs.ToString(), availableBuild.isRemote.ToString()]);
            }

            // special sorting for patch
            if (orderCol == 0)
            {
                if (orderDir == "asc")
                    result.data = result.data.OrderBy(x => NumericalPatch(x[orderCol])).ToList();
                else
                    result.data = result.data.OrderByDescending(x => NumericalPatch(x[orderCol])).ToList();
            }
            else
            {
                if (orderDir == "asc")
                    result.data = result.data.OrderBy(x => x[orderCol]).ToList();
                else
                    result.data = result.data.OrderByDescending(x => x[orderCol]).ToList();
            }

            result.recordsTotal = result.data.Count;
            result.recordsFiltered = result.data.Count;

            if (!string.IsNullOrEmpty(search))
            {
                result.data = result.data.Where(x => x.Any(field => field.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
                result.recordsFiltered = result.data.Count;
            }

            return result;
        }

        private static uint NumericalPatch(string patch)
        {
            var firstBuild = patch.Split('.');
            return (uint)((int.Parse(firstBuild[0]) * 10000) + (int.Parse(firstBuild[1]) * 100) + int.Parse(firstBuild[2]));
        }
    }
}