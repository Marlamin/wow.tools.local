using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("sql/")]
    [ApiController]
    public class SQLController() : Controller
    {
        [Route("query")]
        [HttpGet]
        public IActionResult Query(string query)
        {
            if (SettingsManager.ReadOnly)
                return Forbid();

            return Ok(SQLiteDB.RunQuery(query));
        }

        [Route("getCreatureName")]
        [HttpGet]
        public IActionResult GetCreatureName(int creatureID)
        {
            var name = SQLiteDB.GetCreatureNameByID(creatureID);
            if (string.IsNullOrEmpty(name))
            {
                var creatureData = Archavon.GetCreatureData(creatureID);
                if (creatureData != null && creatureData.TryGetValue("name_0", out var creatureName))
                {
                    name = creatureName;

                    if(int.TryParse(creatureData["game_build"], out var gameBuild))
                        SQLiteDB.InsertOrUpdateCreature(creatureID, name, gameBuild);
                }
            }

            if (string.IsNullOrEmpty(name))
                return NotFound();
            else
                return Ok(name);
        }

        [Route("getQuestName")]
        [HttpGet]
        public IActionResult GetQuestName(int questID)
        {
            var name = SQLiteDB.GetQuestNameByID(questID.ToString());
            if (string.IsNullOrEmpty(name))
            {
                var questData = Archavon.GetQuestData(questID);
                if (questData != null && questData.TryGetValue("log_title", out var questName))
                {
                    name = questName;

                    if(int.TryParse(questData["game_build"], out var gameBuild))
                        SQLiteDB.InsertOrUpdateQuest(questID, name, gameBuild);
                }
            }

            if (string.IsNullOrEmpty(name))
                return NotFound();
            else
                return Ok(name);
        }
    }
}