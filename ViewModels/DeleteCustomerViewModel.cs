using lib_iis.Core;
using lib_iis.Core.HttpService;
using lib_iis.Core.MVVM;
using lib_iis.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.ViewModels
{
    internal class DeleteCustomerViewModel : BaseViewModel
    {
        #region Static Members
        private static readonly string FormatError = "Please enter a number greater than 0.";
        private static readonly string DeletionError = "Could not delete customer account. Please enter a valid ID OR the customer account has already been deleted.";
        #endregion

        #region Fields 
        private HttpResponseMessage _response;
        private Customer _selectedCustomer;
        #endregion


        #region Dependecies 
        private readonly IHttpService _httpService;
        private readonly IValidation _validation;
        #endregion

        #region Properties
        public Customer SelectedCustomer
        {
            get => _selectedCustomer ??= new Customer();
            set => SetProperty(ref _selectedCustomer, value);
        }
        #endregion

        public DeleteCustomerViewModel(IValidation validation,IHttpService httpService)
        {
            _httpService = httpService;
            _validation = validation;
        }

        #region Commands
        public ICommand DeleteCustomerAccountCommand => new DelegateCommand(DeleteCustomerAccount, ()=> { return SelectedCustomer.CustomerId > 0; });
        public ICommand CancelCommand => new DelegateCommand(()=> Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        #region Methods
        private async Task DeleteCustomerAccount()
        {
            if(SelectedCustomer.CustomerId <= 0)
            {
                DisplayErrorMessage(FormatError);
                return;
            }
            try
            {
                _response = await _httpService.DeleteData("deletecustomer", $"customer={SelectedCustomer.CustomerId}");
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    RaisePropertyChanged(nameof(SelectedCustomer));
                    MessageBox.Show("Successfully deleted customer account", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    MessageBox.Show("External Server Error");
                    break;
            }
            
        }

        private void DisplayErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion
    }
}
