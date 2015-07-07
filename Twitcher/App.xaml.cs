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

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, EventArgs e)
        {
            _model = new TwitcherModel();
            _viewModel = new TwitcherViewModel(_model);

            _view = new MainWindow();
            _view.DataContext = _viewModel;

            _view.Show();

        }
    }
}
