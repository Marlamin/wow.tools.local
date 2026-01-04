using WoWNamingLib.Utils;
using WoWTagLib.DataSources;
using WoWTagLib.Types;

namespace wow.tools.local.Services
{
    public static class TagService
    {
        private static Repository TagRepo;

        static TagService()
        {
            if (string.IsNullOrEmpty(SettingsManager.TagRepo))
                throw new Exception("Tag repository path is not set in settings.");

            // TODO: When releases are going we should just download the SQLite DB from there
            TagRepo = new Repository(SettingsManager.TagRepo);
        }

        public static List<Tag> GetTags()
        {
            return TagRepo.GetTags();
        }

        public static List<WTLTag> GetTagsByFileDataID(int fdid)
        {
            var tagsByFDID = TagRepo.GetTagsByFileDataID(fdid);
            if (tagsByFDID.Count() > 0)
            {
                // We have to do this because tuple types don't get output properly
                var wtlTags = new List<WTLTag>();
                foreach (var tag in tagsByFDID)
                {
                    wtlTags.Add(new WTLTag(tag.Tag, tag.TagSource.ToString(), tag.TagValue));
                }
                return wtlTags;
            }
            else
            {
                return new List<WTLTag>();
            }
        }

        public static void AddOrUpdateTag(string name, string key, string description, string type, string category, bool allowMultiple)
        {
            TagRepo.AddOrUpdateTag(name, key, description, type, category, allowMultiple);
        }

        public static void DeleteTag(string key)
        {
            TagRepo.DeleteTag(key);
        }

        public static void AddOrUpdateTagOption(string tagKey, string name, string description, string aliases)
        {
            TagRepo.AddOrUpdateTagOption(tagKey, name, description, aliases);
        }

        public static void RemoveTagOption(string tagKey, string name)
        {
            TagRepo.DeleteTagOption(tagKey, name);
        }

        public static void AddTagToFDID(int fileDataID, string tagKey, string tagSource, string tagValue)
        {
            TagRepo.AddTagToFDID(fileDataID, tagKey, tagSource, tagValue);
        }

        public static void RemoveTagFromFDID(int fileDataID, string tagKey, string tagValue)
        {
            TagRepo.RemoveTagFromFDID(fileDataID, tagKey, tagValue);
        }

        public static bool HasUnsavedChanges()
        {
            return TagRepo.HasUnsavedChanges();
        }

        public static void ReloadRepo()
        {
            TagRepo.Load();
        }

        public static void SaveRepo()
        {
            TagRepo.Save();
        }
    }

    public record WTLTag(string TagName, string Source, string TagValue);
}
