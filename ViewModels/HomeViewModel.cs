using lib_iis.Core;
using lib_iis.Core.Events;
using lib_iis.Core.HttpService;
using lib_iis.Core.MVVM;
using lib_iis.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.ViewModels
{
    internal class HomeViewModel : BaseViewModel
    {
        #region Static Members
        private static readonly string BookDeletionFormatError = "The Book ID should be a number greater than 0.";
        private static readonly string BookDeletionError = "Cannot delete book. The book does not exist or has already been deleted.";
        private static readonly string BookDeletionSuccess = "Successfully deleted book";
        #endregion

        #region Fileds
        private HttpResponseMessage _response;
        private Book _selectedBook;
        private ObservableCollection<Book> _books;

        #endregion

        #region Dependencies
        private IHttpService _httpService;
        private IValidation _validation;
        #endregion

        #region Properties
        public string Query { get; set; }
        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }
        public ObservableCollection<Book> Books
        {
            get => _books ??= new ObservableCollection<Book>();
            set => SetProperty(ref _books, value);
        }
        public List<BookTransaction> BookHistory { get; set; }
        #endregion


        public HomeViewModel(IValidation validation,IHttpService httpService)
        {
            _validation = validation;
            _httpService = httpService;
            QueryReq();
        }

        #region Commands 
        public ICommand SearchCommand => new DelegateCommand(QueryReq);
        public ICommand AddBookCommand => new DelegateCommand(() => Close?.Invoke(new AddBookViewModel(_validation, _httpService), "AddNewBookWindow"));
        public ICommand AddCustomerCommand => new DelegateCommand(() => Close?.Invoke(new AddCustomerViewModel(_validation, _httpService), "CreateCustomerWindow"));
        public ICommand UpdateCustomerCommand => new DelegateCommand(() => Close?.Invoke(new UpdateCustomerViewModel(_validation, _httpService), "UpdateCustomerInfoWindow"));
        public ICommand DeleteCustomerCommand => new DelegateCommand(() => Close?.Invoke(new DeleteCustomerViewModel(_validation, _httpService), "CustomerDeleteionWindow"));
        public ICommand OpenUpdateCommand => new DelegateCommand(() => Close?.Invoke(new UpdateBookViewModel(_validation, _httpService, SelectedBook), "UpdateBookInfoWindow"));
        public ICommand EnableCheckOutCommand => new DelegateCommand(() => Close?.Invoke(new CheckOutViewModel(_validation, _httpService, SelectedBook), "CheckOut"),
            () => { return (SelectedBook != null && SelectedBook.AvailableCopies > 0); });
        public ICommand EnableCheckInCommand => new DelegateCommand(() => Close?.Invoke(new CheckInViewModel(_validation, _httpService, SelectedBook), "CheckWindow" ),
            () => { return (SelectedBook != null && (SelectedBook.AvailableCopies <= SelectedBook.TotalCopies)); });
        public ICommand ViewBookHistoryCommand => new DelegateCommand(() => Close?.Invoke(new BookHistoryViewModel(_validation, _httpService, SelectedBook), "BookHistoryWindow"));
        public ICommand DeleteBookCommand => new DelegateCommand(DeleteBook);
        #endregion


        #region Methods 
        private async Task QueryReq()
        {
            try
            {
                _response = await _httpService.GetData("search", $"search={Query}");
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch(_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(messageContent))
                    {
                        Books = JsonConvert.DeserializeObject<ObservableCollection<Book>>(messageContent);
                    }
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    MessageBox.Show("External Server error");
                    break;

            }
                      
            
        }
        public async void DeleteBook()
        {
            if (!PerformBookDeletionValidation())
            {
                DisplayErrorMessage(BookDeletionFormatError);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this book pemanently?", "Confirmation", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;
            try
            {
                _response = await _httpService.DeleteData("deletebook", $"book={SelectedBook.BookId}");
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    await QueryReq();
                    DisplayInfoMessage(BookDeletionSuccess, "Success!");
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    DisplayErrorMessage(BookDeletionError);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    DisplayErrorMessage(_invalidSession);
                    break;
                default:
                    MessageBox.Show("External Server error");
                    break;
            }
        }
        private bool PerformBookDeletionValidation()
        {
            return SelectedBook != null && SelectedBook.BookId > 0;
        }

        private bool PerformBookUpdationValidation()
        {
            return _validation.ValidateBook(SelectedBook);
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
