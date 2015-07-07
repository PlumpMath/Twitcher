using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Twitcher.Model;

namespace Twitcher.ViewModel
{
    public class TwitcherViewModel : ViewModelBase
    {


        private TwitcherModel _model;
        private ObservableCollection<TwitchChannel> _streamingList;
        private TwitchChannel _selectedChannel;
        private string _userName;


        


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

            LoadChannelsCommand = new DelegateCommand(async (param) =>
            {
                try
                {
                    await _model.LoadFollowingListForUsername(_userName);
                    StreamingList = new ObservableCollection<TwitchChannel>(_model.StreamingList);
                }
                catch
                {

                }
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

        private void OnExitCommand()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }

        public EventHandler ExitApplication;


        public DelegateCommand LoadChannelsCommand { get; private set; }
        public DelegateCommand RunLivestreamerCommand { get; private set; }
        public DelegateCommand ExitCommand { get; set; }
    }
}
