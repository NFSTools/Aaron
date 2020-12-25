using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aaron.DataIO;
using Aaron.Dialogs;
using Aaron.Messages;
using Aaron.ProgressDialog;
using Aaron.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Aaron.ViewModel
{
    /// <summary>
    /// View model for the main menu.
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IProjectService _projectService;
        private readonly ProgressDialogService _progressDialogService;

        private bool _canOpen;
        private readonly ProgressDialogOptions _progressOptionsGlobalC = new ProgressDialogOptions
        { Label = "Loading GlobalC.lzc...", WindowTitle = "Aaron - Loading..." };
        private readonly ProgressDialogOptions _progressOptionsGeneratingProject = new ProgressDialogOptions
        { Label = "Generating project...", WindowTitle = "Aaron - Loading..." };
        private readonly ProgressDialogOptions _progressOptionsLoadingProject = new ProgressDialogOptions
        { Label = "Loading project...", WindowTitle = "Aaron - Loading..." };
        private readonly ProgressDialogOptions _progressOptionsSavingGlobalC = new ProgressDialogOptions
        { Label = "Saving GlobalC.lzc...", WindowTitle = "Aaron - Loading..." };
        private ProgressDialogOptions _progressOptionsSavingProject = new ProgressDialogOptions
        { Label = "Saving project...", WindowTitle = "Aaron - Loading..." };

        private bool _canClose;

        /// <summary>
        /// The command used for the "Open" menu option.
        /// </summary>
        public RelayCommand OpenCommand { get; }

        /// <summary>
        /// The command used for the "Open Project" menu option.
        /// </summary>
        public RelayCommand OpenProjectCommand { get; }

        /// <summary>
        /// The command used for the "Close" menu option.
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        /// <summary>
        /// The command used for the "About" menu option.
        /// </summary>
        public RelayCommand AboutCommand { get; set; }

        /// <summary>
        /// Whether or not the user can open a project or file.
        /// </summary>
        public bool CanOpen
        {
            get { return _canOpen; }
            set
            {
                _canOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool CanDoProjectActions
        {
            get { return _projectService.GetCurrentProject() != null; }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand SaveRawCommand { get; }
        public RelayCommand SaveProjectCommand { get; }
        public RelayCommand AddCarCommand { get; }

        public MenuViewModel(IDataService dataService, IProjectService projectService,
            ProgressDialogService progressDialogService)
        {
            _dataService = dataService;
            _projectService = projectService;
            _progressDialogService = progressDialogService;

            OpenCommand = new RelayCommand(DoOpenCommand);
            OpenProjectCommand = new RelayCommand(DoOpenProjectCommand);
            SaveCommand = new RelayCommand(DoSaveCommand);
            SaveRawCommand = new RelayCommand(DoSaveRawCommand);
            SaveProjectCommand = new RelayCommand(DoSaveProjectCommand);
            AddCarCommand = new RelayCommand(DoAddCarCommand);
            AboutCommand = new RelayCommand(DoAboutCommand);

            CanOpen = true;

            this.MessengerInstance.Register<ProjectMessage>(this, HandleProjectMessage);
        }

        private void DoAboutCommand()
        {
            new AboutWindow().ShowDialog();
        }

        private void DoAddCarCommand()
        {
            var dialog = new AddCarDialog();

            if (dialog.ShowDialog() == true)
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Information, "Added car!"));
            }
        }

        private void DoSaveProjectCommand()
        {
            if (_projectService.HasUnsavedChanges())
            {
                _projectService.SaveProject();
            }
        }

        private void DoSaveCommand()
        {
            if (_progressDialogService.TryExecute(RunSaveProcess(true), 
                _progressOptionsSavingGlobalC, out _))
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Information, "Saved!"));
            }
        }

        private void DoSaveRawCommand()
        {
            if (_progressDialogService.TryExecute(RunSaveProcess(false), 
                _progressOptionsSavingGlobalC, out _))
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Information, "Saved!"));
            }
        }

        private Func<IProgress<string>, bool> RunSaveProcess(bool compress)
        {
            return (prog) =>
            {
                using (var ccw = new CarControllerWriter(_projectService.GetCurrentProject().OutputFilePath, compress))
                {
                    ccw.DoWrite(prog);
                    return true;
                }
            };
        }

        private void HandleProjectMessage(ProjectMessage obj)
        {
            RaisePropertyChanged("CanDoProjectActions");
        }

        private void DoOpenProjectCommand()
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Select project";
            ofd.Filter = "Aaron Project Files (*.aproj)|*.aproj";

            if (ofd.ShowDialog() == true)
            {
                CanOpen = false;
                Messenger.Default.Send(LoadProject(ofd.FileName)
                    ? new AlertMessage(AlertType.Information, "Loaded project successfully!")
                    : new AlertMessage(AlertType.Error, "Failed to load project."));

                CanOpen = true;
            }
        }

        private void DoOpenCommand()
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Select GlobalC.lzc";
            ofd.Filter = "LZC Files (*.lzc)|*.lzc";

            if (ofd.ShowDialog() == true)
            {
                CanOpen = false;
                _projectService.CloseProject();

                _progressDialogService.Execute(prog => { _dataService.LoadCarController(ofd.FileName, prog); },
                    _progressOptionsGlobalC);

                //_dataService.LoadCarController(ofd.FileName);

                var fbd = new FolderBrowserDialog();
                fbd.Description = "Export project to...";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (_progressDialogService.TryExecute(
                        () => _projectService.GenerateProject(fbd.SelectedPath),
                        _progressOptionsGeneratingProject,
                        out var projectPath
                    ))
                    {
                        LoadProject(projectPath);

                        Messenger.Default.Send(new AlertMessage(AlertType.Information, "Loaded generated project successfully!"));
                    }
                    else
                    {
                        Messenger.Default.Send(new AlertMessage(AlertType.Error, "Failed to generate project!"));
                    }
                }
                else
                {
                    _projectService.CloseProject();
                }

                CanOpen = true;
            }
        }

        private bool LoadProject(string filename)
        {
            CanOpen = false;
            _projectService.CloseProject();

            return _progressDialogService.TryExecute(() => LoadProjectInternal(filename), _progressOptionsLoadingProject,
                out var result) && result;
        }

        private bool LoadProjectInternal(string filename)
        {
#if !DEBUG
            try
            {
#endif
            _projectService.LoadProject(filename);

            return true;
#if !DEBUG

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
#endif
        }
    }
}
