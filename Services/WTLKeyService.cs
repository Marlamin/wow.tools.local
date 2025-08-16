namespace wow.tools.local.Services
{
    public class WTLKeyService
    {
        public static readonly List<ulong> KnownKeys = new();
        private static readonly HttpClient WebClient = new();

        public static bool TryGetKey(ulong keyName, out byte[] key)
        {
            if (CASC.IsCASCLibInit)
            {
                key = CASCLib.KeyService.GetKey(keyName);
                return key != null;
            }
            else if (CASC.IsTACTSharpInit)
            {
                return TACTSharp.KeyService.TryGetKey(keyName, out key);
            }
            else
            {
                throw new Exception("No TACT/CASC library initialized");
            }
        }

        public static byte[] GetKey(ulong keyName)
        {
            if (CASC.IsCASCLibInit)
                return CASCLib.KeyService.GetKey(keyName);
            else if (CASC.IsTACTSharpInit)
            {
                if (TACTSharp.KeyService.TryGetKey(keyName, out var key))
                    return key;

                return Array.Empty<byte>();
            }
            else
                throw new Exception("No TACT/CASC library initialized");
        }

        public static bool HasKey(ulong keyName)
        {
            if (CASC.IsCASCLibInit)
                return CASCLib.KeyService.HasKey(keyName);
            else if (CASC.IsTACTSharpInit)
                return TACTSharp.KeyService.TryGetKey(keyName, out _);
            else
                throw new Exception("No TACT/CASC library initialized");
        }

        public static void SetKey(ulong keyName, byte[] key)
        {
            if (!KnownKeys.Contains(keyName))
                KnownKeys.Add(keyName);

            if (CASC.IsCASCLibInit)
                CASCLib.KeyService.SetKey(keyName, key);
            else if (CASC.IsTACTSharpInit)
                TACTSharp.KeyService.SetKey(keyName, key);
            else
                throw new Exception("No TACT/CASC library initialized");
        }

        public static void LoadKeys()
        {
            if (!File.Exists("WoW.txt")) return;

            foreach (var line in File.ReadAllLines("WoW.txt"))
            {
                var splitLine = line.Split(' ');
                var lookup = ulong.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber);
                byte[] key = Convert.FromHexString(splitLine[1].Trim());

                if (!KnownKeys.Contains(lookup))
                    KnownKeys.Add(lookup);

                SetKey(lookup, key);
            }
        }

        public static bool LoadKeys(bool forceRedownload = false)
        {
            if (KnownKeys.Count > 0 || forceRedownload)
                KnownKeys.Clear();

            var download = forceRedownload;
            if (File.Exists("WoW.txt"))
            {
                var info = new FileInfo("WoW.txt");
                if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromHours(12)) > info.LastWriteTime)
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

                using (var s = WebClient.GetStreamAsync(SettingsManager.TACTKeyURL + "?=v" + (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds).Result)
                using (var fs = new FileStream("WoW.txt", FileMode.Create))
                {
                    s.CopyTo(fs);
                }
            }

            if (CASC.IsCASCLibInit || CASC.IsTACTSharpInit)
                LoadKeys();

            // If there are known statuses, make sure to reload.
            if (CASC.EncryptionStatuses.Count > 0)
                CASC.RefreshEncryptionStatus();

            Console.WriteLine("Loaded " + KnownKeys.Count + " known TACTkeys");

            return true;
        }

        public static bool ExportTACTKeys()
        {
            KnownKeys.Sort();

            var tactKeyLines = new List<string>();
            foreach (var key in KnownKeys)
            {
                if (!HasKey(key))
                    continue;

                tactKeyLines.Add(key.ToString("X16") + " " + Convert.ToHexString(GetKey(key)));
            }

            File.WriteAllLines("WoW.txt", tactKeyLines.ToArray());
            return true;
        }
    }
}
