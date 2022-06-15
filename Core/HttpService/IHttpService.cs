using System.Net.Http;
using System.Threading.Tasks;

namespace lib_iis.Core.HttpService
{
    internal interface IHttpService
    {
        Task<HttpResponseMessage> PostData(string controller, string jsonString);
        Task<HttpResponseMessage> DeleteData(string controller, string request);
        Task<HttpResponseMessage> GetData(string controller, string request);
        void SetAccessToken(string token);
    }
}
