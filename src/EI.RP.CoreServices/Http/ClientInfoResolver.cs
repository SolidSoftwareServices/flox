using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using NLog;
using UAParser;

namespace EI.RP.CoreServices.Http
{
    class ClientInfoResolver : IClientInfoResolver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IHttpContextAccessor _contextAccessor;
        private static readonly Lazy<UAParser.Parser> Parser =new Lazy<Parser>(()=> UAParser.Parser.GetDefault());

		public ClientInfoResolver(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        


            ClientInfo=new Lazy<ClientInfo>(() =>
            {
                ClientInfo result = null;

                var httpContext = _contextAccessor.HttpContext;
                if (httpContext != null)
                {

                    try
                    {
                        result = Parser.Value.Parse(ResolveUserAgent());
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex);
                        result = null;
                    }
                }

                return result;
            });
        }

        public string ResolveUserAgent()
        {
            string result = string.Empty;
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext != null)
            {
                var httpRequest = httpContext.Request;

                var headerName = "User-Agent";
                if (httpRequest != null && httpRequest.Headers.ContainsKey(headerName))
                {
                    result = httpRequest.Headers[headerName];
                }
            }

            return result;
        }

        public string ResolveIp()
        {
            string ip = IPAddress.None.ToString();

            var httpContext = _contextAccessor.HttpContext;
            if (httpContext != null)
            {
                ip = GetHeaderValueAs<string>("X-Forwarded-For")?.Split(',').FirstOrDefault();

                // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
                if (string.IsNullOrWhiteSpace(ip) && _contextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                    ip = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrWhiteSpace(ip))
                    ip = GetHeaderValueAs<string>("REMOTE_ADDR");

                if (string.IsNullOrWhiteSpace(ip))
                    ip=IPAddress.None.ToString();

            }

            return ip;
        }

        private readonly Lazy<ClientInfo> ClientInfo;
        
        public string ResolveBrowserVersion()
        {
            return ClientInfo.Value != null ? ClientInfo.Value.UA.ToString() : string.Empty;
        }

        public string ResolveOperatingSystem()
        {
            return ClientInfo.Value != null ? ClientInfo.Value.OS.ToString() : string.Empty;
        }

        public string ResolveDevice()
        {
            return ClientInfo.Value != null ? ClientInfo.Value.Device.ToString() : string.Empty;
        }

        public T GetHeaderValueAs<T>(string headerName)
        {
            if (_contextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                string rawValues = values.ToString();  

                if (!string.IsNullOrWhiteSpace(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T),CultureInfo.InvariantCulture);
            }
            return default(T);
        }
    }
}