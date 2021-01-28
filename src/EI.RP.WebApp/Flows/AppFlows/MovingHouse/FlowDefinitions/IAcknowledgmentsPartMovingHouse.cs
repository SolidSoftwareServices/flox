using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface IAcknowledgmentsPartMovingHouse : IUiFlowScreenModel
    {
        ConfirmationInputFields UserConfirmationInputFields { get; set; }
        string ElectricityAccountNumber { get; set; }
        string GasAccountNumber { get; set; }
        ShowTermsAcknowledgmentType? ShowTermsAcknowledgmentType { get; set; }
	}

    public class ConfirmationInputFields
    {
	    [BooleanRequired(ErrorMessage = "Please confirm that you are authorised to provide Electric Ireland with this information ")]
        public bool InformationCollectionAuthorized { get; set; }

        [BooleanRequired(ErrorMessage = "Please confirm that you have read and accept the Electric Ireland Terms and Conditions")]
        public bool TermsAndConditionsAccepted { get; set; }
    }

    public class DeviceRegisterUiModel
    {
		[JsonConstructor]
	    public DeviceRegisterUiModel()
	    {
	    }
	    public DeviceRegisterUiModel(DeviceRegisterInfo register)
	    {
		    MeterType = register.MeterType;
		    MeterNumber = register.MeterNumber;
		    MeterUnit = register.MeterUnit == Ei.Rp.DomainModels.MappingValues.MeterUnit.KWH ? "kWh" : register.MeterUnit;
	    }

	    public string MeterType { get; set; }
	    public string MeterNumber { get; set; }
	    public ClientAccountType AccountType { get; set; }
	    public string MeterUnit { get; set; }
    }
}