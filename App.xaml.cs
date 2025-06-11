using APR_TEST.Utils;
using APR_TEST.ViewModels;
using APR_TEST.Views;

using System.Configuration;
using System.Data;
using System.Windows;

namespace APR_TEST
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainViewModel GlobalViewModel { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GlobalViewModel = new MainViewModel();

            var mainWindow = new MainWindow
            {
                DataContext = GlobalViewModel
            };
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalViewModel?.CleanupResources();
            ImageProcessor.Dispose();
            GlobalViewModel?.TimeEndPeriodSafe();
        }
    }

}
