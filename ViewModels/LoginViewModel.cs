using lib_iis.Core.Events;
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
    internal class LoginViewModel : BaseViewModel
    {

        #region Fields
        private UserModel _user;
        private HttpResponseMessage _response;
        public EventHandler<SessionEventArgs> OnSuccessfulConnect;
        #endregion

        #region Dependencies 
        private IHttpService _httpService;
        #endregion

        #region Properties
        public UserModel User
        {
            get => _user ??= new UserModel();
            set => SetProperty(ref _user, value);
        }
        #endregion

        #region Commands
        public ICommand SignInCommand => new DelegateCommand(SignIn);
        #endregion


        #region Methods
        private async Task SignIn()
        {
            if (string.IsNullOrEmpty(User.Username) || string.IsNullOrEmpty(User.Password))
            {
                MessageBox.Show("Probably one or both of fields are empty!");
                return;
            }
            using (HttpClient client = new HttpClient())
            {
                var serializedData = JsonConvert.SerializeObject(User);
                try
                {
                    _response = await _httpService.PostData("login", serializedData);
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                switch (_response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        _httpService.SetAccessToken(await _response.Content.ReadAsStringAsync());
                        Application.Current.Dispatcher.Invoke(() =>
                        OnSuccessfulConnect?.Invoke(this, new SessionEventArgs
                        {
                            HoveViewModelContext = new HomeViewModel(new Validation(), _httpService)
                        }));
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        MessageBox.Show("Invalid login or password");
                        break;
                    default:
                        MessageBox.Show("External Server error");
                        break;
                }
            }
        }
        #endregion

        public LoginViewModel(IHttpService httpService)
        {
            _httpService = httpService;
        }
    }
}
