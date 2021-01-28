using System.Collections.Generic;

namespace EI.RP.DomainServices.Validation
{
	public interface IReservedIbanSettings
	{
		IEnumerable<string> ReservedIban { get; }
	}
}