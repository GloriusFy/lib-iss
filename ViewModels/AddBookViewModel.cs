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
    internal class AddBookViewModel : BaseViewModel
    {
        private static readonly string FormatError = "Please enter the book details in a valid format.";
        private static readonly string InsertionError = "Cannot insert book. " +
                                                        "The book already exists in the system or " +
                                                        "has been deleted permanently from the system preventing reinsertion.";


        #region Fields
        private Book _newBook;
        private IHttpService _httpService;
        private HttpResponseMessage _response;
        #endregion

        #region Properties
        public Book NewBook 
        { 
            get => _newBook ??= new Book();
            set => _newBook = value; 
        }
        #endregion

        #region Dependincies
        private readonly IValidation _validation;
        #endregion

        #region Commands
        public ICommand AddBookCommand => new DelegateCommand(CreateBook);
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation ,_httpService), null));

        
        #endregion


        public AddBookViewModel(IValidation validation, IHttpService httpService)
        {
            _httpService = httpService;
            _validation = validation;
        }

        #region Methods

        
        private async Task CreateBook()
        {
            NewBook.DateAdded = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            NewBook.AvailableCopies = NewBook.TotalCopies;

            if (!_validation.ValidateBook(NewBook))
            {
                ThrowErrorMessage(FormatError);
                return;
            }
            var serializedData = JsonConvert.SerializeObject(NewBook);
            try
            {
                _response = await _httpService.PostData("addbook", serializedData);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                    case System.Net.HttpStatusCode.OK:
                        NewBook.ClearBookDetails();
                        RaisePropertyChanged(nameof(NewBook));
                        MessageBox.Show("Book was successfully added!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        ThrowErrorMessage(_invalidSession);
                        break;
                    default:
                        ThrowErrorMessage(InsertionError);
                        break;
            }
            
        }

        private void ThrowErrorMessage(string msg)
        {
          MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}
