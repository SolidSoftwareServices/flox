using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace S3.UiFlows.Mvc.Components.PreLoad
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public class ComponentPreLoader<TId> 
	{
		private readonly IViewComponentHelper _componentHelper;
		private readonly IUiFlowScreenModel _screenModel;
		private readonly Dictionary<TId, Task<IHtmlContent>> _componentInvocations=new Dictionary<TId, Task<IHtmlContent>>();

		public ComponentPreLoader(IViewComponentHelper componentHelper,
			IUiFlowScreenModel screenModel)
		{
			_componentHelper = componentHelper;
			_screenModel = screenModel;
		}
		/// <summary>
		/// Starts loading a component without blocking the thread
		/// </summary>
		public void StartLoading(PreRenderComponentInfo<TId> componentInfo)
		{
			var invokeAsync= _componentHelper.InvokeAsync(componentInfo.ComponentType,
				new {input = componentInfo.Input, screenModel = _screenModel});
			_componentInvocations.Add(componentInfo.ComponentId, invokeAsync);
		}
		/// <summary>
		/// Starts loading a component without blocking the thread
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="componentId"></param>
		/// <param name="input"></param>
		public void StartLoading<TComponent>(TId componentId,object input) where TComponent:ViewComponent
		{
			StartLoading(new PreRenderComponentInfo<TId> (componentId,typeof(TComponent),input));
		}
		/// <summary>
		/// Starts loading a component without blocking the thread
		/// </summary>
		public void StartLoading<TComponent>(TId componentId) where TComponent : ViewComponent
		{
			StartLoading(new PreRenderComponentInfo<TId>(componentId, typeof(TComponent), null));
		}
		/// <summary>
		/// Starts loading a component without blocking the thread
		/// </summary>
		public void StartLoading(Tuple<TId, Type, object> componentInfoAsTuple)
		{
			StartLoading((PreRenderComponentInfo<TId>)componentInfoAsTuple);
		}
		

		public Task<IHtmlContent> Render(TId componentId)
		{
			return _componentInvocations[componentId];
		}
	
	}
}