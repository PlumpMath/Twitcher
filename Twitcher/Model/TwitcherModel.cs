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

namespace Twitcher.Model
{
    public class TwitcherModel : INotifyPropertyChanged
    {
        #region Declarations
        private HttpClient _twitchClient;
        private ObservableCollection<TwitchChannel> _followingList;
        private ObservableCollection<TwitchChannel> _streamingList;
        private StreamQuality _quality = StreamQuality.Low;
        private ObservableCollection<StreamQuality> _qualities = new ObservableCollection<StreamQuality> { StreamQuality.Low, StreamQuality.Medium, StreamQuality.High, StreamQuality.Source };
        private string userName;
        private TwitchChannel selectedChannel;
        private bool loadButtonIsEnabled = true;

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
                if(_streamingList != value)
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
                if(_followingList != value)
                {
                    _followingList = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion Properties

        public TwitcherModel()
        {
            _twitchClient = new HttpClient();
            _twitchClient.BaseAddress = new System.Uri("https://api.twitch.tv/kraken/");
            _twitchClient.DefaultRequestHeaders.Accept.Clear();
            _twitchClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _twitchClient.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v2+json");
        }
        #endregion Declarations

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Methods
        public async Task LoadFollowingListForUsername()
        {
            LoadButtonIsEnabled = false;
            StreamingList = StreamingList ?? new ObservableCollection<TwitchChannel>();

            HttpResponseMessage response = await _twitchClient.GetAsync(String.Format("users/{0}/follows/channels", userName));
            if (response.IsSuccessStatusCode)
            {
                var tasklist = new List<Task<TwitchStream>>();
                TwitchFollowsReturnObject obj = await response.Content.ReadAsAsync<TwitchFollowsReturnObject>();
                foreach (TwitchChannel channel in obj.Follows.Select(item => item.Channel).Where(item => !StreamingList.Contains(item)))
                {
                    tasklist.Add(GetResponseAsync(channel.DisplayName));
                }
                try { await Task.WhenAll(tasklist.ToArray()); }
                catch {}

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
            HttpResponseMessage msg = await _twitchClient.GetAsync(String.Format("streams/{0}",myrequest));
            return msg.IsSuccessStatusCode ? await msg.Content.ReadAsAsync<TwitchStream>() : null;
        }

        public void StartLivestreamerWithChannelName()
        {
            Process livestreamer = new Process();
            livestreamer.StartInfo.FileName = "livestreamer";
            livestreamer.StartInfo.Arguments = string.Format(" twitch.tv/{0} {1}", selectedChannel.DisplayName, _quality.ToString());
            livestreamer.StartInfo.CreateNoWindow = true;
            livestreamer.StartInfo.UseShellExecute = false;
            livestreamer.Start();
            livestreamer.WaitForExit();
        }
        #endregion Methods

    }
}
