using EI.RP.CoreServices.System;
using Microsoft.AspNetCore.Http;

namespace EI.RP.CoreServices.Http.Server
{
    class HttpContextSessionDataProvider:ISessionData
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextSessionDataProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Set<TValue>(string key, TValue value) where TValue : class
        {
            _contextAccessor.HttpContext.Session.Set(key,value.ToByteArray());
        }

        public TValue Get<TValue>(string key) where TValue : class
        {
            if (!_contextAccessor.HttpContext.Session.TryGetValue(key, out byte[] value))
            {
                return null;
            }

            return value.ToObject<TValue>();
        }
    }
}
