using AngleSharp.Html.Dom;

namespace S3.UI.TestServices.Sut
{
	public interface ISutPage
	{
		IHtmlDocument Document { get; }
		bool TryParse(IHtmlDocument document);

		bool IsContainer();
	}
}