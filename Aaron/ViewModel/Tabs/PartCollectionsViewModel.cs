using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Messages;
using Aaron.ProgressDialog;
using Aaron.Services;
using Aaron.Utils;
using FontAwesome5;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aaron.ViewModel.Tabs
{
    public class PartCollectionsViewModel : TabModelBase
    {
        private readonly IProjectService _projectService;
        private readonly ICarPartService _carPartService;
        private readonly ProgressDialogService _progressDialogService;
        private readonly AlertService _alertService;

        private AaronCarPartCollection _selectedCarPartCollection;

        private ProgressDialogOptions reloadingCollectionDialogOptions = new ProgressDialogOptions
        {
            WindowTitle = "Aaron - Part Manager",
            Label = "Reloading collection..."
        };

        public override string TabTitle
        {
            get { return "Part Collections"; }
        }

        public override EFontAwesomeIcon TabIcon
        {
            get { return EFontAwesomeIcon.Solid_Wrench; }
        }

        public override bool IsEnabled
        {
            get { return _projectService.GetCurrentProject() != null; }
        }

        public SynchronizedObservableCollection<AaronCarPartCollection> CarPartCollections
        {
            get { return _carPartService.GetCarPartCollections(); }
        }

        public AaronCarPartCollection SelectedCarPartCollection
        {
            get => _selectedCarPartCollection;
            set
            {
                _selectedCarPartCollection = value;
                RaisePropertyChanged();
            }
        }

        //public RelayCommand<AaronCarPartRecord> EditPartCommand { get; }

        public RelayCommand<AaronCarPartCollection> RefreshCollectionCommand { get; }

        public PartCollectionsViewModel(IProjectService projectService, ICarPartService carPartService,
            ProgressDialogService progressDialogService, AlertService alertService)
        {
            _projectService = projectService;
            _carPartService = carPartService;
            _progressDialogService = progressDialogService;
            _alertService = alertService;

            this.RefreshCollectionCommand = new RelayCommand<AaronCarPartCollection>(this.DoRefreshCollection);
            //this.EditPartCommand = new RelayCommand<AaronCarPartRecord>(this.DoEditPart);
            this.MessengerInstance.Register<ProjectMessage>(this, HandleProjectMessage);
        }

        private void DoRefreshCollection(AaronCarPartCollection obj)
        {
            var project = _projectService.GetCurrentProject();

            try
            {
                if (this._progressDialogService.TryExecute(
                    () => this._carPartService.ReplaceCarPartCollection(
                        obj.Hash,
                        Serialization.Deserialize<AaronCarPartCollection>(
                            File.ReadAllText(Path.Combine(project.CarPartsDirectory, $"{obj.Name}.json")))),
                    this.reloadingCollectionDialogOptions,
                    out var newCollection))
                {
                    this.SelectedCarPartCollection = newCollection;
                    this.MessengerInstance.Send(new AlertMessage(AlertType.Information, "Reloaded collection!"));
                }
                //this._progressDialogService.Execute(
                //    () => this._carPartService.ReplaceCarPartCollection(
                //        obj.Hash,
                //        Serialization.Deserialize<AaronCarPartCollection>(
                //            File.ReadAllText(Path.Combine(project.CarPartsDirectory, $"{obj.Name}.json")))),
                //    new ProgressDialogOptions
                //    {
                //        WindowTitle = "Aaron - Car Part Manager",
                //        Label = "Reloading collection '" + obj.Name + "'..."
                //    });
                //this.SelectedCarPartCollection = obj;
                //this.MessengerInstance.Send(new AlertMessage(AlertType.Information, "Reloaded collection!"));
            }
            catch (Exception e)
            {
                this.MessengerInstance.Send(new AlertMessage(AlertType.Error, e.Message));
            }
        }

        private void DoEditPart(AaronCarPartRecord obj)
        {
            // TODO: implement this
        }

        private void HandleProjectMessage(ProjectMessage obj)
        {
            RaisePropertyChanged("IsEnabled");
        }
    }
}
