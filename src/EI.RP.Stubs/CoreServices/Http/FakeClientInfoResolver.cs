using System;
using System.Net;
using EI.RP.CoreServices.Http;

namespace EI.RP.Stubs.CoreServices.Http
{
    class FakeClientInfoResolver : IClientInfoResolver
    {
        public string ResolveBrowserVersion()
        {
            return string.Empty;
        }

        public string ResolveDevice()
        {
            return Environment.MachineName;
        }

        public string ResolveIp()
        {
            return IPAddress.Loopback.ToString();
        }

        public string ResolveOperatingSystem()
        {
            return Environment.OSVersion.ToString();
        }

        public string ResolveUserAgent()
        {
            return "Commandline";
        }
    }
}