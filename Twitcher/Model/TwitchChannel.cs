using Newtonsoft.Json;

namespace Twitcher.Model
{
    public class TwitchChannel
    {
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }
}
