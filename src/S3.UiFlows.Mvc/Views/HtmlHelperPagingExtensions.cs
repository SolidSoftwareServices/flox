using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace S3.UiFlows.Mvc.Views
{
	public static class HtmlHelperPagingExtensions
	{
		public static HtmlString Pager(this HtmlHelper helper, int currentPageIndex, Func<int, string> action,
			int totalItems, int pageSize, int numberOfLinks)
		{
			if (totalItems <= 0 || totalItems <= pageSize) return HtmlString.Empty;
			var totalPages = (int) Math.Ceiling(totalItems / (double) pageSize);
			var lastPageNumber = (int) Math.Ceiling((double) currentPageIndex / numberOfLinks) * numberOfLinks;
			var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
			var hasPreviousPage = currentPageIndex > 1;
			var hasNextPage = currentPageIndex < totalPages;
			if (lastPageNumber > totalPages) lastPageNumber = totalPages;
			var ul = new TagBuilder("ul");

			ul.AddCssClass("");
			ul.InnerHtml.Append(AddLink(1, action, currentPageIndex == 1, "fa", "<<", "First Page").ToString());
			ul.InnerHtml.Append(AddLink(currentPageIndex - 1, action, !hasPreviousPage, "fa", "<", "Previous Page")
				.ToString());
			for (var i = firstPageNumber; i <= lastPageNumber; i++)
				ul.InnerHtml.Append(
					AddLink(i, action, i == currentPageIndex, "", i.ToString(), i.ToString()).ToString());

			ul.InnerHtml.Append(AddLink(currentPageIndex + 1, action, !hasNextPage, "fa", ">", "Next Page").ToString());
			ul.InnerHtml.Append(AddLink(totalPages, action, currentPageIndex == totalPages, "fa", ">>", "Last Page")
				.ToString());

			return new HtmlString(ul.ToString());
		}

		private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd,
			string linkText, string tooltip)
		{
			var li = new TagBuilder("li");
			li.MergeAttribute("title", tooltip);
			if (condition) li.AddCssClass(classToAdd);
			var a = new TagBuilder("a");
			a.MergeAttribute("href", !condition ? action(index) : "javascript:");
			if (classToAdd != "fa" && condition) a.AddCssClass("selected");
			a.InnerHtml.Append(linkText);
			li.InnerHtml.Append(a.ToString());
			return li;
		}
	}
}