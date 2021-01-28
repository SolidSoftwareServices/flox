using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Html;
using NUnit.Framework;

namespace EI.RP.UI.TestServices.Sut
{
	public abstract class SutPage<TApp> : ISutPage where TApp: ISutApp
    {
		protected SutPage(TApp app)
		{
			App = app;
			Document = app.CurrentPage?.Document;
		}

		protected TApp App { get; }

		public IHtmlDocument Document { get; private set; }

		public bool TryParse(IHtmlDocument document)
		{
			using (App.Profiler.ProfileTest($"{GetType().Name}-{nameof(TryParse)}"))
			{
				Document = document;

				var isInPage = IsInPage();

				return isInPage;
			}
		}

		public virtual bool IsContainer() => false;

		protected abstract bool IsInPage();

		public void AssertTitle(string expected)
		{
			Assert.AreEqual(expected, Document.Title);
		}

		public async Task<ISutApp> ClickOnElementBySelector(string selector)
		{
			var element = Document.QuerySelector(selector);
			return await App.ClickOnElement(element);
		}

		public async Task<ISutApp> ClickOnElementByText(string text)
		{
			var element = Document.GetElementByText(text);
			return await App.ClickOnElement(element);
		}

		public async Task<ISutApp> ClickOnElementById(string id)
		{
			var element = Document.GetElementById(id);
			return await App.ClickOnElement(element);
		}

		public async Task<ISutApp> ClickOnElement(IElement clickable)
		{
			return await App.ClickOnElement(clickable);
		}
        public async Task<ISutApp> ExecuteEventOnInputChanged(IHtmlInputElement input)
        {
            //TODO: remove once Js is supported

            return await App.ClickOnElement(input);
        }
	}
}