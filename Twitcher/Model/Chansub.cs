using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Twitcher.Model
{
    [JsonObject("chansub")]
    public class Chansub
    {
        [JsonProperty("view_until")]
        //[JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public int ViewUntil { get; set; }

        [JsonProperty("restricted_bitrates")]
        public IEnumerable<int> RestrictedBitrates { get; set; }
    }
}