using CASCLib;
using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace wow.tools.local.Services
{
    public class CASC
    {
        private static CASCHandler cascHandler;
        public static bool IsCASCInit = false;
        public static string BuildName;
        public static Dictionary<int, string> Listfile = new();
        public static Dictionary<string, int> ListfileReverse = new();
        public static SortedDictionary<int, string> M2Listfile = new();
        private static HttpClient WebClient = new HttpClient();
        public static void InitCasc(BackgroundWorkerEx worker = null, string? basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            CASCConfig.LoadFlags &= ~(LoadFlags.Download | LoadFlags.Install);
            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;

            if (basedir == null)
            {
                Console.WriteLine("Initializing CASC from web for program " + program);
                cascHandler = CASCHandler.OpenOnlineStorage(program, "eu", worker);
            }
            else
            {
                basedir = basedir.Replace("_retail_", "").Replace("_ptr_", "");
                Console.WriteLine("Initializing CASC from local disk with basedir " + basedir + " and program " + program);
                cascHandler = CASCHandler.OpenLocalStorage(basedir, program, worker);
            }

            var splitName = cascHandler.Config.BuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];
            
            cascHandler.Root.SetFlags(locale);
            
            IsCASCInit = true;
            Console.WriteLine("Finished loading " + BuildName);
        }

        public async static Task<bool> LoadListfile()
        {
            if (!File.Exists("listfile.csv"))
            {
                Console.WriteLine("Downloading listfile");

                using var s = await WebClient.GetStreamAsync(SettingsManager.listfileURL);
                using var fs = new FileStream("listfile.csv", FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }

            if (!File.Exists("listfile.csv"))
            {
                throw new FileNotFoundException("Could not find listfile.csv");
            }

            Console.WriteLine("Loading listfile");

            foreach (var line in File.ReadAllLines("listfile.csv"))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                var fdid = int.Parse(splitLine[0]);

                if (!cascHandler.FileExists(fdid))
                    continue;

                Listfile.Add(fdid, splitLine[1]);
                ListfileReverse.Add(splitLine[1], fdid);

                // Pre-filtered M2 list
                if (line.EndsWith(".m2"))
                    M2Listfile.Add(fdid, splitLine[1]);
            }

            return true;
        }

        public static Stream GetFileByID(uint filedataid)
        {
            try
            {
                return cascHandler.OpenFile((int)filedataid);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception retrieving FileDataID " + filedataid + ": " + e.Message);
                return new MemoryStream();
            }
        }

        public static Stream GetFileByName(string filename)
        {
            if (ListfileReverse.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return cascHandler.OpenFile(fileDataID);
            }
            
            throw new FileNotFoundException("Could not find " + filename + " in listfile");
        }

        public static bool FileExists(string filename)
        {
            if (ListfileReverse.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return cascHandler.FileExists(fileDataID);
            }

            return false;
        }
        
        public static bool FileExists(uint filedataid)
        {
            return cascHandler.FileExists((int)filedataid);
        }
    }
}
