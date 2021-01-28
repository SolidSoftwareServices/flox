using AngleSharp.Html.Dom;

namespace EI.RP.UI.TestServices.Sut
{
	public interface ISutPage
	{
		IHtmlDocument Document { get; }
		bool TryParse(IHtmlDocument document);

		bool IsContainer();
	}
}