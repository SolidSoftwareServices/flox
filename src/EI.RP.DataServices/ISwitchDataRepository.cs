using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.DataModels.Switch;

namespace EI.RP.DataServices
{
	public interface ISwitchDataRepository: IDataService
	{
		Task<IEnumerable<DiscountDto>> GetDiscountsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<AddressDetailDto> GetAddressDetailFromMprn(string mprn, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<RegisterDto>> GetRegisterDetailFromMprn(string mprn, CancellationToken cancellationToken = default(CancellationToken));
    }
}