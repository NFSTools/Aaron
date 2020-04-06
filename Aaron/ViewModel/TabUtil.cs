using GalaSoft.MvvmLight.Ioc;

namespace Aaron.ViewModel
{
    internal static class TabUtil
    {
        public static T GetTab<T>() where T : TabModelBase
        {
            return SimpleIoc.Default.GetInstance<T>();
        }
    }
}