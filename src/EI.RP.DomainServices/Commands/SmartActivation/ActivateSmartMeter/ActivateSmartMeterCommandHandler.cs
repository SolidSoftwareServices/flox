using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DomainServices.InternalShared.ContractSales;

namespace EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter
{
	internal class ActivateSmartMeterCommandHandler : ICommandHandler<ActivateSmartMeterCommand>
	{
		private readonly IContractSaleCommand _contractSaleBuilder;

		public ActivateSmartMeterCommandHandler(IContractSaleCommand contractSaleBuilder)
		{
			_contractSaleBuilder = contractSaleBuilder;
		}

		public async Task ExecuteAsync(ActivateSmartMeterCommand command)
		{
			var contractSale = await _contractSaleBuilder.ActivateSmartMeter(command);
		}
	}
}