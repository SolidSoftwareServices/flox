using Microsoft.Extensions.Primitives;

namespace EI.RP.CoreServices.Http
{
    public interface IClientInfoResolver
    {
        string ResolveUserAgent();
        string ResolveIp();
        string ResolveBrowserVersion();
        string ResolveOperatingSystem();
        string ResolveDevice();
    }

}