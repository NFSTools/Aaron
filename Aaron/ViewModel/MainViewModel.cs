using System.Collections.ObjectModel;
using System.Windows.Forms;
using Aaron.Data;
using Aaron.DataIO;
using Aaron.Messages;
using Aaron.Services;
using Aaron.Utils;
using Aaron.ViewModel.Tabs;
using FontAwesome5.WPF;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Aaron.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _windowTitle;

        private readonly IProjectService _projectService;

        /// <summary>
        /// The title of the window.
        /// </summary>
        public string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// The current project.
        /// </summary>
        public AaronProject CurrentProject
        {
            get { return _projectService.GetCurrentProject(); }
            set => RaisePropertyChanged();
        }

        /// <summary>
        /// The currently opened tabs.
        /// </summary>
        public SynchronizedObservableCollection<TabModelBase> OpenTabs { get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IProjectService projectService)
        {
            _projectService = projectService;

            OpenTabs = new SynchronizedObservableCollection<TabModelBase>();
            WindowTitle = "Aaron";

            InitMessageHandlers();
            InitTabs();
        }

        private void InitTabs()
        {
            OpenTabs.Add(TabUtil.GetTab<StartPageViewModel>());
            OpenTabs.Add(TabUtil.GetTab<CarListViewModel>());
            OpenTabs.Add(TabUtil.GetTab<PartCollectionsViewModel>());
        }

        private void InitMessageHandlers()
        {
            Messenger.Default.Register<ProjectMessage>(this, this.HandleProjectMessage);
        }

        private void HandleProjectMessage(ProjectMessage obj)
        {
            if (obj.Project != null)
            {
                this.CurrentProject = obj.Project;
                WindowTitle = $"Aaron | {this.CurrentProject.Path}";
            }
            else
            {
                WindowTitle = "Aaron";
            }
        }
    }
}