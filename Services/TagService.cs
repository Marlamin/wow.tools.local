using WoWTagLib.DataSources;
using WoWTagLib.Types;

namespace wow.tools.local.Services
{
    public static class TagService
    {
        private static Repository TagRepo;
        private static bool Enabled = false;

        static TagService()
        {
            if (string.IsNullOrEmpty(SettingsManager.TagRepo))
            {
                Enabled = false;
                return;
            }
            
            // TODO: When releases are going we should just download the SQLite DB from there
            TagRepo = new Repository(SettingsManager.TagRepo);
        }

        public static List<Tag> GetTags()
        {
            if (!Enabled)
                return [];

            return TagRepo.GetTags();
        }

        public static List<WTLTag> GetTagsByFileDataID(int fdid)
        {
            if (!Enabled)
                return [];

            var tagsByFDID = TagRepo.GetTagsByFileDataID(fdid);
            if (tagsByFDID.Count > 0)
            {
                // We have to do this because tuple types don't get output properly
                var wtlTags = new List<WTLTag>();
                foreach (var tag in tagsByFDID)
                {
                    wtlTags.Add(new WTLTag(tag.Tag, tag.TagValue));
                }
                return wtlTags;
            }
            else
            {
                return new List<WTLTag>();
            }
        }

        public static List<int> GetFileDataIDsByTag(string tagKey)
        {
            if(!Enabled)
                return new List<int>();

            return TagRepo.GetFileDataIDsByTag(tagKey).Select(x => x.FileDataID).ToList();
        }

        public static List<int> GetFileDataIDsByTagAndValue(string tagKey, string tagValue)
        {
            if(!Enabled)
                return new List<int>();

            return TagRepo.GetFileDataIDsByTagAndValue(tagKey, tagValue);
        }

        public static void AddOrUpdateTag(string name, string key, string description, string type, string source, string category, bool allowMultiple, string status)
        {
            if(!Enabled)
                return;

            TagRepo.AddOrUpdateTag(name, key, description, type, source, category, allowMultiple, status);
        }

        public static void DeleteTag(string key)
        {
            if(!Enabled)
                return;

            TagRepo.DeleteTag(key);
        }

        public static void AddOrUpdateTagOption(string tagKey, string name, string description, string aliases)
        {
            if(!Enabled)
                return;

            TagRepo.AddOrUpdateTagOption(tagKey, name, description, aliases);
        }

        public static void RemoveTagOption(string tagKey, string name)
        {
            if(!Enabled)
                return;

            TagRepo.DeleteTagOption(tagKey, name);
        }

        public static void AddTagToFDID(int fileDataID, string tagKey, string tagValue)
        {
            if(!Enabled)
                return;

            TagRepo.AddTagToFDID(fileDataID, tagKey, tagValue);
        }

        public static void RemoveTagFromFDID(int fileDataID, string tagKey, string tagValue)
        {
            if(!Enabled)
                return;

            TagRepo.RemoveTagFromFDID(fileDataID, tagKey, tagValue);
        }

        public static bool RequiresSavingStep()
        {
            if(!Enabled)
                return false;

            return TagRepo.RequiresSavingStep();
        }

        public static bool HasUnsavedChanges()
        {
            if(!Enabled)
                return false;

            return TagRepo.HasUnsavedChanges();
        }

        public static void ReloadRepo()
        {
            if (!Enabled)
                return;

            TagRepo.Load();
        }

        public static void SaveRepo()
        {
            if (!Enabled)
                return;

            TagRepo.Save();
        }
    }

    public record WTLTag(string TagName, string TagValue);
}
