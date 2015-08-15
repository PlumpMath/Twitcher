using System;
using System.Windows;
using Twitcher.Model;
using Twitcher.ViewModel;
using Twitcher.View;

namespace Twitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TwitcherModel _model;
        private TwitcherViewModel _viewModel;
        private MainWindow _view;

        private YoutubeModel youtubeModel;
        private YoutubeViewModel youtubeViewModel;
        private YoutubeWindow youtubeWindow;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, EventArgs e)
        {
            _model = new TwitcherModel();
            _viewModel = new TwitcherViewModel(_model);

            _viewModel.ExitApplication += delegate (object twitcherViewModelSender, EventArgs twitcherViewModelEventArgs)
            {
                Shutdown();
            };
            _viewModel.YoutubeWindowCommandInvoked += delegate (object twitcherViewModelSender, EventArgs twitcherViewModelEventArgs)
            {
                _view.Hide();

                youtubeModel = new YoutubeModel();
                youtubeViewModel = new YoutubeViewModel(youtubeModel);

                youtubeWindow = new YoutubeWindow();
                youtubeWindow.DataContext = youtubeViewModel;

                youtubeViewModel.TwitcherCommandInvoked += delegate (object youtubeViewModelSender, EventArgs youtubeViewModelEventArgs)
                {
                    youtubeWindow.Close();

                    _view.Show();
                };

                youtubeViewModel.Model_DownloadFinished += delegate (object youtubeViewModelSender, EventArgs youtubeViewModelEventArgs)
                {
                    MessageBox.Show("Download finished!", "Youtube video download", MessageBoxButton.OK);
                };

                youtubeWindow.Show();
            };


            _view = new MainWindow();
            _view.DataContext = _viewModel;

            _view.Show();

        }

        private void ViewModel_ExitApplication(object sender, EventArgs e)
        {
        }
    }
}
