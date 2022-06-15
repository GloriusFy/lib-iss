using lib_iis.Core;
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
    internal class AddCustomerViewModel : BaseViewModel
    {
        #region Static members
        private static readonly string FormatError = "Please enter all the customer details in a valid format.";
        private static readonly string InsertionError = "Cannot create customer account.";
        private static readonly string InsertionSuccess = "Successfully created customer account!\nCustomer ID : {0}";
        #endregion

        #region Fields
        private Customer _customer;
        private HttpResponseMessage _response;
        #endregion 

        #region Dependencies
        private readonly IValidation _validation;
        private readonly IHttpService _httpService;
        #endregion

        #region Properties
        public Customer NewCustomer
        {
            get => _customer ??= new Customer();
            set => SetProperty(ref _customer, value);
        }
        #endregion

        #region Commands 
        public ICommand CreateCustomerCommand => new DelegateCommand(CreateCustomer, CanCreateCustomer);
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        public AddCustomerViewModel(IValidation validation, IHttpService httpService)
        {
            _validation = validation;
            _httpService = httpService;
            NewCustomer.DateOfBirth = DateTime.UtcNow;   
        }


        #region Methods 
        private async Task CreateCustomer()
        {
            if(!PerformValidation())
            {
                DisplayErrorMessage(FormatError);
                return;
            }
            NewCustomer.DateOfBirth = TimeZoneInfo.ConvertTimeToUtc(NewCustomer.DateOfBirth);
            NewCustomer.AccountCreatedOn = TimeZoneInfo.ConvertTimeToUtc(NewCustomer.DateOfBirth);
            try
            {
                var serializedData = JsonConvert.SerializeObject(NewCustomer);
                _response = await _httpService.PostData("addcustomer", serializedData);
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(messageContent) || 0 >= Convert.ToInt32(messageContent))
                    {
                        DisplayErrorMessage(InsertionError);
                        break;
                    }
                    else
                    {
                        NewCustomer.ClearCustomerDetails();
                        RaisePropertyChanged(nameof(NewCustomer));
                        MessageBox.Show(InsertionSuccess);
                        break;
                    }
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;

                default:
                    DisplayErrorMessage(InsertionError);
                    break;
            }
           
        }
        private bool CanCreateCustomer()
        {
            return !(string.IsNullOrWhiteSpace(NewCustomer.FirstName) || string.IsNullOrWhiteSpace(NewCustomer.Email) ||
                 NewCustomer.DateOfBirth == DateTime.MinValue || string.IsNullOrWhiteSpace(NewCustomer.LastName));
        }
        private bool PerformValidation()
        {
            return _validation.ValidateCustomer(NewCustomer);
        }
        private void DisplayErrorMessage(string messageContent)
        {
            MessageBox.Show(messageContent, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion
    }
}
