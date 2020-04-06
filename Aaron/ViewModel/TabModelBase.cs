using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome5;
using GalaSoft.MvvmLight;

namespace Aaron.ViewModel
{
    public abstract class TabModelBase : ViewModelBase
    {
        public abstract string TabTitle { get; }

        public virtual EFontAwesomeIcon TabIcon
        {
            get { return EFontAwesomeIcon.None; }
        }

        public virtual bool IsEnabled
        {
            get { return true; }
        }
    }
}
