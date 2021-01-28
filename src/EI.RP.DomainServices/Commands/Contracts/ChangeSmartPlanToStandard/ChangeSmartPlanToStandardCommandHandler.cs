using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DomainServices.InternalShared.ContractSales;

namespace EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard
{
	internal class ChangeSmartPlanToStandardCommandHandler : ICommandHandler<ChangeSmartPlanToStandardCommand>
	{
		private readonly IContractSaleCommand _contractSaleBuilder;
	
		public ChangeSmartPlanToStandardCommandHandler(IContractSaleCommand contractSaleBuilder)
		{
			_contractSaleBuilder = contractSaleBuilder;
		}

		public async Task ExecuteAsync(ChangeSmartPlanToStandardCommand command)
		{
			await _contractSaleBuilder.ExecuteChangeSmartPlanToStandardContractSale(command.ElectricityAccountNumber);
		}
	}
}