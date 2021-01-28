using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;

using EI.RP.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using NLog;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DataModels.Switch;

namespace EI.RP.DomainServices.Queries.Contracts.PointOfDelivery
{
	[Obsolete("TODO:this has to be moved to the premises aggregate")]
    internal class PointOfDeliveryQueryHandler : QueryHandler<PointOfDeliveryQuery>
    {

		private static readonly ILogger Logger=LogManager.GetCurrentClassLogger();
        private readonly ISapRepositoryOfCrmUmc _repository;

        private readonly ISwitchDataRepository _switchDataRepository;

        public PointOfDeliveryQueryHandler(ISapRepositoryOfCrmUmc repository, ISwitchDataRepository switchDataRepository)
        {
            _repository = repository;
            _switchDataRepository = switchDataRepository;
        }

        protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(PointOfDeliveryQuery queryModel)
        {
	        if (queryModel.Prn == null || !queryModel.Prn.HasValue)
	        {
		        return new TQueryResult[0];
	        }
			var result = new PointOfDeliveryInfo();
            PointOfDeliveryDto pointOfDelivery;
            try
            {
                pointOfDelivery = (await _repository
                    .NewQuery<PointOfDeliveryDto>()
                    .Key(queryModel.Prn.ToString())
                    .Expand(x => x.Premise).GetMany()).FirstOrDefault();
            }
            catch
            {
                pointOfDelivery = null;
            }

            if (pointOfDelivery != null)
            {
                result.PointOfDeliveryId = pointOfDelivery.PointOfDeliveryID;
                result.PremiseId = pointOfDelivery.PremiseID;
	            result.IsNewAcquisition = false;
                var premise = (await _repository.NewQuery<PremiseDto>()
                    .Expand(x => x.ContractItems)
                    .Key(pointOfDelivery.PremiseID).GetMany()).FirstOrDefault();

	            if (premise != null)
	            {
		            result.AddressInfo = MapAddressDetail(premise);
	            }

                var addressDetail = await GetAddressDetail(queryModel.Prn);
                if (addressDetail!=null)
                    result.IsAddressInSwitch = true;
            }
	        else
	        {
                var addressDetail = await GetAddressDetail(queryModel.Prn);
                if (addressDetail != null)
                {
                    result.AddressInfo = MapAddressDetail(addressDetail);
                    result.IsNewAcquisition = true;
                    result.IsAddressInSwitch = true;
                }
                else
                {
                    return new TQueryResult[0];
                }
            }

	        return result.ToOneItemArray().Cast<TQueryResult>();
        }

        private async Task<AddressDetailDto> GetAddressDetail(PointReferenceNumber prn)
        {
            if (prn.Type != PointReferenceNumberType.Mprn) return null;

            try
            {
                var addressDetail = await _switchDataRepository.GetAddressDetailFromMprn((string)prn);
                return addressDetail;
            }
            catch (Exception ex)
            {
                Logger.Warn(() => ex.ToString());
                return null;
            }
        }

        private static AddressInfo MapAddressDetail(AddressDetailDto addressDetailDto)
        {
            return new AddressInfo
            {
                City = addressDetailDto.City,
                Country = addressDetailDto.Country,
                DuosGroup = addressDetailDto.DuosGroup,
                HouseNo = addressDetailDto.HouseNo,
                HouseNo2 = addressDetailDto.HouseNo2,
                PostalCode = addressDetailDto.PostalCode,
                Region = addressDetailDto.Region,
                ShortForm = addressDetailDto.ShortForm,
                Street = addressDetailDto.Street,
                Street2 = addressDetailDto.Street2,
                Street3 = addressDetailDto.Street3,
                Street4 = addressDetailDto.Street4,
                Street5 = addressDetailDto.Street5
            };
        }

        private static AddressInfo MapAddressDetail(PremiseDto premiseDto)
        {
            return new AddressInfo
            {
                City = premiseDto.AddressInfo.City,
                Country = premiseDto.AddressInfo.CountryName,
                HouseNo = premiseDto.AddressInfo.HouseNo,
                PostalCode = premiseDto.AddressInfo.PostalCode,
                Region = premiseDto.AddressInfo.Region,
                ShortForm = premiseDto.AddressInfo.ShortForm,
                Street = premiseDto.AddressInfo.Street,
            };
        }

        protected override Type[] ValidQueryResultTypes { get; } = { typeof(PointOfDeliveryInfo) };
    }
}
