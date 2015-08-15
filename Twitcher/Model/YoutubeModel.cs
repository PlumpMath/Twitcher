using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Twitcher.Model
{
    public class YoutubeModel : INotifyPropertyChanged
    {
        #region Declarations
        private Process livestreamer = null;
        private ObservableCollection<string> youtubeAvailableVideoQualities = new ObservableCollection<string>();
        private string youtubeSelectedQuality;
        private string youtubeVideoUrl;
        private string youtubeVideoFileName;
        #region Properties
        public string YoutubeSelectedQuality
        {
            get
            {
                return youtubeSelectedQuality;
            }

            set
            {
                this.youtubeSelectedQuality = value;
                OnPropertyChanged();
            }
        }

        public string YoutubeVideoUrl
        {
            get
            {
                return youtubeVideoUrl;
            }

            set
            {
                this.youtubeVideoUrl = value;
            }
        }

        public string YoutubeVideoFileName
        {
            get
            {
                return youtubeVideoFileName;
            }

            set
            {
                this.youtubeVideoFileName = value;
            }
        }

        public ObservableCollection<string> YoutubeAvailableVideoQualities
        {
            get
            {
                return youtubeAvailableVideoQualities;
            }

            set
            {
                this.youtubeAvailableVideoQualities = value;
                this.YoutubeSelectedQuality = this.youtubeAvailableVideoQualities[0];
                OnPropertyChanged();
            }
        }
        #endregion


        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler DownloadFinished;

        private void OnDownloadFinished()
        {
            if (this.DownloadFinished != null)
                this.DownloadFinished(this, EventArgs.Empty);
        }
        #endregion

        public YoutubeModel()
        {
            livestreamer = new Process();
            livestreamer.StartInfo.FileName = "livestreamer";
            livestreamer.StartInfo.CreateNoWindow = true;
            livestreamer.StartInfo.UseShellExecute = true;
            livestreamer.StartInfo.RedirectStandardOutput = true;
        }
        #endregion                             

        public void GetVideoQualityOptions()
        {
            livestreamer.StartInfo.Arguments = YoutubeVideoUrl;
            livestreamer.StartInfo.UseShellExecute = false;
            livestreamer.Start();
            string output = livestreamer.StandardOutput.ReadToEnd();
            if (output.Contains("streams:"))
            {
                string pattern = @"\d+p";
                Regex rgx = new Regex(pattern);
                MatchCollection matches = rgx.Matches(output);
                List<string> ls = new List<string>();
                foreach (Match m in matches)
                    ls.Add(m.Value);
                YoutubeAvailableVideoQualities = new ObservableCollection<string>(ls);
            }            
        }

        public void DownloadVideo()
        {
            livestreamer.StartInfo.Arguments = string.Format("-o {0} {1} {2}", YoutubeVideoFileName, YoutubeVideoUrl, YoutubeSelectedQuality);
            livestreamer.Start();
            livestreamer.WaitForExit();
            OnDownloadFinished();
        }
    }
}
