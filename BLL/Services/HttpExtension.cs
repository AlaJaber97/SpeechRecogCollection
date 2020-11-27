using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class HttpExtension<T> where T : class
    {
        public async Task<T> Get(string Controller,string Token = null, string Parameter = "")
        {
            var Respones = await HttpClinetRespones.Get(HttpMethod.Get, $"{Controller}/{Parameter}", Token);
            if (Respones.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var ss = await Respones.Content.ReadAsStringAsync();
                    return await Respones.Content.ReadAsAsync<T>();
                }
                catch (Exception)
                {
#if DEBUG
                    var contentstring = await Respones.Content.ReadAsStringAsync();
#endif
                    return null;

                }
            }
            else
            {
                return null;
            }
        }
        public async Task<(HttpStatusCode statusCode, T body, string message)> GetReturnStatusCodeAndString(string Controller,string Token, string Parameter = "")
        {
            var Respones = await HttpClinetRespones.Get(HttpMethod.Get, $"{Controller}/{Parameter}", Token);
            if (Respones.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var ss = await Respones.Content.ReadAsStringAsync();
                    return (Respones.StatusCode, await Respones.Content.ReadAsAsync<T>(),string.Empty);
                }
                catch (Exception)
                {
                    var contentstring = await Respones.Content.ReadAsStringAsync();
                    return (Respones.StatusCode,null, contentstring);

                }
            }
            else
            {
                var contentstring = await Respones.Content.ReadAsStringAsync();
                return (Respones.StatusCode, null, contentstring);
            }
        }
        public async Task<string> GetString(string Controller, string Token, string Parameter = "")
        {
            var Respones = await HttpClinetRespones.Get(HttpMethod.Get, $"{Controller}/{ Parameter}", Token);
            if (Respones.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await Respones.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<(HttpStatusCode statusCode,string message)> PostReturnStatusCodeAndString(string Controller, object model, string Token,string Parameter = "")
        {
            var Respones = await HttpClinetRespones.Get(HttpMethod.Post, $"{Controller}/{Parameter}", Token, model);
            return (Respones.StatusCode, await Respones.Content.ReadAsStringAsync());
        }
        public async Task<(HttpStatusCode statusCode, string message)> UploadFileReturnStatusCodeAndString(string Controller, MultipartFormDataContent content, string Token, string Parameter = "")
        {
            var Respones = await HttpClinetRespones.Upload($"{Controller}/{Parameter}", Token, content);
            return (Respones.StatusCode, await Respones.Content.ReadAsStringAsync());
        }
    }
}