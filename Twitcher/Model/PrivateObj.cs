using Newtonsoft.Json;

namespace Twitcher.Model
{
    public class PrivateObj
    {
        [JsonProperty("allowed_to_view")]
        public bool AllowedToView { get; set; }
    }
}