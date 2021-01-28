namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions
{
	public enum AccountsPaymentConfigurationFlowStartType
	{
		//default
		NotSpecified=0,
		ShowHistory ,
		ConfigureDirectDebit,
		EditDirectDebit,
		EstimateYourCost,
		UseExistingAccountDirectDebit,

		AddGasAccount,
		MoveElectricity,
		MoveGas,

		/// <summary>
		/// MoveGasAndAddElectricity or MoveElectricityAndAddGas
		/// </summary>
		MoveOneAndAddAnother,
		MoveElectricityAndCloseGas,
		MoveElectricityAndGas,

		EqualizerMonthlySetup,

		SmartActivation
	}
}