using Caliburn.Micro;
using Connect4Project.ViewModels;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Connect4Project
{
    public class BootStrapper : BootstrapperBase
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        SplashScreenViewModel splashScreen = new SplashScreenViewModel();

        public BootStrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Tick += OnTimedEvent;
            dispatcherTimer.Start();
            WindowManager windowManager = new WindowManager();
            windowManager.ShowWindowAsync(splashScreen);
        }

        private void OnTimedEvent(Object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            splashScreen.TryCloseAsync();
            DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
