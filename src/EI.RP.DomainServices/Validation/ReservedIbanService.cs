using System;

namespace EI.RP.DomainServices.Validation
{
	public class ReservedIbanService : IReservedIbanService
	{
		private readonly IReservedIbanSettings _reservedIbanSettings;
		public ReservedIbanService(IReservedIbanSettings reservedIbanSettings)
		{
			_reservedIbanSettings = reservedIbanSettings;
		}

		public bool IsReservedIban(string iban)
		{
			foreach (var reservedIBan in _reservedIbanSettings.ReservedIban)
			{
				if (string.Equals(reservedIBan.Trim(), iban.Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}
	}
}