using lib_iis.Core.HttpService;
using lib_iis.Core.MVVM;
using lib_iis.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.ViewModels
{
    internal class UpdateCustomerViewModel : BaseViewModel
    {
        #region Static Members
        private static readonly string FormatError = "Please enter a number greater than 0.";
        private static readonly string FetchError = "Could not find customer. Please enter a valid Customer ID OR the customer account has already been deleted.";
        private static readonly string UpdationFormatError = "Please enter all the customer details in a valid format.";
        private static readonly string UpdationError = "Could not update customer details.";
        #endregion

        #region Fields
        private Customer _fetchedCustomer;
        private HttpResponseMessage _response;
        private Visibility _updationVisibility;
        #endregion

        #region Dependencies
        private readonly IValidation _validation;
        private readonly IHttpService _httpService;
        #endregion

        #region Properties
        public Customer FetchedCustomer
        {
            get => _fetchedCustomer ??= new Customer();
            set => SetProperty(ref _fetchedCustomer, value);
        }
        public Visibility UpdationVisibility
        {
            get => _updationVisibility;
            set => SetProperty(ref _updationVisibility, value);
        }
        public int SuppliedCustomerID { get; set; }
        #endregion

        public UpdateCustomerViewModel(IValidation validate, IHttpService httpSerivce)
        {
            _httpService = httpSerivce;
            _validation = validate;
            UpdationVisibility = Visibility.Hidden;
            UpdationVisibility = Visibility.Hidden;
        }

        #region Commands
        public ICommand FetchCustomerDetailsCommand => new DelegateCommand(FetchCustomerDetails);
        public ICommand UpdateCustomerCommand => new DelegateCommand(UpdateCustomerAccount, CanUpdateCustomerAccount);
        public ICommand CancelCOmmand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        #region Methods
        private async Task UpdateCustomerAccount()
        {
            if (PerformUpdateValidation())
            {
                DisplayErrorMessage(UpdationFormatError);
                return;
            }
            try
            {
                _response = await _httpService.GetData("updatecustomer", $"customer={FetchedCustomer.CustomerId}");
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    FetchedCustomer.CustomerId = 0;
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    DisplayErrorMessage(UpdationError);
                    break;
            }
        }
        public async Task FetchCustomerDetails()
        {
            if (!PerfomFetchValidation())
            {
                DisplayErrorMessage(FormatError);
                return;
            }
            try
            {
                _response = await _httpService.PostData("fetchcustomer", $"customer ={SuppliedCustomerID}");
            }
            catch (HttpRequestException ex) 
            {
                Debug.WriteLine(ex.Message);
            }
            switch(_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    FetchedCustomer = JsonConvert.DeserializeObject<Customer>(messageContent);
                    RaisePropertyChanged(nameof(FetchedCustomer));
                    UpdationVisibility = Visibility.Visible;
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    DisplayErrorMessage(FetchError);
                    break;
            }
            
        }

        private bool PerfomFetchValidation()
        {
            return SuppliedCustomerID > 0;
        }
        private bool PerformUpdateValidation()
        {
            return _validation.ValidateCustomer(FetchedCustomer);
        }
        private bool CanUpdateCustomerAccount()
        {
            return !(string.IsNullOrWhiteSpace(FetchedCustomer.FirstName) ||
                string.IsNullOrWhiteSpace(FetchedCustomer.Email) || FetchedCustomer.DateOfBirth == DateTime.MinValue);
        }
        private void DisplayErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion
    }
}
