using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twitcher.Model
{
    public class TwitchStream
    {
        [JsonProperty("_links")]
        public Dictionary<string, object> Links { get; set; }

        [JsonProperty("stream")]
        public Stream Stream { get; set; }
    }
}
