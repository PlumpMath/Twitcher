using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Timers;

namespace Twitcher.Model
{
    public class TwitcherModel : INotifyPropertyChanged
    {
        #region Declarations
        private const int REFRESH_INTERVAL = 60000;
        private HttpClient krakenClient;
        private HttpClient apiClient;
        private HttpClient usherClient;
        private ObservableCollection<TwitchChannel> _followingList;
        private ObservableCollection<TwitchChannel> _streamingList;
        private StreamQuality _quality = StreamQuality.Low;
        private ObservableCollection<StreamQuality> _qualities = new ObservableCollection<StreamQuality> { StreamQuality.Low, StreamQuality.Medium, StreamQuality.High, StreamQuality.Source };
        private string userName;
        private TwitchChannel selectedChannel;
        private bool loadButtonIsEnabled = true;
        private string mediaPlayerFilename;
        private static Timer timer = new Timer(REFRESH_INTERVAL);

        #region Properties
        public ObservableCollection<StreamQuality> Qualities
        {
            get { return _qualities; }
            set
            {
                if (value != _qualities)
                {
                    _qualities = value;
                    OnPropertyChanged();
                }
            }
        }
        public StreamQuality Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                if (value != _quality)
                {
                    _quality = value;
                    OnPropertyChanged();
                }
            }
        }
        public string UserName
        {
            get { return userName; }
            set
            {
                if (value != userName)
                {
                    userName = value;
                    OnPropertyChanged();
                }
            }
        }
        public TwitchChannel SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                if (value != selectedChannel)
                {
                    selectedChannel = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<TwitchChannel> StreamingList
        {
            get
            {
                return _streamingList;
            }
            private set
            {
                if (_streamingList != value)
                {
                    _streamingList = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool LoadButtonIsEnabled
        {
            get { return loadButtonIsEnabled; }
            set
            {
                if (value != loadButtonIsEnabled)
                {
                    loadButtonIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<TwitchChannel> FollowingList
        {
            get
            {
                return _followingList;
            }
            private set
            {
                if (_followingList != value)
                {
                    _followingList = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MediaPlayerFilename
        {
            get
            {
                return mediaPlayerFilename;
            }

            set
            {
                this.mediaPlayerFilename = value;
            }
        }
        #endregion Properties

        public TwitcherModel()
        {
            krakenClient = new HttpClient();
            krakenClient.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
            krakenClient.DefaultRequestHeaders.Accept.Clear();
            krakenClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            krakenClient.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v2+json");
            krakenClient.DefaultRequestHeaders.Add("Keep-Alive", "true");

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri("https://api.twitch.tv/api/");
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("Keep-Alive", "true");

            usherClient = new HttpClient();
            usherClient.BaseAddress = new Uri("http://usher.twitch.tv/api/");
            usherClient.DefaultRequestHeaders.Accept.Clear();
            usherClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            timer.Elapsed += async delegate(object sender, ElapsedEventArgs e)
            {
                if (LoadButtonIsEnabled)
                    await LoadFollowingListForUsername();
            };
            timer.Start();
        }
        #endregion Declarations

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Methods
        public async void Start()
        {
            using (Process vlc = new Process())
            {
                vlc.StartInfo.FileName = "vlc";
                M3uPlaylist playlist = await getPlaylistForSelectedChannel();
                vlc.StartInfo.Arguments = playlist.Links[Quality];
                vlc.Start();
                vlc.WaitForExit();
            }
        }

        public async Task LoadFollowingListForUsername()
        {
            LoadButtonIsEnabled = false;
            StreamingList = StreamingList ?? new ObservableCollection<TwitchChannel>();

            HttpResponseMessage response = await krakenClient.GetAsync(String.Format("users/{0}/follows/channels", userName));
            if (response.IsSuccessStatusCode)
            {
                var tasklist = new List<Task<TwitchStream>>();
                TwitchFollowsReturnObject obj = await response.Content.ReadAsAsync<TwitchFollowsReturnObject>();
                foreach (TwitchChannel channel in obj.Follows.Select(item => item.Channel).Where(item => !StreamingList.Contains(item)))
                {
                    tasklist.Add(GetResponseAsync(channel.DisplayName));
                }
                try 
                { 
                    await Task.WhenAll(tasklist.ToArray()); 
                }
                catch { }

                foreach (Task<TwitchStream> t in tasklist)
                {
                    if (t.IsCompleted)
                    {
                        TwitchStream stream = t.Result;
                        if (stream.Stream != null)
                            StreamingList = new ObservableCollection<TwitchChannel>(StreamingList ?? new ObservableCollection<TwitchChannel>()) { stream.Stream.Channel };
                    }
                }

            }
            else
            {
                throw new ModelException();
            }
            LoadButtonIsEnabled = true;
        }

        public async Task<TwitchStream> GetResponseAsync(string myrequest)
        {
            HttpResponseMessage msg = await krakenClient.GetAsync(string.Format("streams/{0}", myrequest));
            return msg.IsSuccessStatusCode ? await msg.Content.ReadAsAsync<TwitchStream>() : null;
        }

        private async Task<TokenObject> getToken()
        {

            HttpResponseMessage response = await apiClient.GetAsync(string.Format("channels/{0}/access_token", SelectedChannel.DisplayName));
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<TokenObject>();
            else
                return null;
        }

        private async Task<M3uPlaylist> getPlaylistForSelectedChannel()
        {
            TokenObject param = await getToken();
            return new M3uPlaylist(
                await usherClient.GetAsync(
                    getUsherApiUri(
                        selectedChannel.DisplayName
                        , param.Token
                        , param.Sig))
                .Result
                .Content
                .ReadAsStringAsync());
        }

        private string getUsherApiUri(string channel, string token, string sig)
        {
            Random r = new Random();
            return string.Format("channel/hls/{0}.m3u8?player=twitchweb&&token={1}&sig={2}&allow_audio_only=true&allow_source=true&type=any&p={3}"
                , channel.ToLower()
                , token
                , sig
                , r.Next().ToString());
        }
        #endregion Methods

    }
}
