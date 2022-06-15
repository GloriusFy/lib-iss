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
    internal class CheckOutViewModel : BaseViewModel
    {
        #region Static Members
         private static readonly string BookCheckOutSuccess = "Successfully checked out book.";
        private static readonly string BookCheckOutConditionError = "Customer ID should be a number greater than 0 OR all copies of this book have been checked out.";
        private static readonly string BookCheckOutError = "Could not check out book. Please enter a valid Customer ID OR the customer currently has this book checked out.";
        #endregion

        #region Fields
        private HttpResponseMessage _response;
        #endregion

        #region Dependencies
        private IValidation _validation;
        private IHttpService _httpService;
        #endregion

        #region Properties
        public Book SelectedBook { get; set; }
        public uint SuppliedCustomerId { get; set; }
        #endregion

        public CheckOutViewModel(IValidation validation, IHttpService httpService, Book selectedBook)
        {
            SelectedBook = selectedBook;
            _validation = validation;
            _httpService = httpService;
        }

        #region Commands
        public ICommand CheckOutCommand => new DelegateCommand(CheckOutBook);
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        #region Methods
        public async void CheckOutBook()
        {
            if (!PerformCheckOutValidation())
            {
                DisplayErrorMessage(BookCheckOutConditionError);
                return;
            }
            try
            {
                _response = await _httpService.GetData("checkout", $"customer={SuppliedCustomerId}&book={SelectedBook.BookId}");
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    SuppliedCustomerId = 0;
                    RaisePropertyChanged(nameof(SuppliedCustomerId));
                    DisplayInfoMessage(BookCheckOutSuccess, "Success!");
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    DisplayErrorMessage(BookCheckOutError);
                    break;
            }
        }
        private bool PerformCheckOutValidation()
        {
            return _validation.BookValidForCheckOut(SelectedBook) && SuppliedCustomerId > 0;
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
