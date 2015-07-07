using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twitcher.Model
{
    internal class TwitchFollowsReturnObject
    {
        [JsonProperty("_links")]
        public IDictionary<string, object> Links { get; set; }
        [JsonProperty("_total")]
        public int Total { get; set; }
        [JsonProperty("follows")]
        public List<TwitchFollowObject> Follows { get; set; }
    }
}