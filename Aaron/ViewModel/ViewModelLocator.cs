/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Aaron"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Aaron.ProgressDialog;
using Aaron.Services;
using Aaron.Services.Implementations;
using Aaron.ViewModel.Tabs;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Aaron.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            // Entity management
            SimpleIoc.Default.Register<ICarService, CarService>(true);
            SimpleIoc.Default.Register<ICarPartService, CarPartService>(true);
            SimpleIoc.Default.Register<IPresetCarService, PresetCarService>(true);
            SimpleIoc.Default.Register<IPresetSkinService, PresetSkinService>(true);
            SimpleIoc.Default.Register<IDataTableService, DataTableService>(true);

            // Internal state
            SimpleIoc.Default.Register<IDataService, DataService>(true);
            SimpleIoc.Default.Register<IProjectService, ProjectService>(true);

            // Helpers
            SimpleIoc.Default.Register<AlertService>(true);
            SimpleIoc.Default.Register(() => new ProgressDialogService());

            // Views
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MenuViewModel>();
            SimpleIoc.Default.Register<PartEditorViewModel>();
            SimpleIoc.Default.Register<AddCarViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();

            SimpleIoc.Default.Register<StartPageViewModel>();
            SimpleIoc.Default.Register<CarListViewModel>();
            SimpleIoc.Default.Register<PartCollectionsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        public MenuViewModel Menu
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MenuViewModel>();
            }
        }

        public PartEditorViewModel PartEditor
        {
            get { return ServiceLocator.Current.GetInstance<PartEditorViewModel>(); }
        }

        public AddCarViewModel AddCar
        {
            get { return ServiceLocator.Current.GetInstance<AddCarViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}