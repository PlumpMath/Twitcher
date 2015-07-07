using Newtonsoft.Json;

namespace Twitcher.Model
{
    public class TwitchGame
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
