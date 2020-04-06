using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Utils;
using FontAwesome5;

namespace Aaron.ViewModel.Tabs
{
    public class StartPageViewModel : TabModelBase
    {
        public override string TabTitle
        {
            get { return "Start Page"; }
        }

        public override EFontAwesomeIcon TabIcon
        {
            get { return EFontAwesomeIcon.Solid_Globe; }
        }

        public string Version
        {
            get { return $"v{VersionReader.AppVersion}"; }
        }
    }
}
