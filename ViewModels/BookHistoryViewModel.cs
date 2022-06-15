using lib_iis.Core;
using lib_iis.Core.HttpService;
using lib_iis.Core.MVVM;
using lib_iis.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace lib_iis.ViewModels
{
    internal class BookHistoryViewModel : BaseViewModel
    {

        #region Fields
        private HttpResponseMessage _response;
        private List<BookTransaction> _bookTransactions;
        #endregion

        #region Dependencies
        private IValidation _validation;
        private IHttpService _httpService;
        #endregion

        #region Properties
        public List<BookTransaction> BookHistory
        {
            get => _bookTransactions ??= new List<BookTransaction>();
            set => SetProperty(ref _bookTransactions, value);
        }
        public Book SelectedBook { get; set; }
        #endregion

        public BookHistoryViewModel(IValidation validation, IHttpService httpService, Book selectedBook)
        {
            SelectedBook = selectedBook;
            _validation = validation;
            _httpService = httpService;
            GetBookHistory();
        }

        #region Commands 
        public ICommand BookHistoryCommand => new DelegateCommand(GetBookHistory);
        public ICommand CancelCommand => new DelegateCommand(() => Close?.Invoke(new HomeViewModel(_validation, _httpService), null));
        #endregion

        #region Methods
        private async Task GetBookHistory()
        {
            try
            {
                _response = await _httpService.GetData("bookhistory", $"book={SelectedBook.BookId}");
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            switch (_response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var messageContent = await _response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(messageContent))
                    {
                        break;
                    }
                    else
                    {
                        BookHistory = JsonConvert.DeserializeObject<List<BookTransaction>>(messageContent);
                        RaisePropertyChanged(nameof(BookHistory));
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

        private void DisplayErrorMessage(string messageContent)
        {
            MessageBox.Show(messageContent, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion 
    }
}
