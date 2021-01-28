using System.Threading.Tasks;

namespace EI.RP.SwitchApi.Clients
{
    interface ISwitchApiHttpClient
    {
        Task<T> GetAsync<T>(string url) where T : class;
    }
}