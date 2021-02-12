using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Autofac;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NLog;
using NUnit.Framework;
using S3.TestServices.Logging;
using S3.TestServices.Profilers;
using S3.UI.TestServices.Html;
using S3.UI.TestServices.Http;
using S3.UI.TestServices.Services;

namespace S3.UI.TestServices.Sut
{
	public abstract class SutAppBase<TApp, TStartUp> : ISutApp
		where TApp : class, ISutApp
		where TStartUp : class
	{
		protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		public ConsoleProfiler Profiler { get; private set; }
		public PagesResolver<TApp> PagesResolver { get; }
		private AppFactory<TStartUp> Factory { get; set; }
		public Lazy<TestHttpClient> Client { get; private set; }


		
		static SutAppBase()
		{
			var minLogLevel = LogLevel.Info;
#if FrameworkDeveloper || DEBUG
			minLogLevel = LogLevel.Trace;
#endif

			TestLogging.Default.ConfigureLogging(@"${time} - ${logger} - ${level} - ${message} - ${exception} - ${stackTrace}",minLogLevel);
		}
		protected SutAppBase()
		{
			Profiler = new ConsoleProfiler(25);
			PagesResolver = new PagesResolver<TApp>(this as TApp);
			Factory = new AppFactory<TStartUp>();
			Client = new Lazy<TestHttpClient>(CreateClient);
		}

		

		private TestHttpClient CreateClient()
		{
			DisposeClient();
			var client = OnCreateClient(Factory);
			
			return new TestHttpClient(client,Profiler);
		}

		protected virtual void Reset()
		{
			Client.Value.Reset();
			Profiler.Flush();

		}
		public virtual void Release()
		{
		}

		public virtual void Dispose()
		{
			
			DisposeClient();
			DisposeFactory();
			Profiler.Dispose();
		}


		private void DisposeFactory()
		{
			if (Factory != null)
			{
				Factory.Dispose();
				Factory = null;
			}
		}

		protected virtual HttpClient OnCreateClient(AppFactory<TStartUp> factory)
		{
			var result = factory
				.WithWebHostBuilder(b =>
				{
					b.ConfigureTestContainer<ContainerBuilder>(
						cb => { });
				})
				.CreateClient(new WebApplicationFactoryClientOptions
				{
					AllowAutoRedirect = true
				});

			return result;
		}

		private void DisposeClient()
		{
			if (Client.IsValueCreated)
			{
				Client.Value.Dispose();
				Client = new Lazy<TestHttpClient>(CreateClient);
			}
		}

		public async Task<ISutApp> ClickOnElementByText(string byText)
		{
			await ClickOnElement((CurrentPage.Document.GetElementByText(byText)));
			return this as TApp;
		}
		public async Task<ISutApp> ClickOnElementById(string id)
		{
			await ClickOnElement((CurrentPage.Document.GetElementById(id)));
			return this as TApp;
		}

		public async Task<ISutApp> SelectOnElement(IHtmlOptionElement selectable)
		{
			if (selectable == null) throw new ArgumentNullException(nameof(selectable));

			var type = selectable.GetType();
			if(selectable.Value == null) throw new NotImplementedException($"selectable {type.FullName} is not supported.");

            
			return this as TApp;
		}

		public async Task<ISutApp> ClickOnElement(IElement clickable)
		{
			if (clickable == null) throw new ArgumentNullException(nameof(clickable));

			var type = clickable.GetType();
			Task<HttpResponseMessage> responseTask = null;
			if (clickable is IHtmlAnchorElement anchorElement)
			{
				if (anchorElement.Attributes["data-trigger-event"] != null)
				{
					var eventFieldName = anchorElement.GetAttribute("data-event-field-name");
					var triggerEvent = anchorElement.GetAttribute("data-trigger-event");
					var form = anchorElement.Closest("form");

					var eventField = form.QuerySelector($"[name='{eventFieldName}']") as IHtmlInputElement;
					eventField.Value = triggerEvent;

					return await PostFormAsync(eventField);
				}

				responseTask = Client.Value.GetAsync(anchorElement.Href);
			}
			else if (clickable is IHtmlButtonElement || clickable is IHtmlInputElement)
			{
				return await PostFormAsync(clickable);
			}

			if (responseTask == null) throw new NotImplementedException($"clickable {type.FullName} is not supported.");


			await UpdateCurrentPageFromMessage(await responseTask);
			return this as TApp;
		}

		public async Task<TApp> ToUrl(string relativeUrl)
		{
			using (Profiler.ProfileTest(nameof(ToUrl)))
			{
				var message = await Client.Value.GetAsync(relativeUrl);
				await UpdateCurrentPageFromMessage(message);
				return this as TApp;
			}
		}
		public async Task<ISutApp> BrowsePrevious()
		{
			using (Profiler.ProfileTest(nameof(ToUrl)))
			{
				var message = await Client.Value.ToPreviousUrl();
				await UpdateCurrentPageFromMessage(message);
				return this as TApp;
			}
		}

		public async Task<ISutApp> BrowseForward()
		{
			using (Profiler.ProfileTest(nameof(ToUrl)))
			{
				var message = await Client.Value.ToNextUrl();
				await UpdateCurrentPageFromMessage(message);
				return this as TApp;
			}
		}
		public async Task<ISutApp> ReloadCurrentPage()
		{
			using (Profiler.ProfileTest(nameof(ToUrl)))
			{
				var message = await Client.Value.GetAsync(Client.Value.CurrentUrl);
				await UpdateCurrentPageFromMessage(message);
				return this as TApp;
			}
		}

		private async Task UpdateCurrentPageFromMessage(HttpResponseMessage message)
		{
			using (Profiler.ProfileTest(nameof(UpdateCurrentPageFromMessage)))
			{
				Assert.IsTrue(message.IsSuccessStatusCode, $"Server returned:{message.StatusCode}");
				CurrentPage = await PagesResolver.ResolvePage(message);
				OnNewPageLoaded();
			}
		}

		public event Action<ISutApp,ISutPage> NewPageLoaded;

		private void OnNewPageLoaded()
		{
			NewPageLoaded?.Invoke(this, CurrentPage);
		}

		public ISutPage CurrentPage { get; private set; }

		public TResult CurrentPageAs<TResult>() where TResult : ISutPage
		{
			if (CurrentPage == null)
			{
				throw new InvalidOperationException(
					"No navigation has been performed yet. There is not a current page yet");
			}

			if (!(CurrentPage is TResult))
			{
				throw new InvalidCastException(
					$"Requested Current page as {typeof(TResult).FullName} and it is actually {CurrentPage.GetType().FullName}");
			}

			return (TResult) CurrentPage;
		}
		public AppOptions Options { get; }=new AppOptions();

		private async Task<ISutApp> PostFormAsync(
			IElement submitButton)
		{
			if (Options.OnlyCurrentPageCanPost && submitButton?.GetRoot() != CurrentPage.Document?.GetRoot())
			{
				throw new InvalidOperationException("The button is from another document. You are clicking on an element taken from a page that is different than the current being shown.");
			}
			using (Profiler.ProfileTest(nameof(PostFormAsync)))
			{
				var responseMessage = await Client.Value.SendAsync(submitButton);
				await UpdateCurrentPageFromMessage(responseMessage);
				return this as TApp;
			}
		}

	}
}