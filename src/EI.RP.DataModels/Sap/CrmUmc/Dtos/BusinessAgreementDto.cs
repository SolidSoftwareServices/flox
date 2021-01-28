using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
	public partial class BusinessAgreementDto
	{
		

		public override ODataUpdateType UpdateMode()
		{
			return ODataUpdateType.FullModel;
		}
	}
}