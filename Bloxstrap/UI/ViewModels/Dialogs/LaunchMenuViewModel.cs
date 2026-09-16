using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using Bloxstrap.UI.Elements.About;

namespace Bloxstrap.UI.ViewModels.Installer
{
    public class LaunchMenuViewModel
    {
        public string Version => string.Format(Strings.Menu_About_Version, App.Version);

        public Visibility RobloxStudioOptionVisibility => App.IsStudioVisible ? Visibility.Visible : Visibility.Collapsed;

        public ICommand LaunchSettingsCommand => new RelayCommand(LaunchSettings);
        public ICommand LaunchRobloxCommand => new RelayCommand(LaunchRoblox);
        public ICommand LaunchMultiRobloxCommand => new RelayCommand(LaunchMultiRoblox);
        public ICommand LaunchRobloxStudioCommand => new RelayCommand(LaunchRobloxStudio);
        public ICommand LaunchAboutCommand => new RelayCommand(LaunchAbout);

        public event EventHandler<NextAction>? CloseWindowRequest;

        private void LaunchSettings() => CloseWindowRequest?.Invoke(this, NextAction.LaunchSettings);

        private void LaunchRoblox() => CloseWindowRequest?.Invoke(this, NextAction.LaunchRoblox);

        private void LaunchMultiRoblox()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Paths.Application,
                    Arguments = "-player -multi 2 -performance",
                    WorkingDirectory = Paths.Base,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("LaunchMenuViewModel::LaunchMultiRoblox", ex);
                Frontend.ShowMessageBox(ex.Message, MessageBoxImage.Error);
            }

            CloseWindowRequest?.Invoke(this, NextAction.Terminate);
        }

        private void LaunchRobloxStudio() => CloseWindowRequest?.Invoke(this, NextAction.LaunchRobloxStudio);

        private void LaunchAbout() => new MainWindow().ShowDialog();
    }
}
