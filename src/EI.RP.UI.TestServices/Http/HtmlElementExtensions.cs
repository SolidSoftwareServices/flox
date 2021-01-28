using System;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace EI.RP.UI.TestServices.Http
{
	public static class HtmlElementExtensions
	{
		public static IHtmlFormElement ResolveParentForm(this IElement element,bool failIfNotFound=true)
		{
			IElement currentElement = element;
			do
			{
				currentElement = currentElement.ParentElement;
			} while (currentElement!=null &&!(currentElement is IHtmlFormElement));

			var result = currentElement as IHtmlFormElement;
			if (failIfNotFound && result == null)
			{
				throw new ArgumentException("Could not find parent form");
			}
			return result;
		}

		public static bool IsVisibleError(this IHtmlElement htmlElement)
		{
			return !htmlElement.GetStyle().CssText.Contains("display:none");
		}

		public static IHtmlElement GetElementById(this IElement element,string id)
		{
			return (IHtmlElement)element.QuerySelector($"[id='{id}']");
		}
	}
}