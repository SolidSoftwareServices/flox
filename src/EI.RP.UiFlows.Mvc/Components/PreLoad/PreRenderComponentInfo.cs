using System;

namespace EI.RP.UiFlows.Mvc.Components.PreLoad
{
	public class PreRenderComponentInfo<TId>
	{
		public PreRenderComponentInfo(TId componentId, Type componentType, object input)
		{
			ComponentId = componentId;
			ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
			Input = input;
		}
		/// <summary>
		/// the id of the component to be rendered
		/// </summary>
		public TId ComponentId { get; }
		public Type ComponentType { get; }
		public object Input { get; }

		public static implicit operator PreRenderComponentInfo<TId>(Tuple<TId,Type,object> input)
		{
			return new PreRenderComponentInfo<TId>(input.Item1,input.Item2,input.Item3);
		}
	}
}