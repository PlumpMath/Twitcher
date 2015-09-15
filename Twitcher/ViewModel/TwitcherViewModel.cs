using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Twitcher.Model;

namespace Twitcher.ViewModel
{
    public class TwitcherViewModel : ViewModelBase
    {


        private TwitcherModel _model;


        #region Properties 
        public bool LoadButtonIsEnabled
        {
            get
            {
                return _model.LoadButtonIsEnabled;
            }

            set
            {
                OnPropertyChanged();
            }
        }

        public StreamQuality Quality
        {
            get
            {
                return _model.Quality;
            }
            set
            {
                if (value != _model.Quality)
                    _model.Quality = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StreamQuality> Qualities
        {
            get
            {
                return _model.Qualities;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public TwitchChannel SelectedChannel
        {
            get
            {
                return _model.SelectedChannel;
            }
            set
            {
                if (_model.SelectedChannel != value)
                    _model.SelectedChannel = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get
            {
                return _model.UserName;
            }
            set
            {
                if (_model.UserName != value)
                    _model.UserName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TwitchChannel> StreamingList
        {
            get
            {
                return _model.StreamingList;
            }
            private set
            {
                OnPropertyChanged();
            }
        }
        #endregion

        private void TwitcherModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.GetType()
                .GetProperties()
                .FirstOrDefault(item => item.Name == e.PropertyName)
                .SetValue(this,
                   sender
                   .GetType()
                   .GetProperties()
                   .FirstOrDefault(item => item.Name == e.PropertyName)
                   .GetValue(sender));
        }


        public TwitcherViewModel(TwitcherModel model)
        {
            _model = model;

            _model.PropertyChanged += TwitcherModel_PropertyChanged;

            #region DelegateCommands
            LoadChannelsCommand = new DelegateCommand(async param =>
            {
                OnLoadStarted();   
                try
                {
                    await _model.LoadFollowingListForUsername();
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
                    //_model.StartLivestreamerWithChannelName();
                    _model.Start();
                }
                catch
                {

                }
            });

            ExitCommand = new DelegateCommand(param => 
            {
                OnExitCommand();
            });

            OpenYoutubeWindowCommand = new DelegateCommand(param =>
            {
                if (YoutubeWindowCommandInvoked != null)
                    YoutubeWindowCommandInvoked(this, EventArgs.Empty);
            });
            #endregion DelegateCommands
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
        public EventHandler YoutubeWindowCommandInvoked;
        


        public DelegateCommand LoadChannelsCommand { get; private set; }
        public DelegateCommand RunLivestreamerCommand { get; private set; }
        public DelegateCommand ChangeStreamQualityCommand { get; private set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand OpenYoutubeWindowCommand { get; private set; }
    }
}
