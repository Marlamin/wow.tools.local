using DBDiffer;
using DBDiffer.DiffResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using wow.tools.local.Services;

namespace wow.tools.Local.Controllers
{
    [Route("dbc/")]
    [ApiController]
    public class DiffController(IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private static Lock diffLock = new();
        private static Dictionary<(string table, string build1, string build2, bool useHotfixesFor1, bool useHotfixesFor2), WoWToolsDiffResult> diffCache = new();

        [Route("clearCache")]
        [HttpGet]
        public async Task ClearCache()
        {
            diffCache.Clear();
        }

        [Route("diff")]
        [HttpPost]
        public async Task<string> Diff(string name, string build1, string build2, bool useHotfixesFor1 = false, bool useHotfixesFor2 = false)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(build1) || string.IsNullOrEmpty(build2))
            {
                return "Invalid arguments! Require name, build1, build2";
            }

            var parameters = new Dictionary<string, string>();

            var draw = 0;
            var start = 0;
            var length = 25;

            if (Request.Method == "POST")
            {
                // POST, what site uses
                foreach (var post in Request.Form)
                    parameters.Add(post.Key, post.Value!);

                if (parameters.TryGetValue("draw", out string? drawString))
                    draw = int.Parse(drawString);

                if (parameters.TryGetValue("start", out string? startString))
                    start = int.Parse(startString);

                if (parameters.TryGetValue("length", out string? lengthString))
                    length = int.Parse(lengthString);
            }
            else
            {
                // GET, backwards compatibility for scripts/users using this
                foreach (var get in Request.Query)
                    parameters.Add(get.Key, get.Value!);
            }

            Console.WriteLine("Serving diff for " + name + " between " + build1 + " and " + build2);

            var cacheKey = (name, build1, build2, useHotfixesFor1, useHotfixesFor2);
            lock (diffLock)
            {
                if (diffCache.TryGetValue(cacheKey, out var cachedDiff))
                {
                    Console.WriteLine("Returning cached diff for " + name + " between " + build1 + " and " + build2 + " (start: " + start + ", length: " + length + ")");
                    return cachedDiff.ToJSONString(draw, start, length);
                }
            }

            var dbc1 = (IDictionary)await dbcManager.GetOrLoad(name, build1, useHotfixesFor1);
            var dbc2 = (IDictionary)await dbcManager.GetOrLoad(name, build2, useHotfixesFor2);

            var comparer = new DBComparer(dbc1, dbc2);
            WoWToolsDiffResult diff = (WoWToolsDiffResult)comparer.Diff(DiffType.WoWTools);

            lock (diffLock)
            {
                if (diffCache.TryGetValue(cacheKey, out WoWToolsDiffResult? value))
                {
                    Console.WriteLine("Returning cached diff for " + name + " between " + build1 + " and " + build2 + " (start: " + start + ", length: " + length + ")");
                    return value.ToJSONString(draw, start, length);
                }

                Console.WriteLine("Caching diff for " + name + " between " + build1 + " and " + build2);
                diffCache[cacheKey] = diff;
            }

            return diff.ToJSONString(draw, start, length);
        }
    }
}