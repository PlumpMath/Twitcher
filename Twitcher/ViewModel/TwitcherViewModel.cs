using System;
using System.Collections.ObjectModel;
using Twitcher.Model;

namespace Twitcher.ViewModel
{
    public class TwitcherViewModel : ViewModelBase
    {


        private TwitcherModel _model;
        private StreamQuality _quality = StreamQuality.Low;
        private ObservableCollection<TwitchChannel> _streamingList;
        public ObservableCollection<StreamQuality> _qualities = new ObservableCollection<StreamQuality> { StreamQuality.Low, StreamQuality.Medium, StreamQuality.High, StreamQuality.Source };
        private TwitchChannel _selectedChannel;
        private string _userName;
        private bool _loadButtonIsEnabled;


        public bool LoadButtonIsEnabled
        {
            get
            {
                return _loadButtonIsEnabled;
            }

            set
            {
                if(value != _loadButtonIsEnabled)
                {
                    _loadButtonIsEnabled = value;
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
                if(value != _quality)
                {
                    _quality = value;
                    _model.Quality = _quality;
                }
            }
        }

        public ObservableCollection<StreamQuality> Qualities
        {
            get
            {
                return _qualities;
            }
            set
            {
                if (value != _qualities)
                    _qualities = value;
            }
        }

        public TwitchChannel SelectedChannel
        {
            get
            {
                return _selectedChannel;
            }
            set
            {
                if(_selectedChannel != value)
                {
                    _selectedChannel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if(_userName != value)
                {
                    _userName = value;
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

        public TwitcherViewModel(TwitcherModel model)
        {
            _model = model;
            _streamingList = new ObservableCollection<TwitchChannel>();
            _loadButtonIsEnabled = true;

            LoadChannelsCommand = new DelegateCommand(async (param) =>
            {
                OnLoadStarted();   
                try
                {
                    await _model.LoadFollowingListForUsername(_userName);
                    StreamingList = new ObservableCollection<TwitchChannel>(_model.StreamingList);
                }
                catch
                {

                }
                OnLoadFinished();
            });

            RunLivestreamerCommand = new DelegateCommand(param =>
            {
                try
                {
                   _model.StartLivestreamerWithChannelName(SelectedChannel.DisplayName);
                }
                catch
                {

                }
            });

            ExitCommand = new DelegateCommand(param => 
            {
                OnExitCommand();
            });
        }

        private void OnLoadStarted()
        {
            LoadButtonIsEnabled = false;
        }

        private void OnLoadFinished()
        {
            LoadButtonIsEnabled = true;
        }

        private void OnExitCommand()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }

        public EventHandler ExitApplication;
        


        public DelegateCommand LoadChannelsCommand { get; private set; }
        public DelegateCommand RunLivestreamerCommand { get; private set; }
        public DelegateCommand ChangeStreamQualityCommand { get; private set; }
        public DelegateCommand ExitCommand { get; set; }
    }
}
