using lib_iis.Core.HttpService;
using lib_iis.ViewModels;
using lib_iis.Core.MVVM;
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
using lib_iis.Core;

namespace lib_iis.Views
{
    /// <summary>
    /// Логика взаимодействия для AddNewBookWindow.xaml
    /// </summary>
    public partial class AddNewBookWindow : Window
    {
        internal AddNewBookWindow(BaseViewModel context)
        {
            InitializeComponent();
            DataContext = context;
            Loaded += Window_Loaded;
            

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (DataContext is IControl vm)
            {
                vm.Close += (context, _) =>
                {   HomeWindow view = new HomeWindow(context);
                    view.Show();
                    Close();
                };
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
