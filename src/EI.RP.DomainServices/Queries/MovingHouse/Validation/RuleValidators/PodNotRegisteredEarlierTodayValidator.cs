using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
    internal class PodNotRegisteredEarlierTodayValidator : IMovingHouseValidator
    {
        private readonly IDomainQueryResolver _queryResolver;
        private readonly ICompositeRuleValidatorOutputTypeResolver _compositeOutputResolver;
        public string ResolveCacheKey(MovingHouseValidationQuery query)
        {
	        return $"{query.ValidateNewPremisePodNotRegisteredToday}-{query.IsMPRNDeregistered}-{query.NewMPRN}-{query.IsGPRNDeregistered}-{query.NewGPRN}-{query.MoveOutDate.GetValueOrDefault().Date}";
        }
        public PodNotRegisteredEarlierTodayValidator(
            IDomainQueryResolver queryResolver,
            ICompositeRuleValidatorOutputTypeResolver compositeOutputResolver)
        {
            _queryResolver = queryResolver;
            _compositeOutputResolver = compositeOutputResolver;
        }

        public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
        {
            var movingHouseValidationType = MovingHouseValidationType.PodNotRegisteredEarlierToday;

            if (!query.ValidateNewPremisePodNotRegisteredToday)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            if (!query.IsValidQuery(out var nothing))
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }
            return new MovingHouseRulesValidationResult
            {
                Output = await ValidatePodNotRegisteredEarlierToday(query.IsMPRNDeregistered, query.NewMPRN, query.IsGPRNDeregistered, query.NewGPRN, query.MoveOutDate),
                MovingHouseValidationType = movingHouseValidationType
            };
        }

        private async Task<OutputType> ValidatePodNotRegisteredEarlierToday(bool isMPRNDeregistered, string newMPRN, bool isGPRNDeregistered, string newGPRN, DateTime? moveOutDate)
        {
            var electricityTask =
                string.IsNullOrEmpty(newMPRN) ? null : CanMoveToNewPremise((ElectricityPointReferenceNumber)newMPRN, moveOutDate.Value, isMPRNDeregistered);

            var gasTask =
                string.IsNullOrEmpty(newGPRN) ? null : CanMoveToNewPremise((GasPointReferenceNumber)newGPRN, moveOutDate.Value, isGPRNDeregistered);

            var outputTypeTask = _compositeOutputResolver.Resolve(
                electricityTask,
                gasTask, CompositeRuleValidatorOutputTypePassIf.AllPassed);

            return await outputTypeTask;
        }

        private async Task<OutputType> CanMoveToNewPremise(PointReferenceNumber prn, DateTime moveOutDate, bool isDeregistered)
        {
            if (isDeregistered) return OutputType.NotExecuted;
            var premiseTask = _queryResolver.GetPremiseByPrn(prn);
            var premise = await premiseTask;
            if (premise?.Installations == null || !premise.Installations.Any())
                return OutputType.NotExecuted;

            var installationInfo = premise.Installations.FirstOrDefault();
            if (installationInfo?.Devices == null || !installationInfo.Devices.Any())
                return OutputType.NotExecuted;

            var moveInStartDate = await GetMoveInDate(installationInfo.Devices, moveOutDate);
            
            if (moveInStartDate == default(DateTime) || moveInStartDate.Date == DateTime.Now.AddDays(2).Date)
            {
                return OutputType.Failed;
            }

            return OutputType.Passed;
        }

        private async Task<DateTime> GetMoveInDate(IEnumerable<DeviceInfo> devices, DateTime moveOutDate)
        {
            var newPremiseDateList = new List<DateTime> { moveOutDate };
            var newPremiseMoveInDateList = new List<DateTime>();
            var newPremiseMoveOutDateList = new List<DateTime>();
            foreach (var device in devices)
            {

                var deviceMoveInDate = device.MeterReadingResults.Where(m => m.MeterReadingReasonID == "06")
                                           .OrderByDescending(x => x.ReadingDateTime).FirstOrDefault()?.ReadingDateTime ?? default(DateTime);
                newPremiseDateList.Add(deviceMoveInDate);
                newPremiseMoveInDateList.Add(deviceMoveInDate);
                var deviceMoveOutDate = device.MeterReadingResults.Where(m => m.MeterReadingReasonID == "03")
                                            .OrderByDescending(x => x.ReadingDateTime).FirstOrDefault()?.ReadingDateTime ?? default(DateTime);
                newPremiseDateList.Add(deviceMoveOutDate);
                newPremiseMoveOutDateList.Add(deviceMoveOutDate);
            }

            var newPremiseDate = newPremiseDateList.OrderByDescending(d => d).FirstOrDefault();
            var newPremiseMoveInDate = newPremiseMoveInDateList.OrderByDescending(d => d).FirstOrDefault();
            var newPremiseMoveOutDate = newPremiseMoveOutDateList.OrderByDescending(d => d).FirstOrDefault();
            
            if (newPremiseDate == newPremiseMoveInDate)
            {
                return newPremiseMoveInDate.Date >= DateTime.Now.Date ? default(DateTime) : newPremiseDate.AddDays(2);
            }
            
            if (newPremiseDate == newPremiseMoveOutDate)
            {
                return newPremiseMoveOutDate.Date > DateTime.Now.Date ? default(DateTime) : newPremiseDate.AddDays(1);
            }
            
            return newPremiseDate.AddDays(1);
        }
    }
}