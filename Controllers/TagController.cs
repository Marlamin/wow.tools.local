using Microsoft.AspNetCore.Mvc;

namespace wow.tools.local.Controllers
{
    [Route("tags/")]
    [ApiController]
    public class TagController() : Controller
    {
        [Route("list")]
        [HttpGet]
        public IActionResult GetTagList(string tagKey = "")
        {
            var tags = Services.TagService.GetTags();

            if (!string.IsNullOrEmpty(tagKey))
                tags = tags.Where(t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(tags);
        }

        [Route("table")]
        [HttpGet]
        public IActionResult GetTable(int draw, int start, int length)
        {
            var result = new DataTablesResult();

            var allTags = Services.TagService.GetTags();

            result.draw = draw;
            result.recordsTotal = allTags.Count;
            result.recordsFiltered = allTags.Count;
            result.data = new List<List<string>>();

            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                allTags = allTags.Where(tag =>
                    tag.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    tag.Key.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    tag.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var pagedTags = allTags.Skip(start).Take(length);
            foreach (var tag in pagedTags)
            {
                var row = new List<string>();
                row.Add(tag.Key);
                row.Add(tag.Name);
                row.Add(tag.Category);
                result.data.Add(row);
            }

            return Ok(result);
        }

        [Route("tagsByFDID")]
        [HttpGet]
        public IActionResult GetTagsByFileDataID(int fdid)
        {
            var tags = Services.TagService.GetTagsByFileDataID(fdid);
            return Ok(tags);
        }

        [Route("updateTag")]
        [HttpPost]
        public IActionResult UpdateTag([FromForm]string name, [FromForm]string key, [FromForm]string description, [FromForm]string type, [FromForm]string category, [FromForm]bool allowMultiple)
        {
            Services.TagService.AddOrUpdateTag(name, key, description, type, category, allowMultiple);

            if(type == "Preset")
            {
                var optionIndex = 0;
                var hasOptions = Request.Form.ContainsKey("presetOption[0]");
                while (hasOptions)
                {
                    var presetOption = Request.Form["presetOption[" + optionIndex + "]"]!;

                    var presetDescription = "";
                    if(Request.Form.TryGetValue("presetDescription[" + optionIndex + "]", out var presetDescriptionRaw))
                        presetDescription = presetDescriptionRaw.First();

                    var presetAliases = "";
                    if(Request.Form.TryGetValue("presetAliases[" + optionIndex + "]", out var presetAliasesRaw))
                        presetAliases = presetAliasesRaw.First();

                    Services.TagService.AddOrUpdateTagOption(key, presetOption, presetDescription, presetAliases);
                    optionIndex++;

                    hasOptions = Request.Form.ContainsKey("presetOption[" + optionIndex + "]");
                }
            }
            return Ok(new { status = "success" });
        }

        [Route("deleteTag")]
        [HttpPost]
        public IActionResult DeleteTag(string name)
        {
            Services.TagService.DeleteTag(name);
            return Ok(new { status = "success" });
        }

        [Route("addTagToFDID")]
        [HttpPost]
        public IActionResult AddTagToFDID(int fileDataID, string tagName, string tagSource, string tagValue)
        {
            Services.TagService.AddTagToFDID(fileDataID, tagName, tagSource, tagValue);
            return Ok(new { status = "success" });
        }

        [Route("save")]
        [HttpGet]
        public IActionResult SaveTagRepo()
        {
            Services.TagService.SaveRepo();
            return Ok(new { status = "success" });
        }

        [Route("reload")]
        [HttpGet]
        public IActionResult ReloadTagRepo()
        {
            Services.TagService.ReloadRepo();
            return Ok(new { status = "success" });
        }

        [Route("changeStatus")]
        [HttpGet]
        public IActionResult ChangeStatus()
        {
            return Ok(new { unsavedChanges = Services.TagService.HasUnsavedChanges() });
        }

        public struct DataTablesResult
        {
            public int draw { get; set; }
            public int recordsFiltered { get; set; }
            public int recordsTotal { get; set; }
            public List<List<string>> data { get; set; }
        }
    }
}
