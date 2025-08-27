using Microsoft.AspNetCore.Mvc;

namespace wow.tools.local.Controllers
{
    [Route("settings/")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [Route("list")]
        [HttpGet]
        public List<WTLSetting> List()
        {
            return SettingsManager.GetPersistentSettings();
        }

        [Route("save")]
        [HttpPost]
        public IActionResult Save([FromForm] Dictionary<string, string> newSettings)
        {
            foreach (var setting in SettingsManager.GetPersistentSettings())
            {
                if (newSettings.TryGetValue(setting.Key, out var newSetting) && newSetting != "null" && !string.IsNullOrEmpty(newSetting))
                {
                    if(SettingsManager.ValidateSetting(setting.Key, newSetting).Item1)
                        setting.Value = newSetting;
                }
                else
                    setting.Value = setting.DefaultValue;
            }

            // Save the updated settings to the config file
            SettingsManager.SaveSettings();
            return Ok();
        }

        [Route("check")]
        [HttpPost]
        public IActionResult Check([FromForm] string key, [FromForm] string? value)
        {
            var (isValid, message) = SettingsManager.ValidateSetting(key, value);
            if (isValid)
            {
                return Ok(message);
            }
            else
            {
                return BadRequest(message);
            }
        }
    }
}