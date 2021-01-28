using System.Xml.Serialization;

namespace EI.RP.DataServices.SAP.Clients.ErrorHandling
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
	public class ODataErrorMessage
	{
		[XmlAttribute(AttributeName = "lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Language { get; set; }

		[XmlText]
		public string Value { get; set; }
	}
}