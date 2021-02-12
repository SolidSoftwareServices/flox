using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.XPath;
using S3.CoreServices.Profiling;
using S3.TestServices.Profilers;

namespace S3.UI.TestServices.Html
{
	public static class HtmlDocumentExtensions
    {
		private static readonly Lazy<IBrowsingContext> Context=new Lazy<IBrowsingContext>(()=> BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithJs().WithXPath().WithCss()));

        public static async Task<IHtmlDocument> ParseDocumentAsync(this HttpResponseMessage response,
	        IProfiler profiler)
        {

            var contentTask = response.Content.ReadAsStringAsync();
			
            var document = await Context.Value
                .OpenAsync(ResponseFactory, CancellationToken.None);
            return (IHtmlDocument) document;

            void ResponseFactory(VirtualResponse htmlResponse)
            {
                htmlResponse
                    .Address(response.RequestMessage.RequestUri)
                    .Status(response.StatusCode);

                MapHeaders(response.Headers);
                MapHeaders(response.Content.Headers);

                htmlResponse.Content(contentTask.GetAwaiter().GetResult());

                void MapHeaders(HttpHeaders headers)
                {
                    foreach (var header in headers)
                    {
                        foreach (var value in header.Value)
                        {
                            htmlResponse.Header(header.Key, value);
                        }
                    }
                }
            }
        }


        public static IElement GetElementByText(this IHtmlDocument document, string text, bool caseSensitive = true)
        {
	        return document.Body.GetElementByText(text,caseSensitive);
        }


        public static IElement GetElementByText(this IHtmlElement element, string text,bool caseSensitive=true)
        {
	        var xpath = caseSensitive
		        ? $".//*[contains(text(),'{text}')]"
			        : $".//*[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'{text.ToLowerInvariant()}')]";

            return (IElement)element.SelectSingleNode(xpath);
        }

	}
}
