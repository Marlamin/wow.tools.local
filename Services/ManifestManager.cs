namespace wow.tools.local.Services
{
    public static class ManifestManager
    {
        private static readonly bool IsNewFormatEnabled = true;

        public static bool ExistsForBuild(string patch, string build)
        {
            return ExistsForVersion(patch + "." + build);
        }

        public static bool ExistsForVersion(string version)
        {
            if (IsNewFormatEnabled && File.Exists(Path.Combine(SettingsManager.ManifestFolder, version + ".wtlm")))
                return true;

            if (File.Exists(Path.Combine(SettingsManager.ManifestFolder, version + ".txt")))
                return true;

            return false;
        }

        public static List<(uint FileDataID, byte[] MD5)> GetEntriesForVersion(string version)
        {
            if (!ExistsForVersion(version))
                throw new FileNotFoundException("Manifest file not found for version " + version);

            var entries = new List<(uint FileDataID, byte[] MD5)>();
            if (IsNewFormatEnabled && File.Exists(Path.Combine(SettingsManager.ManifestFolder, version + ".wtlm")))
            {
                using var fs = File.OpenRead(Path.Combine(SettingsManager.ManifestFolder, version + ".wtlm"));
                using var br = new BinaryReader(fs);

                var magic = br.ReadBytes(4);
                if (magic[0] != (byte)'W' || magic[1] != (byte)'T' || magic[2] != (byte)'L' || magic[3] != (byte)'M')
                    throw new InvalidDataException("Invalid manifest file format.");

                var fileVersion = br.ReadUInt32();
                if (fileVersion != 1)
                    throw new InvalidDataException("Unsupported manifest file version.");

                var count = br.ReadUInt32();

                // Skip reserved
                br.ReadBytes(12);

                for (uint i = 0; i < count; i++)
                    entries.Add((br.ReadUInt32(), br.ReadBytes(16)));
            }
            else if (File.Exists(Path.Combine(SettingsManager.ManifestFolder, version + ".txt")))
            {
                var lines = File.ReadAllLines(Path.Combine(SettingsManager.ManifestFolder, version + ".txt"));
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2)
                        entries.Add((uint.Parse(parts[0]), Convert.FromHexString(parts[1])));
                }

            }
            return entries;
        }

        public static async Task<List<(uint FileDataID, byte[] MD5)>> GetEntriesForVersionAsync(string version)
        {
            if (!ExistsForVersion(version))
                throw new FileNotFoundException("Manifest file not found for version " + version);

            var entries = new List<(uint FileDataID, byte[] MD5)>();

            if (IsNewFormatEnabled)
            {
                var path = Path.Combine(SettingsManager.ManifestFolder, version + ".wtlm");
                if (!File.Exists(path))
                    throw new FileNotFoundException("Manifest file not found for version " + version);

                using var fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    FileOptions.SequentialScan);

                using var bs = new BufferedStream(fs, 1024 * 1024); // 1MB buffer
                using var bin = new BinaryReader(bs);

                if (bin.ReadByte() != (byte)'W' || bin.ReadByte() != (byte)'T' ||
                    bin.ReadByte() != (byte)'L' || bin.ReadByte() != (byte)'M')
                    throw new InvalidDataException("Bad magic.");

                uint manifestVersion = bin.ReadUInt32();
                if (manifestVersion != 1)
                    throw new InvalidDataException("Unsupported manifest file version.");

                uint count = bin.ReadUInt32();
                entries.EnsureCapacity((int)count);

                // Skip reserved
                bin.ReadBytes(12);

                for (uint i = 0; i < count; i++)
                    entries.Add((bin.ReadUInt32(), bin.ReadBytes(16)));
            }
            else if (File.Exists(Path.Combine(SettingsManager.ManifestFolder, version + ".txt")))
            {
                var lines = await File.ReadAllLinesAsync(Path.Combine(SettingsManager.ManifestFolder, version + ".txt"));
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2)
                        entries.Add((uint.Parse(parts[0]), Convert.FromHexString(parts[1])));
                }
            }
            return entries;
        }

        public static List<string> GetManifestVersions()
        {
            List<string> versions = new List<string>();
            if (Directory.Exists(SettingsManager.ManifestFolder))
            {
                var files = Directory.GetFiles(SettingsManager.ManifestFolder, "*.*");
                foreach (var file in files)
                {
                    if (!Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase) && (IsNewFormatEnabled && !Path.GetExtension(file).Equals(".wtlm", StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var fileName = Path.GetFileNameWithoutExtension(file);

                    if (!versions.Contains(fileName))
                        versions.Add(fileName);
                }
            }
            return versions;
        }

        public static void ConvertAllTxtToWtlm()
        {
            if (Directory.Exists(SettingsManager.ManifestFolder))
            {
                var files = Directory.GetFiles(SettingsManager.ManifestFolder, "*.txt");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);

                    var entries = GetEntriesForVersion(fileName);
                    Write(fileName, entries);

                    Console.WriteLine("Converted manifest " + fileName + " to .wtlm format.");
                    //File.Delete(file);
                }
            }
        }

        public static void Write(string version, List<(uint FileDataID, byte[] MD5)> entries)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.Write("Writing manifest for version " + version + "... ");

            // sort by ID
            entries.Sort((a, b) => a.FileDataID.CompareTo(b.FileDataID));

            if (IsNewFormatEnabled)
            {
                var path = Path.Combine(SettingsManager.ManifestFolder, version + ".wtlm");

                using var fs = new FileStream(
                    path,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.SequentialScan);

                fs.SetLength(20 + (entries.Count * 20));

                using var bs = new BufferedStream(fs, 1024 * 1024);
                using var bw = new BinaryWriter(bs);

                bw.Write((byte)'W');
                bw.Write((byte)'T');
                bw.Write((byte)'L');
                bw.Write((byte)'M');

                bw.Write((uint)1);      // Version
                bw.Write((uint)0);      // Count placeholder
                bw.Write(new byte[12]); // Reserved

                uint count = 0;
                foreach (var r in entries)
                {
                    bw.Write(r.FileDataID);
                    bw.Write(r.MD5);
                    count++;
                }

                
                bw.Flush();         // Not doing this caused bad things to happen
                fs.Position = 8;    // Go back to count pos
                bw.Write(count);

                bw.Flush();
            }
            else
            {
                List<string> lines = new List<string>();
                foreach (var entry in entries)
                {
                    lines.Add(entry.FileDataID + ";" + Convert.ToHexString(entry.MD5));
                }
                File.WriteAllLines(Path.Combine(SettingsManager.ManifestFolder, version + ".txt"), lines);
            }

            sw.Stop();
            Console.WriteLine("done in " + sw.Elapsed.TotalMilliseconds + "ms.");
        }
    }
}
