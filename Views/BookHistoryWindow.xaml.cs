using lib_iis.Core;
using lib_iis.Core.MVVM;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.Views
{
    /// <summary>
    /// Логика взаимодействия для BookHistoryWindow.xaml
    /// </summary>
    public partial class BookHistoryWindow : Window
    {
        public BookHistoryWindow(BaseViewModel context)
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
                {
                    HomeWindow view = new HomeWindow(context);
                    view.Show();
                    Close();
                };
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
