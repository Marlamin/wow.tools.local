using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/[controller]")]
    [ApiController]
    public class ZipController : ControllerBase
    {
        [Route("fdids")]
        [HttpGet]
        public ActionResult GetByFileDataID(string ids, string filename)
        {
            var filedataidlist = new List<uint>();
            foreach (var fdid in ids.Split(','))
            {
                filedataidlist.Add(uint.Parse(fdid));
            }

            var filedataids = filedataidlist.ToArray();


            Console.WriteLine("Serving zip file \"" + filename + "\" (" + filedataids.Length + " fdids starting with " + filedataids[0].ToString() + ")");

            var errors = new List<string>();

            using (var zip = new MemoryStream())
            {
                using (var archive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    foreach (var filedataid in filedataids)
                    {
                        if (zip.Length > 100000000)
                        {
                            errors.Add("Max of 100MB per archive reached, didn't include file " + filedataid);
                            Console.WriteLine("Max of 100MB per archive reached!");
                            continue;
                        }

                        try
                        {
                            using (var cascStream = CASC.GetFileByID(filedataid))
                            {
                                if(cascStream == null)
                                {
                                    errors.Add("Error opening file " + filedataid);
                                    Console.WriteLine("File " + filedataid + " not found in CASC");
                                    continue;
                                }

                                var entryname = Path.GetFileName(CASC.Listfile[(int)filedataid]);
                                if (entryname == "")
                                {
                                    entryname = filedataid.ToString() + ".unk";
                                }

                                var entry = archive.CreateEntry(entryname);
                                using (var entryStream = entry.Open())
                                {
                                    cascStream.CopyTo(entryStream);
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            errors.Add("File " + filedataid + " not found in root");
                            Console.WriteLine("File " + filedataid + " not found in root of buildconfig");
                        }
                        catch (Exception e)
                        {
                            errors.Add("Error " + e.Message + " occured when getting file " + filedataid);
                            Console.WriteLine("Error " + e.Message + " occured when getting file " + filedataid);
                        }
                    }

                    if (errors.Count > 0)
                    {
                        using (var errorStream = new MemoryStream())
                        {
                            var entry = archive.CreateEntry("errors.txt");
                            using (var entryStream = entry.Open())
                            {
                                foreach (var error in errors)
                                {
                                    entryStream.Write(Encoding.UTF8.GetBytes(error + "\n"));
                                }
                            }
                        }
                    }
                }

                return new FileContentResult(zip.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = filename
                };
            }
        }
    }
}