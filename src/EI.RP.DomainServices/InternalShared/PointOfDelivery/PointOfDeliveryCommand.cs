using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Infrastructure.Mappers;

namespace EI.RP.DomainServices.InternalShared.PointOfDelivery
{
	class PointOfDeliveryCommand : IPointOfDeliveryCommand
	{
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
		private readonly IDomainMapper<PointOfDeliveryDto, PointOfDeliveryInfo> _domainMapper;

		public PointOfDeliveryCommand(ISapRepositoryOfCrmUmc crmUmcRepository, IDomainMapper<PointOfDeliveryDto, PointOfDeliveryInfo> domainMapper)
		{
			_crmUmcRepository = crmUmcRepository;
			_domainMapper = domainMapper;
		}

		public async Task<PointOfDeliveryInfo> AddPointOfDelivery(PointReferenceNumber prn, 
															     ClientAccountType accountType, 
															     string usePremisesAddressOfAccountNumber = null, 
															     string usePremisesAddressFromPremiseId = null)
		{
			var dto = new PointOfDeliveryDto
			{
				ExternalID = (string)prn,
				DivisionID = accountType.ToDivisionType(),
				PremiseID = string.Empty,
				PointOfDeliveryID = string.Empty,
				Premise = new PremiseDto
				{
					AddressInfo = await MapAddress(usePremisesAddressOfAccountNumber, usePremisesAddressFromPremiseId),
					PremiseType = new PremiseTypeDto(),
					PremiseDetails =
						new PremiseDetailDto { PremiseNotes = string.Empty },
				}
			};

			dto.SetAddAsOdata(false);
			dto = await _crmUmcRepository.AddThenGet(dto);

			var pointOfDelivery = await _domainMapper.Map(dto);
			pointOfDelivery.IsNewAcquisition = true;

			return pointOfDelivery;
		}

		private async Task<AddressInfoDto> MapAddress(string usePremisesAddressOfAccountNumber, string usePremisesAddressFromPremiseId)
		{
			if (!string.IsNullOrEmpty(usePremisesAddressOfAccountNumber))
				return await MapAddressFromAccountNumber(usePremisesAddressOfAccountNumber);

			if (!string.IsNullOrEmpty(usePremisesAddressFromPremiseId))
				return await MapAddressFromPremiseId(usePremisesAddressFromPremiseId);

			throw new NotSupportedException();
		}

		private async Task<AddressInfoDto> MapAddressFromAccountNumber(string accountNumber)
		{
			var businessAgreement = await _crmUmcRepository
				.NewQuery<BusinessAgreementDto>()
				.Key(accountNumber)
				.Expand(x => x.ContractItems)
				.Expand(x => x.ContractItems[0].Premise)
				.GetOne();

			var premise = businessAgreement.ContractItems?.FirstOrDefault()?.Premise;

			return MapAddressDetail(premise);
		}

		private async Task<AddressInfoDto> MapAddressFromPremiseId(string premiseId)
		{
			var premise = (await _crmUmcRepository.NewQuery<PremiseDto>()
				.Expand(x => x.ContractItems)
				.Key(premiseId).GetMany()).FirstOrDefault();

			return MapAddressDetail(premise);
		}

		private static AddressInfoDto MapAddressDetail(PremiseDto premise)
		{
			var address = new AddressInfoDto();
			address.HouseNo = premise?.AddressInfo?.HouseNo ?? string.Empty;
			address.Street = premise?.AddressInfo?.Street ?? string.Empty;
			address.City = premise?.AddressInfo?.City ?? string.Empty;
			address.PostalCode = premise?.AddressInfo?.PostalCode ?? string.Empty;
			address.CountryID = "IE";

			return address;
		}
	}
}
