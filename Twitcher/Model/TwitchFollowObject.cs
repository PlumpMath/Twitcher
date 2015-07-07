using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twitcher.Model
{
    public class TwitchFollowObject
    {
        [JsonProperty("created_at")]
        public DateTime Created { get; set; }
        [JsonProperty("_links")]
        public IDictionary<string, object> Links { get; set; }
        [JsonProperty("notifications")]
        public bool Notifications { get; set; }
        [JsonProperty("channel")]
        public TwitchChannel Channel { get; set; }

    }
}