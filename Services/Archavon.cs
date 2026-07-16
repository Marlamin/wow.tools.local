using System.Text.Json;

namespace wow.tools.local.Services
{
    public static class Archavon
    {
        private static Dictionary<int, Dictionary<string, string>> CreatureCache = [];
        private static Dictionary<int, Dictionary<string, string>> QuestCache = [];
        private static HttpClient HttpClient = new HttpClient();

        static Archavon()
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");
        }

        public static Dictionary<string, string>? GetCreatureData(int creatureID)   
        {
            if (CreatureCache.TryGetValue(creatureID, out var cachedData))
                return cachedData;

            var response = HttpClient.GetAsync($"https://archavon.kruithne.net/api/v1/creatures/{creatureID}?locale=enUS").Result;
            var json = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            var versions = json.RootElement.GetProperty("versions");

            if(versions.GetArrayLength() == 0)
                return null;

            var firstVersion = versions[0];
            foreach(var kvp in firstVersion.EnumerateObject())
            {
                if (!CreatureCache.ContainsKey(creatureID))
                    CreatureCache[creatureID] = new Dictionary<string, string>();

                CreatureCache[creatureID][kvp.Name] = kvp.Value.ToString() ?? "";
            }

            return CreatureCache[creatureID];
        }

        public static Dictionary<string, string>? GetQuestData(int questID)
        {
            if (QuestCache.TryGetValue(questID, out var cachedData))
                return cachedData;

            var response = HttpClient.GetAsync($"https://archavon.kruithne.net/api/v1/quests/{questID}?locale=enUS").Result;
            var json = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            var versions = json.RootElement.GetProperty("versions");

            if (versions.GetArrayLength() == 0)
                return null;

            var firstVersion = versions[0];
            foreach (var kvp in firstVersion.EnumerateObject())
            {
                if (!QuestCache.ContainsKey(questID))
                    QuestCache[questID] = new Dictionary<string, string>();

                QuestCache[questID][kvp.Name] = kvp.Value.ToString() ?? "";
            }

            return QuestCache[questID];
        }
    }
}
