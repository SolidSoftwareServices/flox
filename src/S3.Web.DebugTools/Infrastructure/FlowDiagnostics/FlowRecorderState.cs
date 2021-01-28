using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using S3.CoreServices.Serialization;
using S3.UiFlows.Core.Infrastructure.DataSources;
using ObjectsComparer;


namespace S3.Web.DebugTools.Infrastructure.FlowDiagnostics
{
	public class FlowRecorderState
	{
		private IEnumerable<UiFlowContextData> _endState;
		public string ScopeId { get; set; }


		public IEnumerable<Difference> Diff()
		{
			
			var result = _Compare(InitialState, EndState);
			return result;
		}
		public IEnumerable<Difference> Diff(string flowHandler)
		{
			
			var initial = InitialState.SingleOrDefault(x=>x.FlowHandler==flowHandler);
			var end = EndState.SingleOrDefault(x => x.FlowHandler == flowHandler);
			var result = _Compare(initial, end);

			return result;
		}

		private IEnumerable<Difference> _Compare<T>(T initial, T end)
		{
			var comparer = new ObjectsComparer.Comparer();
			IEnumerable<Difference> differences;


			var result = comparer.Compare(initial.ToJson().JsonToObject<ExpandoObject>(), end.ToJson().JsonToObject<ExpandoObject>(), out differences);
			return differences;
		}

		public IEnumerable<UiFlowContextData> InitialState { get; set; }

		public IEnumerable<UiFlowContextData> EndState
		{
			get => _endState;
			set
			{
				_endState = value;
				Timestamp = DateTime.Now;
			}
		}

		public DateTime? Timestamp { get; private set; }
	}
}