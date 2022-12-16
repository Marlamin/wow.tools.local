using CASCLib;

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
        public static List<int> AvailableFDIDs = new();

        private static HttpClient WebClient = new HttpClient();
        public static async void InitCasc(string? basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");
            
            CASCConfig.LoadFlags &= ~(LoadFlags.Download | LoadFlags.Install);
            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;

            if (basedir == null)
            {
                Console.WriteLine("Initializing CASC from web for program " + program);
                cascHandler = CASCHandler.OpenOnlineStorage(program, SettingsManager.region);
            }
            else
            {
                basedir = basedir.Replace("_retail_", "").Replace("_ptr_", "");
                Console.WriteLine("Initializing CASC from local disk with basedir " + basedir + " and program " + program);
                cascHandler = CASCHandler.OpenLocalStorage(basedir, program);
            }

            var splitName = cascHandler.Config.BuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];

            cascHandler.Root.SetFlags(locale);

            if (cascHandler.Root is WowTVFSRootHandler wtrh)
            {
                AvailableFDIDs = wtrh.RootEntries.Keys.ToList();
            }
            else if (cascHandler.Root is WowRootHandler wrh)
            {
                AvailableFDIDs = wrh.RootEntries.Keys.ToList();
            }

            Listfile = new Dictionary<int, string>();
            ListfileReverse = new Dictionary<string, int>();
            M2Listfile = new SortedDictionary<int, string>();

            AvailableFDIDs.ForEach(x => Listfile.Add(x, ""));

            var listfileRes = await LoadListfile();
            if (!listfileRes)
                throw new Exception("Failed to load listfile");

            IsCASCInit = true;
            Console.WriteLine("Finished loading " + BuildName);
        }

        public async static Task<bool> LoadListfile()
        {
            var download = false;

            if (File.Exists("listfile.csv"))
            {
                var info = new FileInfo("listfile.csv");
                if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                {
                    Console.WriteLine("Listfile outdated, redownloading..");
                    download = true;
                }
            }
            else
            {
                download = true;
            }

            if (download)
            {
                Console.WriteLine("Downloading listfile");

                using var s = await WebClient.GetStreamAsync(SettingsManager.listfileURL);
                using var fs = new FileStream("listfile.csv", FileMode.OpenOrCreate);
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

                if (Listfile.ContainsKey(fdid))
                {
                    Listfile[fdid] = splitLine[1];
                    ListfileReverse.Add(splitLine[1], fdid);

                    if (line.EndsWith(".m2"))
                        M2Listfile.Add(fdid, splitLine[1]);
                }
            }

            return true;
        }

        public async static Task<bool> LoadKeys()
        {
            var download = false;
            if (File.Exists("TactKey.csv"))
            {
                var info = new FileInfo("TactKey.csv");
                if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                {
                    Console.WriteLine("TACT Keys outdated, redownloading..");
                    download = true;
                }
            }
            else
            {
                download = true;
            }

            if (download)
            {
                Console.WriteLine("Downloading TACT keys");

                using var s = await WebClient.GetStreamAsync(SettingsManager.tactKeyURL);
                List<string> tactKeyLines = new();

                using (var sr = new StreamReader(s))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var splitLine = line.Split(" ");
                        tactKeyLines.Add(splitLine[0] + ";" + splitLine[1]);
                    }
                }

                File.WriteAllLines("TactKey.csv", tactKeyLines);
            }

            return true;
        }

        public static Stream GetFileByID(uint filedataid)
        {
            try
            {
                return cascHandler.OpenFile((int)filedataid);
            }
            catch (Exception e)
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
