using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using GalaSoft.MvvmLight;

namespace Aaron.ViewModel
{
    public class PartEditorViewModel : ViewModelBase
    {
        private AaronCarPartRecord _carPartRecord;
        private string _windowTitle;

        public AaronCarPartRecord CarPartRecord
        {
            get => _carPartRecord;
            set
            {
                _carPartRecord = value;
                RaisePropertyChanged();

                WindowTitle = $"Editing part: {value.Name}";
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                RaisePropertyChanged();
            }
        }
    }
}
