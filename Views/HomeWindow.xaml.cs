using lib_iis.Core;
using lib_iis.Core.MVVM;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.Views
{
    public partial class HomeWindow : Window
    {
        internal HomeWindow(BaseViewModel context)
        {
            InitializeComponent();
            DataContext = context;
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IControl vm)
            {
                vm.Close += (context, name) =>
                {
                    switch (name) {
                        case nameof(AddNewBookWindow):
                        {
                            AddNewBookWindow view = new AddNewBookWindow(context);
                            view.Show();
                            break;
                        }
                        case nameof(CreateCustomerWindow):
                        {
                            CreateCustomerWindow view = new CreateCustomerWindow(context);
                            view.Show();
                            break;
                        }
                        case nameof(UpdateCustomerInfoWindow):
                        {
                            UpdateCustomerInfoWindow view = new UpdateCustomerInfoWindow(context);
                            view.Show();
                            break;
                        }
                        case nameof(CustomerDeleteionWindow):
                        {
                            CustomerDeleteionWindow view = new CustomerDeleteionWindow(context);
                            view.Show();
                            break;
                        }
                        case nameof(UpdateBookInfoWindow):
                        {
                            UpdateBookInfoWindow view = new UpdateBookInfoWindow(context);
                            view.Show();
                            break;
                        }
                        case nameof(CheckWindow):
                        {
                             CheckWindow view = new CheckWindow(context);
                             view.Show();
                             break;  
                        }
                        case nameof(CheckOut):
                        {
                            CheckOut view = new CheckOut(context);
                            view.Show();
                            break;
                        }
                        case nameof(BookHistoryWindow):
                        {
                            BookHistoryWindow view = new BookHistoryWindow(context);
                            view.Show();
                            break;
                        }
                        case "Exit":
                        {
                            break;
                        }

                    }
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
