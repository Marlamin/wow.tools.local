using System.IO;

namespace wow.tools.local.Services
{
    public struct HotfixEntry
    {
        public uint regionID;
        public int pushID;
        public uint uniqueID;
        public uint tableHash;
        public uint recordID;
        public int dataSize;
        public byte status;
        public byte[] data;
    }

    public class DBCacheParser
    {
        public List<HotfixEntry> hotfixes = [];
        public int build;

        public DBCacheParser(string filename)
        {
            using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bin = new BinaryReader(fs))
            {
                bin.ReadUInt32(); // Signature
                var version = bin.ReadUInt32();
                if (version != 9)
                    return;

                build = bin.ReadInt32();
                bin.BaseStream.Position += 32;

                while (bin.BaseStream.Position < bin.BaseStream.Length)
                {
                    bin.ReadUInt32(); // Signature
                    var hotfix = new HotfixEntry();
                    hotfix.regionID = bin.ReadUInt32();
                    hotfix.pushID = bin.ReadInt32();
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
