using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using S3.UI.TestServices.Html;
using S3.UI.TestServices.Sut;
using NUnit.Framework;

namespace S3.UI.TestServices.Services
{
	public class PagesResolver<TApp> where TApp : ISutApp
	{
		private TApp App { get; }


		public PagesResolver(TApp app)
		{
			App = app;
			
			_pagesLazy = new Lazy<IEnumerable<ISutPage>>(() =>
				typeof(TApp).Assembly.GetTypes()
					.Where(x => !x.IsAbstract && typeof(SutPage<TApp>).IsAssignableFrom(x))
					.Select(x => { return (ISutPage) Activator.CreateInstance(x, new object[] {app}); }));
		}

		private readonly Lazy<IEnumerable<ISutPage>> _pagesLazy;

		public async Task<ISutPage> ResolvePage(HttpResponseMessage message)
		{
			
			using (App.Profiler.RecordStep($"{GetType().Name}.{nameof(ResolvePage)}"))
			{
				Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
				IHtmlDocument doc;
				using (App.Profiler.RecordStep($"{GetType().Name}.{nameof(HtmlDocumentExtensions.ParseDocumentAsync)}"))
				{
					doc = await message.ParseDocumentAsync(App.Profiler);
				}

				using (App.Profiler.RecordStep("DoResolve"))
				{
					ISutPage result = null;


					result = Resolve(doc);
					if (result == null)
					{
						throw new NotImplementedException(
							$"Missing parser for current page: {Environment.NewLine}{doc.Body.InnerHtml}");
					}

					return result;
				}
			}

			ISutPage Resolve(IHtmlDocument htmlDocument)
			{
				// ReSharper disable once ConvertToLocalFunction
				Func<ISutPage, bool> predicate = x => x.TryParse(htmlDocument);
				ISutPage sutPage=null;

#if DEBUG
				var candidates = _pagesLazy.Value.Where(predicate).ToArray();
				if (candidates.Length == 1)
				{
					sutPage = candidates.Single();
				}
				else if (candidates.Length > 1)
				{
					var items = candidates.Where(x => x.IsContainer()).ToArray();
					if (items.Length > 1)
					{
						throw new InvalidOperationException(
							$"found {candidates.Length} pages being containers {string.Join(',', items.Select(x => x.GetType().FullName))} container items on: {Environment.NewLine}{htmlDocument.Body.InnerHtml}");
					}

					if (items.Length == 0)
					{
						throw new InvalidOperationException(
							$"found {candidates.Length} pages but none of the is a container items on: {Environment.NewLine}{htmlDocument.Body.InnerHtml}");
					}


					sutPage = items.Single();
				}
#else
				sutPage=_pagesLazy.Value.FirstOrDefault(predicate);
#endif
				return sutPage;
			}

		}
	}
}