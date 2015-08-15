using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitcher.Model;
using System.Reflection;

namespace Twitcher.ViewModel
{
    public class YoutubeViewModel : ViewModelBase
    {
        private YoutubeModel model;


        #region Properties

        #region Model properties
        //for these properties, the setter only invokes OnPropertyChanged
        public ObservableCollection<string> YoutubeAvailableVideoQualities
        {
            get
            {
                return model.YoutubeAvailableVideoQualities;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public string YoutubeSelectedQuality
        {
            get
            {
                return model.YoutubeSelectedQuality;
            }
            set
            {
                if (model.YoutubeSelectedQuality != value)
                    model.YoutubeSelectedQuality = value;
                OnPropertyChanged();
            }
        }

        public string YoutubeVideoUrl
        {
            get
            {
                return model.YoutubeVideoUrl;
            }
            set
            {
                if (model.YoutubeVideoUrl != value)
                    model.YoutubeVideoUrl = value;
                OnPropertyChanged();
            }
        }

        public string YoutubeVideoFileName
        {
            get
            {
                return model.YoutubeVideoFileName;
            }
            set
            {
                if (model.YoutubeVideoFileName != value)
                {
                    model.YoutubeVideoFileName = value;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Events
        public event EventHandler Model_DownloadFinished;
        public event EventHandler TwitcherCommandInvoked;

        private void OnTwitcherWindowCommandInvoked()
        {
            if (TwitcherCommandInvoked != null)
                TwitcherCommandInvoked(this, EventArgs.Empty);
        }
        #endregion Events


        public YoutubeViewModel(YoutubeModel model)
        {
            this.model = model;

            //dinamically link the properties of the model through the viewmodel
            //only works if the property names are the same.
            model.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) 
            {
                this.GetType()
                    .GetProperties()
                    .FirstOrDefault((pi) => pi.Name == e.PropertyName)
                    .SetValue(this, sender.GetType()
                        .GetProperties()
                        .FirstOrDefault((pi) => pi.Name == e.PropertyName)
                        .GetValue(sender));
            };

            model.DownloadFinished += delegate(object sender, EventArgs e)
            {
                if (this.Model_DownloadFinished != null)
                    Model_DownloadFinished(this, EventArgs.Empty);
            };

            YoutubeDownloadVideoCommand = new DelegateCommand(param => 
            {
                try
                {
                    model.DownloadVideo();
                }
                catch { }
            });
            YoutubeUrlTextChangedCommand = new DelegateCommand(param =>
            {
                try
                {
                    model.GetVideoQualityOptions();
                }
                catch { }
            });

            TwitcherWindowCommand = new DelegateCommand(param =>
            {
                OnTwitcherWindowCommandInvoked();
            });
        }

        public DelegateCommand YoutubeDownloadVideoCommand { get; set; }
        public DelegateCommand YoutubeUrlTextChangedCommand { get; set; }
        public DelegateCommand TwitcherWindowCommand { get; set; }
    }
}
