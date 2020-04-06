using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Aaron.Enums;
using Aaron.ViewModel;

namespace Aaron.Dialogs
{
    /// <summary>
    /// Interaction logic for AddCarDialog.xaml
    /// </summary>
    public partial class AddCarDialog : Window
    {
        public AddCarDialog()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            ((AddCarViewModel)this.DataContext).DoWork();
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
        }

        public string CarName
        {
            get { return ((AddCarViewModel)this.DataContext).CarName; }
        }

        public CarUsageType CarType
        {
            get { return ((AddCarViewModel)this.DataContext).CarType; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
