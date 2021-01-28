using System;
using System.Dynamic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Views;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EI.RP.WebApp.Infrastructure.Extensions
{
	public static class HtmlHelperPagingExtensions
    {
        public static async Task<HtmlString> DisplayContainerFlowLinkPaging< TFlowType>(this IHtmlHelper html, 
            string linkText, IUiFlowScreenModel model, TFlowType flow, int currentPageIndex, int totalPages, int pageSize, int numberOfLinks, 
            Func<int, string> action, object routeValues = null)
            where TFlowType : struct
        {
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }
            var ul = new TagBuilder("ul");

            ul.AddCssClass("");

            ul.InnerHtml.AppendHtml(await AddLink(html, model, 1, currentPageIndex == 1, "fa", "<<", "First Page"));
            ul.InnerHtml.AppendHtml(await AddLink(html, model, currentPageIndex - 1,
                !hasPreviousPage, "fa", "<", "Previous Page"));

            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml.AppendHtml(await AddLink(html, model, i,
                    i == currentPageIndex, "", i.ToString(), i.ToString()));
            }

            ul.InnerHtml.AppendHtml(await AddLink(html, model, currentPageIndex + 1, !hasNextPage, "fa", ">", "Next Page"));
            ul.InnerHtml.AppendHtml(await AddLink(html, model, totalPages, currentPageIndex == totalPages, "fa", ">>", "Last Page"));

            return new HtmlString(GetString(ul));

            string GetString(IHtmlContent content)
            {
	            using (var writer = new StringWriter())
	            {
		            content.WriteTo(writer, HtmlEncoder.Default);
		            return writer.ToString();
	            }
            }
		}

        private static async Task<TagBuilder> AddLink(this IHtmlHelper html, IUiFlowScreenModel model, int index, bool condition, string classToAdd, string linkText, string tooltip)
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
            if (condition)
            {
                li.AddCssClass(classToAdd);
            }
            var a = new TagBuilder("a");
            a.MergeAttribute("href", !condition ? await html.UiFlowUrlToCurrentStepAsync(model, new { pageindex = index }) : "javascript:");
            if (classToAdd != "fa" && condition)
            {
                a.AddCssClass("selected");
            }

            a.InnerHtml.Append(linkText);
            li.InnerHtml.AppendHtml(a);
            return li;
        }

        public static async Task<HtmlString> DisplaySmartContainerFlowLinkPaging(this IHtmlHelper html, IUiFlowScreenModel model, 
            int currentPageIndex, int totalPages, int numberOfLinks, object routeValues = null, string id = null, string focusOnElement = null)
        {
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;

            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }

            var nav = new TagBuilder("nav");
            nav.GenerateId(id ?? "pagination", "_");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            ul.InnerHtml.AppendHtml(
                await AddSmartLink(html, focusOnElement ?? nav.Attributes["id"], model, currentPageIndex - 1, currentPageIndex, totalPages, "&laquo;", isPrevious: true, routeValues: routeValues));

            for (var i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml.AppendHtml(
                    await AddSmartLink(html, focusOnElement ?? nav.Attributes["id"], model, i, currentPageIndex, totalPages, i.ToString(), routeValues: routeValues));
            }

            ul.InnerHtml.AppendHtml(
                await AddSmartLink(html, focusOnElement ?? nav.Attributes["id"], model, currentPageIndex + 1, currentPageIndex, totalPages, "&raquo;", isNext: true, routeValues: routeValues));

            nav.InnerHtml.AppendHtml(ul);

            return new HtmlString(GetString(nav));

            string GetString(IHtmlContent content)
            {
                using (var writer = new StringWriter())
                {
                    content.WriteTo(writer, HtmlEncoder.Default);
                    return writer.ToString();
                }
            }
        }

        private static async Task<TagBuilder> AddSmartLink(this IHtmlHelper html, string parentId, IUiFlowScreenModel model, int index, int currentIndex,
            int totalPages, string content, bool isPrevious = false, bool isNext = false, object routeValues = null)
        {
            var isCurrent = index == currentIndex;

            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            var isPreviousOrNextAndDisabled = (isPrevious && currentIndex == 1) || (isNext && currentIndex == totalPages);

            if (isPreviousOrNextAndDisabled)
            {
                li.AddCssClass("disabled");
            }
            else if (index == currentIndex)
            {
                li.AddCssClass("active");
            }

            object newRouteValues = new { pageindex = index };
            if (routeValues != null)
            {
                newRouteValues = newRouteValues.MergeObjects(routeValues);
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            a.MergeAttribute("aria-disabled", isPreviousOrNextAndDisabled ? "true" : "false");
            a.MergeAttribute("href", isCurrent ? $"#{ parentId }" : $"{ await html.UiFlowUrlToCurrentStepAsync(model, newRouteValues) }#{ parentId }");

            if (isPrevious || isNext)
            {
                var span = new TagBuilder("span");
                span.MergeAttribute("aria-hidden", "true");
                span.InnerHtml.Append(content);

                a.MergeAttribute("aria-label", isPrevious ? "Previous page" : "Next page");
                a.InnerHtml.AppendHtml(span);
            }
            else
            {
                a.InnerHtml.Append(content);

                if (isCurrent)
                {
                    var span = new TagBuilder("span");
                    span.AddCssClass("sr-only");
                    span.InnerHtml.Append("(current)");
                    a.InnerHtml.AppendHtml(span);
                }
            }

            li.InnerHtml.AppendHtml(a);

            return li;
        }
    }
}