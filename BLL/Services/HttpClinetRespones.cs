using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BLL.Services
{
    public static class HttpClinetRespones
    {
        public static string BaseUrl = "https://apisystem.azurewebsites.net";
        //public static string BaseUrl = "https://localhost:44332";
        public async static Task<HttpResponseMessage> Get(HttpMethod method, string url,string Token = null, object model = null)
        {
            Task<HttpResponseMessage> response;
            var URL = new Uri($"{BaseUrl}/api/{url}");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = GetHeaderValue(Token);
            switch (method.Method.ToString())
            {
                case "GET":
                    response = client.GetAsync(URL);
                    break;
                case "POST":
                    response = client.PostAsJsonAsync(URL, model);
                    break;
                case "DELETE":
                    response = client.DeleteAsync(URL);
                    break;
                default:
                    response = null;
                    break;
            }
            try
            {
                var result = await response;
                return result;
            }
            catch (Exception ex)
            {
//#if DEBUG
//                _ = App.Current.MainPage.DisplayAlert("POST/GET request", ex.ToString(), "Ok");
//#endif
                throw ex;
            }
        }
        public async static Task<HttpResponseMessage> Upload(string url, string Token = null, MultipartFormDataContent content = null)
        {
            Task<HttpResponseMessage> response;
            var URL = new Uri($"{BaseUrl}/api/{url}");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = GetHeaderValue(Token);
            response = client.PostAsync(URL, content);
            try
            {
                var result = await response;
                return result;
            }
            catch (Exception ex)
            {
                //#if DEBUG
                //                _ = App.Current.MainPage.DisplayAlert("POST/GET request", ex.ToString(), "Ok");
                //#endif
                throw ex;
            }
        }
        private static AuthenticationHeaderValue GetHeaderValue(string Token)
        {
            if (Token != null)
            {
                return new AuthenticationHeaderValue("Bearer", Token);
            }
            else
            {
                return null;
            }
        }
    }
}
