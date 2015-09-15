using Newtonsoft.Json;
namespace Twitcher.Model
{
    public class TokenObject
    {
        [JsonProperty( "token" )]
        public string Token { get; set; }

        [JsonProperty( "sig" )]
        public string Sig { get; set; }

        [JsonProperty( "mobile_restricted" )]
        public bool MobileRestricted { get; set; }
    }
}