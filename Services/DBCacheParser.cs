namespace wow.tools.local.Services
{
    public struct HotfixEntry
    {
        public uint regionID;
        public uint pushID;
        public uint uniqueID;
        public uint tableHash;
        public uint recordID;
        public int dataSize;
        public byte status;
        public byte[] data;
    }

    public class DBCacheParser
    {
        public List<HotfixEntry> hotfixes = new();
        public DBCacheParser(string filename)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(filename)))
            using (var bin = new BinaryReader(ms))
            {
                var hotfix = new HotfixEntry();
                bin.ReadUInt32(); // Signature
                var version = bin.ReadUInt32();
                if (version != 9)
                {
                    Console.WriteLine("Unsupported DBCache version " + version + ", skipping");
                    return;
                }

                bin.BaseStream.Position += 36;

                while (bin.BaseStream.Position < bin.BaseStream.Length)
                {
                    bin.ReadUInt32(); // Signature
                    hotfix.regionID = bin.ReadUInt32();
                    hotfix.pushID = bin.ReadUInt32();
                    hotfix.uniqueID = bin.ReadUInt32();
                    hotfix.tableHash = bin.ReadUInt32();
                    hotfix.recordID = bin.ReadUInt32();
                    hotfix.dataSize = bin.ReadInt32();
                    hotfix.status = bin.ReadByte();
                    bin.ReadBytes(3);
                    hotfix.data = bin.ReadBytes(hotfix.dataSize);
                    hotfixes.Add(hotfix);
                }

            }
        }
    }
}
