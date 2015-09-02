using Newtonsoft.Json;

namespace Twitcher.Model
{
    public class TwitchChannel
    {
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            TwitchChannel other = (TwitchChannel)obj;
            if (Game == other.Game && DisplayName == other.DisplayName)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
