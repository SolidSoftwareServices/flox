using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.Mvc.Core.Cryptography.AntiTampering;
using S3.Mvc.Core.Profiler;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace S3.UiFlows.Mvc.Views
{

	public static class HtmlHelperFlowFormExtensions
	{
		/// <summary>
		/// It builds a form tag that POSTS an Step Event of a Flow
		/// </summary>
		/// <typeparam name="TFlowType"></typeparam>
		/// <param name="html"></param>
		/// <param name="model"></param>
		/// <param name="htmlId"></param>
		/// <param name="queryString"></param>
		/// <param name="className"></param>
		/// <param name="defaultEvent"></param>
		/// <returns></returns>
		public static async Task<MvcForm> BeginUiFlowFormAsync(this IHtmlHelper html,
			IUiFlowScreenModel model, string htmlId = "", object queryString = null, string className = "",
			ScreenEvent defaultEvent = null) 
		{

			var flowType = model.GetFlowType();
			var concreteModel = (UiFlowScreenModel) model;


			MvcForm result;
			var flowName =  flowType.ToString();
			var routeValues = (object) new {flowType = flowName};
			if (queryString != null)
			{
				routeValues = routeValues.MergeObjects(queryString);
			}

			var controllerName = string.IsNullOrEmpty(concreteModel.Metadata.ContainerController)
				? flowName
				: concreteModel.Metadata.ContainerController;



			result = html.BeginForm(nameof(IUiFlowController.OnEvent), controllerName, routeValues, FormMethod.Post,
				true,
				htmlAttributes: new {@id = $"{flowName}-{htmlId}", @class = className});

			var content = html.ViewContext.FormContext.EndOfFormContent;
			var contentItems = new List<Task<HtmlString>>();


			if (defaultEvent != null)
			{
				content.Add(html.Hidden(SharedSymbol.FlowEventFormFieldName, defaultEvent));
			}

			contentItems.Add(html.SecureHiddenAsync(UiFlowController.FlowFormKey, false,
				Guid.NewGuid().ToString()));
			contentItems.Add(
				html.SecureHiddenAsync(nameof(UiFlowScreenModel.FlowHandler), false, model.FlowHandler));
			contentItems.Add(html.SecureHiddenAsync(nameof(UiFlowScreenModel.FlowScreenName), false,
				model.FlowScreenName));
			contentItems.Add(html.SecureHiddenAsync(nameof(UiFlowScreenModel.ScreenTitle), false,
				model.ScreenTitle));
			foreach (var modelContainerProperty in concreteModel.Metadata.ContainerProperties)
			{
				if (modelContainerProperty.Value is string value)
				{
					contentItems.Add(html.SecureHiddenAsync(modelContainerProperty.Key, false, value));
				}
			}

			foreach (var contentItem in contentItems)
			{
				content.Add(await contentItem);
			}

			return result;
		}
	}
}