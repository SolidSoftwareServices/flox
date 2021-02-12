using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using S3.TestServices.Profilers;
using HttpMethod = System.Net.Http.HttpMethod;

namespace S3.UI.TestServices.Http
{
	public class TestHttpClient : IDisposable
	{
		private readonly HttpClient _client;
		private readonly ConsoleProfiler _profiler;
		private UrlTracker UrlHistory { get; } = new UrlTracker();
		public string CurrentUrl => UrlHistory.GetCurrent();

		public TestHttpClient(HttpClient client, ConsoleProfiler profiler)
		{
			this._client = client;
			_profiler = profiler;
		}

		private async Task<HttpResponseMessage> ExecuteAndRecordUrl(Func<Task<HttpResponseMessage>> payload)
		{
			var responseMessage = await payload();
			UrlHistory.Record(responseMessage);

			return responseMessage;
		}

		public async Task<HttpResponseMessage> ToPreviousUrl()
		{
			using (_profiler.ProfileTest(nameof(ToPreviousUrl)))
			{
				UrlHistory.ToPrevious();
				var requestUri = UrlHistory.GetCurrent();
				//already recorded
				return await _client.GetAsync(requestUri);
			}
		}

		public async Task<HttpResponseMessage> ToNextUrl()
		{
			using (_profiler.ProfileTest(nameof(ToNextUrl)))
			{
				UrlHistory.ToNext();
				var requestUri = UrlHistory.GetCurrent();

				//already recorded
				return await _client.GetAsync(requestUri);
			}
		}

		public async Task<HttpResponseMessage> GetAsync(string uri)
		{
			using (_profiler.ProfileTest(nameof(GetAsync) + uri))
			{
				return await ExecuteAndRecordUrl(() => _client.GetAsync(uri));
			}
		}


		public async Task<HttpResponseMessage> PostAsync(string uri, StringContent value)
		{
			using (_profiler.ProfileTest(nameof(PostAsync) + uri))
			{
				return await ExecuteAndRecordUrl(() => _client.PostAsync(uri, value));
			}
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
		{


			return await ExecuteAndRecordUrl(() => _client.SendAsync(requestMessage));

		}

		public async Task<HttpResponseMessage> SendAsync(
			IElement submitButton)
		{
			return await SendAsync(submitButton.ResolveParentForm(), submitButton);

		}

		public async Task<HttpResponseMessage> SendAsync(

			IHtmlFormElement form,
			IElement submitButton)
		{
			return await SendAsync(form, submitButton, new KeyValuePair<string, string>[0]);
		}

		public async Task<HttpResponseMessage> SendAsync(

			IHtmlFormElement form,
			params (string, string)[] formValues)
		{
			return await SendAsync(form, formValues.ToDictionary(x => x.Item1, x => x.Item2));
		}


		public async Task<HttpResponseMessage> SendAsync(

			IHtmlFormElement form,
			IEnumerable<KeyValuePair<string, string>> formValues)
		{
			var submitElement = form.QuerySelectorAll("[type=submit]").Single();
			var submitButton = (IHtmlElement) submitElement;

			return await SendAsync(form, submitButton, formValues);
		}

		public async Task<HttpResponseMessage> SendAsync(

			IElement submitButton,
			params (string, string)[] formValues)
		{
			return await SendAsync(submitButton, formValues.ToDictionary(x => x.Item1, x => x.Item2));
		}

		public async Task<HttpResponseMessage> SendAsync(

			IElement submitButton,
			IEnumerable<KeyValuePair<string, string>> formValues)
		{
			return await SendAsync(submitButton.ResolveParentForm(), submitButton, formValues);
		}

		public async Task<HttpResponseMessage> SendAsync(

			IHtmlFormElement form,
			IElement submitButton,
			params (string, string)[] formValues)
		{
			return await SendAsync(form, submitButton, formValues.ToDictionary(x => x.Item1, x => x.Item2));
		}

		public async Task<HttpResponseMessage> SendAsync(

			IHtmlFormElement form,
			IElement submitButton,
			IEnumerable<KeyValuePair<string, string>> formValues)
		{
			using (_profiler.ProfileTest(nameof(SendAsync)))
			{

				foreach (var kvp in formValues)
				{
					var element = form[kvp.Key];
					if (element is IHtmlInputElement inputElement)
						inputElement.Value = kvp.Value;
					else if (element is IHtmlButtonElement buttonElement)
						buttonElement.Value = kvp.Value;
					else
						throw new NotSupportedException();

				}

				ValidateForm();


				var submit = BuildSubmission();

				var target = (Uri) submit.Target;
				if (submitButton.HasAttribute("formaction"))
				{
					var formaction = submitButton.GetAttribute("formaction");
					target = new Uri(formaction, UriKind.Relative);
				}

				var submission = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), target)
				{
					Content = new StreamContent(submit.Body)
				};

				foreach (var header in submit.Headers)
				{
					submission.Headers.TryAddWithoutValidation(header.Key, header.Value);
					submission.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}

				return await ExecuteAndRecordUrl(() => _client.SendAsync(submission));
			}

			DocumentRequest BuildSubmission()
			{

				var result = form.GetSubmission((IHtmlElement) submitButton);
				string name;
				string value;
				if (submitButton is IHtmlInputElement input)
				{
					name = input.Name;
					value = input.Value;
				}
				else if (submitButton is IHtmlButtonElement buttonElement)
				{
					name = buttonElement.Name;
					value = buttonElement.Value;
				}
				else
				{
					throw new NotSupportedException();
				}

				var bodyString = ASCIIEncoding.ASCII.GetString(((MemoryStream) result.Body).ToArray());
				var collection = bodyString.Split('&').Select(
					x =>
					{
						var parts = x.Split('=');
						return new KeyValuePair<string, string>(parts[0], parts[1]);
					}).GroupBy(x => x.Key).Select(x => x.FirstOrDefault()).ToDictionary(x => x.Key, x => x.Value);

				if (name != null && collection.ContainsKey(name))
				{
					collection[name] = value;
				}


				var newBody = string.Join('&', collection.Select(x => $"{x.Key}={x.Value}"));
				result.Body = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(newBody));
				return result;
			}

			void ValidateForm()
			{
				var htmlElements = form.Elements.Where(x => x is IHtmlInputElement).Cast<IHtmlInputElement>().ToArray();
				foreach (var htmlElement in htmlElements)
				{
					if (htmlElement is IHtmlInputElement inputElement && inputElement.WillValidate &&
					    !inputElement.CheckValidity())
					{
						throw new InvalidOperationException(
							$"The form is not valid due to element with Id?: {inputElement.Id}");
					}
				}
			}
		}

		public void Dispose()
		{
			_client?.Dispose();
			Reset();
		}


		public void Reset()
		{
			UrlHistory.Reset();
		}
	}
}