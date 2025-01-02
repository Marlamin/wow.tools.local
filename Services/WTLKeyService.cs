namespace wow.tools.local.Services
{
    public class WTLKeyService
    {
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
                if (TryGetKey(keyName, out var key))
                    return key;

                return null;
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
                SetKey(lookup, key);
            }
        }
    }
}
