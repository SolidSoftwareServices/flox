

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using EI.RP.CoreServices.EmbeddedResources;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Edmx
{
	partial class LegacySapEdmxResolver : IEdmxResolver
	{
		public IEdmModel Parse2(string edmxString)
		{
			var doc = new XmlDocument();
			doc.LoadXml(edmxString);
			using (var xmlReader =
				new XmlTextReader(EmbeddedResourceReader.ReadTextResourceAsStream("odatav2tov4.xsl")))
			{
				var transform = new XslTransform();
				transform.Load(xmlReader);
				using (var resultReader = transform.Transform(doc, null))
				{
					IEdmModel result;

					if (!CsdlReader.TryParse(resultReader, out result, out var errors))
					{
						var sb = new StringBuilder();
						foreach (var error in errors)
						{
							sb.Append(error.ErrorMessage).Append("\n");
						}

						throw new InvalidOperationException(sb.ToString());
					}

					return result;
				}
			}
		}
	}
}