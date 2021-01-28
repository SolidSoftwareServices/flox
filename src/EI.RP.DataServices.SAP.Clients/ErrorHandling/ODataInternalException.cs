using System.Xml.Serialization;

namespace EI.RP.DataServices.SAP.Clients.ErrorHandling
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
	public class ODataInternalException
	{
		[XmlElement("message")]
		public string Message { get; set; }

		[XmlElement("type")]
		public string Type { get; set; }

		[XmlElement("stacktrace")]
		public string StackTrace { get; set; }

		[XmlElement("internalexception")]
		public ODataInternalException InternalException { get; set; }
	}
}