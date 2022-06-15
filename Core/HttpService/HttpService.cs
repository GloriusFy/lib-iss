using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lib_iis.Core.HttpService
{
    internal class HttpService : IHttpService
    {
        

        private static readonly string _endPoint = "http://localhost:8080/api/";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static HttpService _httpService = new HttpService();
        private static string _accessToken { get; set; } 
        static HttpService()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
            _httpClient.BaseAddress = new Uri(_endPoint);
            

        }
        private HttpService() { }
        public static HttpService HttpServiceInstance
        {
            get => _httpService;
        }

        public async Task<HttpResponseMessage> PostData(string controller,string jsonString)
        {
            var respone = await _httpClient.PostAsync($"{controller}",
                new StringContent(jsonString, Encoding.UTF8, "aplication/json"));
            return respone;
        }
        public async Task<HttpResponseMessage> DeleteData(string controller, string request)
        {
            return await _httpClient.DeleteAsync($"{controller}/?{request}");
        }
        public async Task<HttpResponseMessage> GetData(string controller, string request)
        {
            return await _httpClient.GetAsync($"{controller}/?{request}");
            

        }
        
        public void SetAccessToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("access-token", token);
        }
    }
}
