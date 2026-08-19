using Microsoft.AspNetCore.Mvc;
using TACTSharp;
using TACTSharp.Interfaces;
using wow.tools.local.Managers;
using wow.tools.local.Services;
using static wow.tools.local.Services.SQLiteDB;

namespace wow.tools.local.Controllers
{
    [Route("build/")]
    [ApiController]
    public class BuildController : Controller
    {
        private static IVersionService? versionService;
        private static VersionService currentVersionService;

        public BuildController()
        {
            if (versionService == null)
            {
                if (SettingsManager.UseTACTChannels)
                {
                    versionService = new TACTSharp.VersionServices.TACTChannels();
                    currentVersionService = VersionService.TACTChannels;
                }
                else
                {
                    versionService = new TACTSharp.VersionServices.Ribbit();
                    currentVersionService = VersionService.Ribbit;
                }
            }
        }

        [Route("list")]
        [HttpGet]
        public List<BuildMetaData> List()
        {
            return SQLiteDB.GetBuilds();
        }

        [Route("clearVersionServiceCache")]
        [HttpGet]
        public bool ClearVersionServiceCache()
        {
            // Also reload local builds
            CASC.LoadBuildInfo();

            if (currentVersionService == VersionService.TACTChannels && !SettingsManager.UseTACTChannels)
            {
                versionService = new TACTSharp.VersionServices.Ribbit();
                currentVersionService = VersionService.Ribbit;
            }
            else if (currentVersionService == VersionService.Ribbit && SettingsManager.UseTACTChannels)
            {
                versionService = new TACTSharp.VersionServices.TACTChannels();
                currentVersionService = VersionService.TACTChannels;
            }

            versionService!.Refresh();
            return true;
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
            var showEncrypted = false;

            var start = 0;
            var length = 20;
            var orderCol = 1; // Build
            var orderDir = "desc";
            var search = "";

            if (Request.Method == "POST")
            {
                if (Request.Form.TryGetValue("draw", out var drawValue) && int.TryParse(drawValue, out var draw))
                    result.draw = draw;

                _ = Request.Form.TryGetValue("start", out var startValue) && int.TryParse(startValue, out start);
                _ = Request.Form.TryGetValue("length", out var lengthValue) && int.TryParse(lengthValue, out length);

                _ = Request.Form.TryGetValue("showLocal", out var showLocalString) && bool.TryParse(showLocalString, out showLocal);
                _ = Request.Form.TryGetValue("showOnline", out var showOnlineString) && bool.TryParse(showOnlineString, out showOnline);
                _ = Request.Form.TryGetValue("showArchived", out var showArchivedString) && bool.TryParse(showArchivedString, out showArchived);
                _ = Request.Form.TryGetValue("showEncrypted", out var showEncryptedString) && bool.TryParse(showEncryptedString, out showEncrypted);

                _ = Request.Form.TryGetValue("order[0][column]", out var orderColString) && int.TryParse(orderColString, out orderCol);
                _ = Request.Form.TryGetValue("order[0][dir]", out var orderDirString) && (orderDirString == "asc" || orderDirString == "desc") ? orderDir = orderDirString : orderDir = "desc";

                _ = Request.Form.TryGetValue("search[value]", out var searchString) && !string.IsNullOrWhiteSpace(searchString) ? search = searchString : search = "";
                result.data = [];
            }

            if (showLocal && SettingsManager.WoWFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.WoWFolder, ".build.info")))
            {
                CASC.LoadBuildInfo();
                foreach (var availableBuild in CASC.AvailableBuilds)
                    availableBuilds.Add((availableBuild.Product, availableBuild.Version, availableBuild.BuildConfig, availableBuild.CDNConfig, false));
            }

            if (showOnline)
            {
                var products = versionService!.GetProductVariants();

                List<(string buildConfig, string cdnConfig)> availableRemoteBuilds = new();

                foreach (var product in products)
                {
                    if (!product.StartsWith("wow"))
                        continue;

                    if (!showEncrypted)
                        if (product.StartsWith("wowdev") || product.StartsWith("wownev") || product.StartsWith("wowv") || product == "wowlivetest2")
                            continue;

                    var builds = versionService.GetVersions(product);

                    foreach (var build in builds)
                        availableBuilds.Add((product, build.Value.VersionString, build.Value.BuildConfig, build.Value.CDNConfig, true));
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

            result.data = result.data.Skip(start).Take(length).ToList();

            return result;
        }

        private static uint NumericalPatch(string patch)
        {
            var firstBuild = patch.Split('.');
            return (uint)((int.Parse(firstBuild[0]) * 10000) + (int.Parse(firstBuild[1]) * 100) + int.Parse(firstBuild[2]));
        }
    }
}