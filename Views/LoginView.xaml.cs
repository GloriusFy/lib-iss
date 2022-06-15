using lib_iis.Core.Events;
using lib_iis.Core.HttpService;
using lib_iis.ViewModels;
using System.Windows;

namespace lib_iis.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(HttpService.HttpServiceInstance);
            (DataContext as LoginViewModel).OnSuccessfulConnect += ChangeWindow;
        }

        private void ChangeWindow(object sender, SessionEventArgs e)
        {
            HomeWindow homeView = new HomeWindow(e.HoveViewModelContext);
            homeView.Show();
            (DataContext as LoginViewModel).OnSuccessfulConnect -= ChangeWindow;
            Close();
        }
    }
}
