using Newtonsoft.Json;

namespace Twitcher.Model
{
    public class Stream
    {
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("viewers")]
        public int Viewers { get; set; }
        [JsonProperty("_id")]
        public long Id { get; set; }
        [JsonProperty("channel")]
        public TwitchChannel Channel { get; set; }
    }
}
