using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeechRecg.Web.Services
{
    public class SessionAccesser
    {
        private IHttpContextAccessor Context { get; set; }
        public SessionAccesser(IHttpContextAccessor httpContext)
        {
            Context = httpContext;
        }
        public void Set(string key, string value)
        {
            Context.HttpContext.Session.SetString(key, value);
        }
        public string Get(string key)
        {
            return Context.HttpContext.Session.GetString(key);
        }
        public bool Check(string key)
        {
            Context.HttpContext.Session.TryGetValue(key, out byte[] value);
            return value != null;
        }
    }
}
