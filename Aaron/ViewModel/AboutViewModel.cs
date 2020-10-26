using System.Diagnostics;
using GalaSoft.MvvmLight;
using System.Reflection;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aaron.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenDiscordInviteCommand
        {
            get
            {
                return new RelayCommand(() => Process.Start("https://discord.gg/8tP4kVJ"));
            }
        }

        public RelayCommand OpenGitHubCommand
        {
            get
            {
                return new RelayCommand(() => Process.Start("https://github.com/NFSTools/AaronLegacy"));
            }
        }

        public AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}