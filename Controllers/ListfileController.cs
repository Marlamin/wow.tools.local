using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("listfile/")]
    [ApiController]
    public class ListfileController : Controller
    {
        [Route("datatables")]
        [HttpGet]
        public DataTablesResult DataTables(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = CASC.M2Listfile.Count,
                data = new List<List<string>>()
            };

            var listfileResults = new Dictionary<int, string>();
            
            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                listfileResults = CASC.M2Listfile.Where(x => x.Value.ToLower().Contains(searchStr)).ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = listfileResults.Count();
            }
            else
            {
                listfileResults = CASC.M2Listfile.ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = CASC.M2Listfile.Count;
            }
            
            var rows = new List<string>();

            foreach (var listfileResult in listfileResults.Skip(start).Take(length))
            {
                result.data.Add(
                    new List<string>() { 
                        listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename 
                        "", // Lookup
                        "", // Versions
                        Path.GetExtension(listfileResult.Value).Replace(".", "") // Type
                    });
            }

            return result;
        }

        [Route("info")]
        [HttpGet]
        public string Info(int filename, string filedataid)
        {
            var split = filedataid.Split(",");
            if(split.Length > 1)
            {
                foreach(var id in split)
                {
                    if (CASC.Listfile.TryGetValue(int.Parse(id), out string name))
                    {
                        return name;
                    }
                }

                return "";
            }
            else
            {
                if (CASC.Listfile.TryGetValue(int.Parse(filedataid), out string name))
                {
                    return name;
                }
                else
                {
                    return "";
                }
            }
            
        }
    }

    public struct DataTablesResult
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<List<string>> data { get; set; }
    }
}
