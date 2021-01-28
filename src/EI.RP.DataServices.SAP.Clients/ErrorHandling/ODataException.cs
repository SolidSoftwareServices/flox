using System.Xml.Serialization;

namespace EI.RP.DataServices.SAP.Clients.ErrorHandling
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
	[XmlRoot(ElementName = "error", Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", IsNullable = false)]
	public class ODataException
	{
		[XmlElement("code")]
		public string ErrorCode { get; set; }

		[XmlElement("message")]
		public ODataErrorMessage ErrorMessage { get; set; }

		[XmlElement("innererror")]
		public ODataInternalException InnerException { get; set; }
	}
}