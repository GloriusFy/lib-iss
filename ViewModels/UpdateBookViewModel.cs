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
    internal class UpdateBookViewModel : BaseViewModel
    {
        #region Static Members
        private static readonly string BookUpdateFormatError = "Please enter the book details in a valid format.";
        private static readonly string BookUpdationError = "Could not update book details.";
        private static readonly string BookUpdationSuccess = "Successfully updated book details.";
        #endregion

        #region Fields
        private HttpResponseMessage _response;
        #endregion

        #region Dependencies 
        IValidation _validation;
        IHttpService _httpService;
        #endregion

        #region Properties
        public Book BookToUpdate {get;set;}
        #endregion

        public UpdateBookViewModel(IValidation validation, IHttpService httpService, Book bookToUpdate)
        {
            BookToUpdate = bookToUpdate;
            _validation = validation;
            _httpService = httpService;
        }


        #region Commands 
        public ICommand UpdateBookCommand => new DelegateCommand(UpdateBook, 
            () => { return !(BookToUpdate == null || string.IsNullOrWhiteSpace(BookToUpdate.Title) || string.IsNullOrWhiteSpace(BookToUpdate.ISBN)); });
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        #region Methods
        public async Task UpdateBook()
        {
            if (!PerformBookUpdationValidation())
            {
                DisplayErrorMessage(BookUpdateFormatError);
                return;
            }
            var serializedData = JsonConvert.SerializeObject(BookToUpdate);
            try
            {
                _response = await _httpService.PostData("updatebook", serializedData);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    DisplayInfoMessage(BookUpdationSuccess, "Success");
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    DisplayErrorMessage(BookUpdationError);
                    break;
            }
        }

        private bool PerformBookUpdationValidation()
        {
            return _validation.ValidateBook(BookToUpdate);
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
