using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class ShowMovingHouseValidationError : MovingHouseScreen
	{
		readonly IDomainQueryResolver _domainQueryResolver;
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public ShowMovingHouseValidationError(IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

		public override ScreenName ScreenStep => MovingHouseStep.ShowMovingHouseValidationError;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
			var movingHouseValidationResult = (await _domainQueryResolver.GetMovingHouseValidationResult(
				rootData.ElectricityAccountNumber,
				rootData.GasAccountNumber,
				step2Data?.NewMPRN)).ToList();

			if(movingHouseValidationResult.Count == 0)
			{
				throw new InvalidOperationException($"{nameof(ShowMovingHouseValidationError)} reached, but there are no validation errors.");
			}

			LogValidationResult(movingHouseValidationResult, contextData);

			var errorMessageBody = "Please call 1850 372 372 to complete your move.";

			if (movingHouseValidationResult.Any(x =>
				x.MovingHouseValidationType == MovingHouseValidationType.IsNonSmartMoveOutDeviceSet &&
				x.Output == OutputType.Failed))
			{
				errorMessageBody = "To move your account to a different address, please call customer service on 1850 372 372 from 8am - 8pm, Monday - Saturday.";
			}
			else if (movingHouseValidationResult.Any(x =>
				x.MovingHouseValidationType == MovingHouseValidationType.IsNonSmartMoveInDeviceSet &&
				x.Output == OutputType.Failed))
			{
				errorMessageBody =
					"To move your account to an address with a smart meter, please call customer service on 1850 372 372 from 8am - 8pm, Monday - Saturday.";

			}

			var screenModel = new ScreenModel
			{
				ErrorMessageTitle = "We cannot process your move request at the moment.",
				ErrorMessageBody = errorMessageBody
			};

			SetTitle(Title, screenModel);

			return screenModel;
		}

		private void LogValidationResult(IEnumerable<MovingHouseRulesValidationResult> movingHouseValidations, IUiFlowContextData contextData)
		{
			var lastStep = contextData.EventsLog.LastOrDefault();
			Logger.Info(() => string.Join(
				Environment.NewLine, 
				$"{lastStep?.ToStep ?? "FlowInitializer. "}",
				movingHouseValidations.Select(x => $"{nameof(ShowMovingHouseValidationError)}. Rule={x.MovingHouseValidationType} Result={x.Output}")));
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public string ErrorMessageTitle { get; set; }
			public string ErrorMessageBody { get; set; }
		}
	}
}