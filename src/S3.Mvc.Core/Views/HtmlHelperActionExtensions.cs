using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using S3.Mvc.Core.Controllers;
using S3.Mvc.Core.System;

namespace S3.Mvc.Core.Views
{

	public static class HtmlHelperActionExtensions
	{

		/// <returns>A new <see cref="T:Microsoft.AspNetCore.Html.IHtmlContent" /> containing the anchor element.</returns>
		public static IHtmlContent ActionLink(
			this IHtmlHelper helper,
			string linkText,
			string actionName,
			Type controller,
			object routeValues = null,

			object htmlAttributes = null)
		{
			if (helper == null)
				throw new ArgumentNullException(nameof(helper));
			if (linkText == null)
				throw new ArgumentNullException(nameof(linkText));
			if (controller != null && !typeof(Controller).IsAssignableFrom(controller))
				throw new ArgumentException("Not a valid controller");
			return helper.ActionLink(linkText, actionName, controller.GetNameWithoutSuffix(), (string) null,
				(string) null, (string) null, routeValues, htmlAttributes);
		}

		public static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action,
			object parameters = null)
		{
			var controller = (string) helper.ViewContext.RouteData.Values["controller"];
			return await RenderActionAsync(helper, action, controller, parameters);
		}

		public static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action,
			string controller, object parameters = null)
		{
			var area = (string) helper.ViewContext.RouteData.Values["area"];
			return await RenderActionAsync(helper, action, controller, area, parameters);
		}

		private static readonly List<HttpContext> Ctx = new List<HttpContext>();

		public static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action,
			string controller, string area, object parameters = null)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(controller));
			if (controller == null)
				throw new ArgumentNullException(nameof(action));


			var currentHttpContext = helper.ViewContext.HttpContext;
			Ctx.Add(currentHttpContext);
			var httpContextFactory = currentHttpContext.Resolve<IHttpContextFactory>();
			var actionInvokerFactory = currentHttpContext.Resolve<IActionInvokerFactory>();
			var actionSelector = currentHttpContext.Resolve<IActionDescriptorCollectionProvider>();

			// creating new action invocation context
			var routeData = new RouteData();
			var routeParams = new RouteValueDictionary(parameters ?? new { });
			var routeValues = new RouteValueDictionary(new {area, controller, action});
			var newHttpContext = httpContextFactory.Create(currentHttpContext.Features);
			if (currentHttpContext.User == null)
			{ }

			if (newHttpContext.User == null)
			{
				newHttpContext.User = currentHttpContext.User;
			}

			var streamCreator = false;
			if (!(newHttpContext.Response.Body is MemoryStream))
			{
				newHttpContext.Response.Body = new MemoryStream();
				streamCreator = true;
			}

			foreach (var router in helper.ViewContext.RouteData.Routers)
				routeData.PushState(router, null, null);

			routeData.PushState(null, routeValues, null);
			routeData.PushState(null, routeParams, null);

			var actionDescriptor = actionSelector.ActionDescriptors.Items.First(i =>
				i.RouteValues["Controller"] == controller && i.RouteValues["Action"] == action);
			var actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);

			var invoker = actionInvokerFactory.CreateInvoker(actionContext);
			string content = null;
			await invoker.InvokeAsync().ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					content = task.Exception.Message;
				}
				else if (task.IsCompleted)
				{
					if (streamCreator)
					{
						content = ReadContent();
					}
					else
					{
						newHttpContext.Response.Body.Position = 0;
						var body = ReadContent();
					}
				}
			});

			return new HtmlString(content);

			string ReadContent()
			{
				using (var a = new MemoryStream())
				{
					var bodyPosition = newHttpContext.Response.Body.Position;
					newHttpContext.Response.Body.Position = 0;
					newHttpContext.Response.Body.CopyTo(a);
					newHttpContext.Response.Body.Position = bodyPosition;
					a.Position = 0;
					using (var reader = new StreamReader(a))
					{
						content = reader.ReadToEnd();
					}
				}

				return content;
			}
		}

		public static string GetString(this IHtmlContent content)
		{
			using (var stringWriter = new StringWriter())
			{
				content.WriteTo(stringWriter, HtmlEncoder.Default);
				return stringWriter.ToString();
			}
		}

		public static IHtmlContent ToHtmlText(this string src, IHtmlHelper html)
		{
			var htmlNewLine = "<br/>";
			return html.Raw(src
				.Replace(Environment.NewLine, htmlNewLine)
				.Replace("\n", htmlNewLine)
				.Replace("\r", htmlNewLine));
		}
	}
}