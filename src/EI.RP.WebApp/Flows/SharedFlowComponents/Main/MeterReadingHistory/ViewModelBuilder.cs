using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;


using System;
using Ei.Rp.DomainModels.MappingValues;
using System.Collections.Generic;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.System.Paging;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.MeterReadingHistory
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
			_domainQueryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
            var accountNumber = componentInput.AccountNumber;

            var userAccountTask = 
                _domainQueryResolver.GetAccountInfoByAccountNumber(accountNumber);

            var meterReadingHistory = await _domainQueryResolver.GetMeterReadings(accountNumber);

            var result = new ViewModel
            {
                AccountType = (await userAccountTask).ClientAccountType,
                ScreenModel = screenModelContainingTheComponent,
                ContainedInFlowType = componentInput.ContainedInFlowType,
                IsPagingEnabled = componentInput.IsPagingEnabled,
                NumberOfPageLinks = componentInput.NumberPagingLinks,
                TableId = componentInput.TableId,
                PaginationId = componentInput.PaginationId,
                WhenChangingPageFocusOn = componentInput.WhenChangingPageFocusOn
            };

            if (result.AccountType == ClientAccountType.Electricity)
            {
                result.Paging = GetElectricityMeterReadingHistory(meterReadingHistory)
                    .ToPagedData(componentInput.PageSize, componentInput.PageIndex);
            }
            else if (result.AccountType == ClientAccountType.Gas)
            {
                result.Paging = GetGasMeterReadingHistory(meterReadingHistory)
                    .ToPagedData(componentInput.PageSize, componentInput.PageIndex);
            }
            else
            {
                throw new NotSupportedException();
            }

			return result;
		}

        private ViewModel.MeterRead[] GetElectricityMeterReadingHistory(IEnumerable<MeterReadingInfo> meterReadingHistory)
        {
            var lstViewModel = new List<ViewModel.MeterRead>();

            foreach (var item in meterReadingHistory)
            {
                var meterHistoryViewModel = new ViewModel.MeterRead();

                if (item.MeterNumber != null && item.MeterNumber.Length > 4)
                {
                    meterHistoryViewModel.MeterNumber = item.MeterNumber.Substring(item.MeterNumber.Length - 4).PadLeft(9, '*');
                }
                else
                {
                    meterHistoryViewModel.MeterNumber = item.MeterNumber;
                }

                if (item.SerialNumber != null && item.SerialNumber.Length > 4)
                {
                    meterHistoryViewModel.MaskedSerialNumber = item.SerialNumber.Substring(item.SerialNumber.Length - 4).PadLeft(9, '*');
                }

                var model = Map(meterHistoryViewModel, item);

                model.MeasurementUnitForConsumption = item.MeasurementUnitForConsumption;

                lstViewModel.Add(meterHistoryViewModel);
            }

            return lstViewModel.ToArray();
        }

        private ViewModel.MeterRead[] GetGasMeterReadingHistory(IEnumerable<MeterReadingInfo> meterReadingHistory)
        {
            var lstViewModel = new List<ViewModel.MeterRead>();
            foreach (var item in meterReadingHistory)
            {
                var meterHistoryViewModel = new ViewModel.MeterRead
                {
                    MeterNumber = item.MeterNumber
                };

                if (item.SerialNumber != null && item.SerialNumber.Length > 4)
                {
                    meterHistoryViewModel.MaskedSerialNumber = item.SerialNumber.Substring(item.SerialNumber.Length - 4).PadLeft(item.SerialNumber.Length, '*');
                }

                var model = Map(meterHistoryViewModel, item);

                model.MeasurementUnitForConsumption = item.ReadingUnit;

                var tempConversionFactor = item.ConversionFactor;

                tempConversionFactor = Math.Truncate(tempConversionFactor * 1000m) / 1000m;

                model.ConversionFactor = $"{tempConversionFactor * 1000:N0}";

                meterHistoryViewModel.Consumption = Convert.ToInt64(item.Consumption).ToString();

                lstViewModel.Add(model);
            }

            return lstViewModel.ToArray();
        }

        private ViewModel.MeterRead Map(ViewModel.MeterRead meterHistoryViewModel, MeterReadingInfo item)
        {
            meterHistoryViewModel.MeterType = item.MeterType;
            meterHistoryViewModel.ToDate = item.ReadingDate;
            meterHistoryViewModel.Reading = $"{item.Reading:N0}";
            meterHistoryViewModel.IsPendingReadingVerification = item.IsPendingReadingVerification;
            meterHistoryViewModel.IsEstimate = item.IsEstimate;
            meterHistoryViewModel.MeterReadingType = ResolveMeterReadingUiValue(item.MeterReadingCategory);
            meterHistoryViewModel.Consumption = $"{item.Consumption:N0}";
            meterHistoryViewModel.FromDate = item.DateFrom;
            meterHistoryViewModel.SerialNumber = item.SerialNumber;

            return meterHistoryViewModel;
        }

        private string ResolveMeterReadingUiValue(MeterReadingCategoryType meterReadingType)
        {
            if (meterReadingType == null || meterReadingType == MeterReadingCategoryType.None) return "None";
            if (meterReadingType == MeterReadingCategoryType.Customer) return "Customer";
            if (meterReadingType == MeterReadingCategoryType.EstimatedA ||
                meterReadingType == MeterReadingCategoryType.EstimatedB) return "Estimated";
            if (meterReadingType == MeterReadingCategoryType.Network) return "Networks";

            return meterReadingType.ToString();
        }
    }
}