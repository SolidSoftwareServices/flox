using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace S3.CoreServices.Serialization
{
	public static class XmlExtensions
	{
		public static string ToXml<T>(this T input)
		{
			using (var writer = new StringWriter())
			{ 
				input.ToXml(writer);
				return writer.ToString();
			}
		}
		private static void ToXml<T>(this T objectToSerialize, Stream stream)
		{
			new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
		}

		private static void ToXml<T>(this T objectToSerialize, StringWriter writer)
		{
			new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
		}

		public static TObject XmlToObject<TObject>(this string xml)
		{
			using (var strReader = new StringReader(xml))
			{
				var serializer = new XmlSerializer(typeof(TObject));
				using (var xmlReader = new XmlTextReader(strReader))
				{
					return (TObject) serializer.Deserialize(xmlReader);
				}
			}
		}

		public static string XmlPrettyPrint(this string xml)
		{
			var stringBuilder = new StringBuilder();

			var element = XElement.Parse(xml);

			var settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.NewLineOnAttributes = true;

			using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
			{
				element.Save(xmlWriter);
			}

			return stringBuilder.ToString();
		}
	}
}