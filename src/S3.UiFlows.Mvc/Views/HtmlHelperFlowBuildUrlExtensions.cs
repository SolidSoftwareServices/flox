using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;
using S3.Mvc.Core.Cryptography.AntiTampering;
using S3.Mvc.Core.Cryptography.Urls;
using S3.Mvc.Core.System;
using S3.Mvc.Core.Views;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using S3.UiFlows.Core.DataSources;

namespace S3.UiFlows.Mvc.Views
{

	public static class HtmlHelperFlowBuildUrlExtensions{

		/// <summary>
		/// Resolves a url that changes the view of the current step data without processing a transition
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="html"></param>
		/// <param name="model"></param>
		/// <param name="routeValues"></param>
		/// <param name="linkText"></param>
		/// <remarks>use for filtering, paging and other view customizations</remarks>
		/// <returns></returns>
		public static async Task<string> UiFlowUrlToCurrentStepAsync(this IHtmlHelper html, IUiFlowScreenModel model, object routeValues)
		{
			var controllerName = await html.ResolveModelControllerName(model);
			var modelRouteVals = await html.ResolveFlowActionRouteValues(model, nameof(IUiFlowController.Current));

			if (routeValues != null)
			{
				modelRouteVals = routeValues.MergeObjects(modelRouteVals);

			}

			return await html.GetActionUrlAsync(nameof(IUiFlowController.Current), controllerName, routeValues: modelRouteVals);
		}


		
		/// <param name="usableByClientCode">true if it needs to be visible as decrypted by client javascript</param>
		public static async Task<IHtmlContent> GetHiddenFlowStepMemberAsync<TModel>(this IHtmlHelper html,
			Expression<Func<TModel, object>> stepFieldExpression, string value, bool usableByClientCode)
			where TModel : UiFlowScreenModel
		{
			var propertyPath = stepFieldExpression.GetPropertyPath();
			return await html.GetHiddenFlowStepMemberAsync(propertyPath, value, usableByClientCode);
		}
		/// <param name="usableByClientCode">true if it needs to be visible as decrypted by client javascript</param>
		public static async Task<IHtmlContent> GetHiddenFlowStepMemberAsync(this IHtmlHelper html,
			string propertyPath, string value, bool usableByClientCode)
		{
			return await html.SecureHiddenAsync(propertyPath, usableByClientCode, value);
		}

		public static async Task<IHtmlContent> GetButtonToFlowStepAsync<TStepData>(this IHtmlHelper html,
			string flowType,
            IUiFlowScreenModel currentScreenModel,
			ScreenEvent eventToTrigger,
			string buttonContent,
			object attributes = null,
			object queryString = null,
			params (Expression<Func<TStepData, object>>, string)[] contextValuesToSetOnCurrentStepData)
			where TStepData : UiFlowScreenModel 
		{
			using (await html.BeginUiFlowFormAsync(currentScreenModel, queryString: queryString))
			{
				var content = html.ViewContext.FormContext.EndOfFormContent;
				foreach (var item in contextValuesToSetOnCurrentStepData)
				{
					content.Add(await html.GetHiddenFlowStepMemberAsync(item.Item1, item.Item2, false));

				}
				content.Add(BuildButton());

				return new HtmlString(string.Empty);
			}

			TagBuilder BuildButton()
			{
				var btn = new TagBuilder("button");

				if (attributes != null)
				{
					var attrs = (IDictionary<string, object>)attributes.ToDynamic();
					foreach (var key in attrs.Keys)
					{
						btn.Attributes.Add(key.TrimStart('@'), (string)attrs[key]);
					}
				}

				btn.Attributes.Add("name", SharedSymbol.FlowEventFormFieldName);
				btn.Attributes.Add("value", eventToTrigger.ToString());
				btn.InnerHtml.Append(buttonContent);
				return btn;
			}
		}




		internal static async Task<IHtmlContent> DisplayFlowLinkAsync2(this IHtmlHelper html,
			UiFlowScreenModel model,
			string linkText, string flow, object routeValues,
			object htmlAttributes)
		{
	        
			if (model == null)
			{
				return await html.StartNewFlowActionLink(linkText, flow, routeValues, htmlAttributes);
			}
			if (!(model is UiFlowScreenModel))
			{
				throw new ArgumentException();
			}

				var stepData = model as UiFlowScreenModel;
				
				if (stepData.Metadata.IsContainedInController())
				{
					return await BuildFlowContainerActionLink();
				}

				
				if (stepData.Metadata.IsFlowContainer)
				{
					return await BuildFlowInFlowContainerActionLink(stepData);
				}
				var uiFlowContext = await html.GetUiFlowContext(stepData.FlowHandler);
				if (uiFlowContext.IsInContainer())
				{
					return await BuildFlowInFlowContainerActionLink(stepData);
				}

				return await html.StartNewFlowActionLink(linkText, flow, routeValues, htmlAttributes);

			async Task<IHtmlContent> BuildFlowInFlowContainerActionLink(UiFlowScreenModel m)
			{
				if (m == null) throw new ArgumentNullException(nameof(m));
				var uiFlowContextData = await html.GetUiFlowContext(m.FlowHandler);

				var controllerName = await html.ResolveModelControllerName(model);

				object modelRouteVals = new UiFlowController.GetNewContainedViewRequest()
				{
					FlowHandler = m.Metadata.IsFlowContainer ? m.FlowHandler : uiFlowContextData.ContainerFlowHandler,
					NewContainedFlowType = flow.ToString()

				};
				if (routeValues != null)
				{
					modelRouteVals = routeValues.MergeObjects(modelRouteVals);
				}


				var newContainedViewName = nameof(IUiFlowController.NewContainedView);
				return await html.GetActionLinkAsync(linkText, newContainedViewName,
					controllerName,
					modelRouteVals, htmlAttributes);
			}



			async Task<IHtmlContent> BuildFlowContainerActionLink()
			{
				return await html.GetActionLinkAsync(linkText,
					"DisplayFlow",
					html.CurrentController(),
					new { flow = flow });
			}

		}

		private static async Task<IHtmlContent> StartNewFlowActionLink(this IHtmlHelper html,
			string linkText, string flow, object routeValues = null,object htmlAttributes=null)
		{

			return await html.GetActionLinkAsync(linkText, nameof(IUiFlowController.Init),flow,routeValues,htmlAttributes);
		}

		
		private static async Task<UiFlowContextData> GetUiFlowContext(this IHtmlHelper html, string flowHandler) 
		{
			return await html.ViewContext.HttpContext.Resolve<IUiFlowContextRepository>().Get(flowHandler);
		}

		/// <summary>
		/// It Builds an action that changes the view of the current step data without processing a transition<seealso cref="UiFlowUrlToCurrentStepAsync"/>
		/// </summary>
		/// <param name="html"></param>
		/// <param name="model"></param>
		/// <param name="routeValues"></param>
		/// <param name="linkText"></param>
		/// <remarks>use for filtering, paging and other view customizations</remarks>
		/// <returns></returns>
		internal static async Task<IHtmlContent> UiFlowActionLinkToCurrentStepAsync(this IHtmlHelper html,
			UiFlowScreenModel model, object routeValues,
			string linkText, object htmlAttributes = null)
		{
			var controllerName = await html.ResolveModelControllerName( model);
			var modelRouteVals = await html.ResolveFlowActionRouteValues(model,nameof(IUiFlowController.Current));

			if (routeValues != null)
			{
				modelRouteVals = routeValues.MergeObjects(modelRouteVals);
			}

			return await html.GetActionLinkAsync(linkText, nameof(IUiFlowController.Current), controllerName, modelRouteVals, htmlAttributes);
		}
		

		private static async Task<object> ResolveFlowActionRouteValues(this IHtmlHelper html, IUiFlowScreenModel model,string actionName)
		{
			var uiFlowContextData =await html.GetUiFlowContext(model.FlowHandler);

			switch (actionName)
			{
				case nameof(IUiFlowController.Current):
					if (uiFlowContextData.IsInContainer())
					{
						//when contained in flow
						var containerFlowHandler = uiFlowContextData.ContainerFlowHandler;
						var containerContext =await html.GetUiFlowContext(containerFlowHandler);
						return new UiFlowController.CurrentViewRequest {FlowHandler = containerContext.FlowHandler};
					}
					else
					{
						//when not contained in flow
						return new UiFlowController.CurrentViewRequest { FlowHandler = model.FlowHandler};
					}
				default: 
					throw new NotSupportedException(actionName);
			}


		}

		private static async Task<string> ResolveModelControllerName(this IHtmlHelper html, IUiFlowScreenModel model)
	        
		{
			var getControllerName
				= (await html.GetUiFlowContext(model.FlowHandler)).IsInContainer()
					? html.GetControllerWhenIsContainedInFlow(model)
					: html.GetControllerWhenIsNotContainedInFlow(model);
			var controllerName = await getControllerName;
			return controllerName;
		}

		private static async Task<string> GetControllerWhenIsNotContainedInFlow<TModel>(this IHtmlHelper html,TModel model) where TModel : IUiFlowScreenModel
		{
			return (await html.GetUiFlowContext(model.FlowHandler)).FlowType;
		}

		private static async Task<string> GetControllerWhenIsContainedInFlow<TModel>(this IHtmlHelper html,TModel model) where TModel : IUiFlowScreenModel
		{
			var containerFlowHandler = (await html.GetUiFlowContext(model.FlowHandler)).ContainerFlowHandler;
			var containerContext = await html.GetUiFlowContext(containerFlowHandler);

			return containerContext.FlowType;
		}


	}
}