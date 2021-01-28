namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
	public partial class AccountAddressDto
	{
		public override bool AddsAsOData() => false;

		public AccountAddressDto()
		{
			AccountAddressDependentFaxes = null;
			AccountAddressDependentMobilePhones = null;
			AccountAddressUsages = null;
			AccountAddressDependentPhones = null;
			AccountAddressDependentEmails = null;
			AccountAddressDependentMobilePhones = null;
			AccountAddressUsages = null;
			AccountAddressDependentEmails = null;
		}

	}
}