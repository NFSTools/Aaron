using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Messages;
using Aaron.Services;
using Aaron.Utils;
using FontAwesome5;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aaron.ViewModel.Tabs
{
    public class CarListViewModel : TabModelBase
    {
        private readonly IProjectService _projectService;
        private readonly ICarService _carService;
        private AaronCarRecord _selectedCarRecord;

        public override string TabTitle
        {
            get { return "Cars"; }
        }

        public override EFontAwesomeIcon TabIcon
        {
            get { return EFontAwesomeIcon.Solid_Car; }
        }

        public override bool IsEnabled
        {
            get { return _projectService.GetCurrentProject() != null; }
        }

        public SynchronizedObservableCollection<AaronCarRecord> Cars
        {
            get { return _carService.GetCars(); }
        }

        public AaronCarRecord SelectedCarRecord
        {
            get { return _selectedCarRecord; }
            set
            {
                _selectedCarRecord = value;
                RaisePropertyChanged();
            }
        }

        public CarListViewModel(IProjectService projectService, ICarService carService)
        {
            this._projectService = projectService;
            this._carService = carService;
            this.MessengerInstance.Register<ProjectMessage>(this, HandleProjectMessage);
        }

        private void HandleProjectMessage(ProjectMessage obj)
        {
            RaisePropertyChanged("IsEnabled");
        }
    }
}
