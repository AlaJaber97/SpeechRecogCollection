using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechRecg.Web.Services
{
    public class ApiClient : HttpClient
    {
        private IHttpContextAccessor Context { get; set; }
        public ApiClient(IHttpContextAccessor httpContext)
        {
            Context = httpContext;
        }
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = Context.HttpContext.Session.GetString("Jwt");
            if (!string.IsNullOrEmpty(token))
                this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
