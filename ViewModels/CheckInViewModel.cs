using lib_iis.Core;
using lib_iis.Core.HttpService;
using lib_iis.Core.MVVM;
using lib_iis.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.ViewModels
{
    internal class CheckInViewModel : BaseViewModel
    {
        #region Static Members
        private static readonly string BookCheckInSuccess = "Successfully checked in book.";
        private static readonly string BookCheckInConditionError = "Customer ID should be a number greater than 0 OR all copies of this book have already been checked in.";
        private static readonly string BookCheckInError = "Could not check in book. Please enter a valid Customer ID OR the customer has not currently checked out this book.";
        #endregion

        #region Fields
        private HttpResponseMessage _response;
        #endregion

        #region Dependencies
        private IValidation _validation;
        private IHttpService _httpServce;
        #endregion

        #region Properties
        public Book SelectedBook { get; set; }
        public uint SuppliedCustomerId { get; set; }
        #endregion

        public CheckInViewModel(IValidation validation, IHttpService httpService, Book selectedBook)
        {
            SelectedBook = selectedBook;
            _validation = validation;
            _httpServce = httpService; 
        }

        #region Command 
        public ICommand CheckInCommand => new DelegateCommand(CheckInBook);
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpServce), null));
        #endregion

        #region Methods
        public async void CheckInBook()
        {
            if (!PerformCheckInValidation())
            {
                DisplayErrorMessage(BookCheckInConditionError);
                return;
            }
            try
            {
                _response = await _httpServce.GetData("checkin", $"customer={SuppliedCustomerId}&book{SelectedBook.BookId}");
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    DisplayInfoMessage(BookCheckInSuccess, "Success!");
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    SuppliedCustomerId = 0;
                    RaisePropertyChanged(nameof(SuppliedCustomerId));
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    DisplayErrorMessage(BookCheckInError);
                    break;
            }
        }

        private bool PerformCheckInValidation()
        {
            return _validation.BookValidForCheckIn(SelectedBook) && SuppliedCustomerId > 0;
        }

        private void DisplayErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void DisplayInfoMessage(string errorMessage, string caption)
        {
            MessageBox.Show(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}
