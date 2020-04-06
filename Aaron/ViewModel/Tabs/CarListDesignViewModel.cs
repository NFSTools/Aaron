
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.ViewModel.Tabs
{
    public class CarListDesignViewModel : TabModelBase
    {
        public override string TabTitle { get; }

        public override bool IsEnabled
        {
            get { return true; }
        }

        public SynchronizedObservableCollection<AaronCarRecord> Cars
        {
            get
            {
                return new SynchronizedObservableCollection<AaronCarRecord>();
            }
        }
    }
}
