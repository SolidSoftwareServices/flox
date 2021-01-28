using System;
using System.Threading.Tasks;

namespace EI.RP.SwitchApi.Clients
{
	public interface ISwitchApiSettings
	{
		string SwitchApiEndPoint { get; }
		Task<string> SwitchApiBearerTokenProviderUrlAsync();
	}
}