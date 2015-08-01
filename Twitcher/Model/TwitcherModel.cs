using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Twitcher.Model
{
    public class TwitcherModel
    {

        private HttpClient _twitchClient;
        private List<TwitchChannel> _followingList;
        private List<TwitchChannel> _streamingList;
        private TwitchFollowsReturnObject _obj;
        private StreamQuality _quality = StreamQuality.Low;


        public StreamQuality Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                if (value != _quality)
                    _quality = value;
            }
        }

        public List<TwitchChannel> StreamingList
        {
            get
            {
                return _streamingList;
            }
            private set
            {
                if(_streamingList != value)
                {
                    _streamingList = value;
                }
            }
        }

        public List<TwitchChannel> FollowingList
        {
            get
            {
                return _followingList;
            }
            private set
            {
                if(_followingList != value)
                {
                    _followingList = value;
                }
            }
        }

        public TwitcherModel()
        {
            _twitchClient = new HttpClient();
            _twitchClient.BaseAddress = new System.Uri("https://api.twitch.tv/kraken/");
            _twitchClient.DefaultRequestHeaders.Accept.Clear();
            _twitchClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _twitchClient.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v2+json");
        }


        public async Task LoadFollowingListForUsername(string username)
        {
            HttpResponseMessage response = await _twitchClient.GetAsync("users/" + username + "/follows/channels");
            if (response.IsSuccessStatusCode)
            {
                _obj = await response.Content.ReadAsAsync<TwitchFollowsReturnObject>();
                _streamingList = new List<TwitchChannel>();
                foreach(TwitchFollowObject followObject in _obj.Follows)
                {
                    response = await _twitchClient.GetAsync("streams/" + followObject.Channel.DisplayName);
                    if (response.IsSuccessStatusCode)
                    {
                        TwitchStream stream = await response.Content.ReadAsAsync<TwitchStream>();
                        if(stream.Stream != null)
                        {
                            StreamingList.Add(followObject.Channel);
                        }
                    }
                }

            }
            else
            {
                throw new ModelException();
            }
        }

        public void StartLivestreamerWithChannelName(string name)
        {
            Process livestreamer = new Process();
            livestreamer.StartInfo.FileName = "livestreamer";
            livestreamer.StartInfo.Arguments = " twitch.tv/" + name + " " + _quality.ToString();
            livestreamer.StartInfo.CreateNoWindow = true;
            livestreamer.StartInfo.UseShellExecute = false;
            livestreamer.Start();
            livestreamer.WaitForExit();
        }

    }
}
