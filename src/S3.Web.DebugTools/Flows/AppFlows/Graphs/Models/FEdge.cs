namespace S3.Web.DebugTools.Flows.AppFlows.Graphs.Models
{
	internal class FEdge:FElement
	{
		private readonly FNode _sourceNode;
		private readonly FNode _targetNode;
	

		public FEdge(FNode sourceNode, FNode targetNode, string label)
		{
			this.Label = label;
			_sourceNode = sourceNode;
			_targetNode = targetNode;
			
		}

		public string Label { get;  }


		public string Source => _sourceNode.Id;
		public string Target => _targetNode.Id;
		public int? Thickness { get; set; }
		public string Dash { get; set; }

		public override string ToString()
		{
			return $"{Label}: {Source} --> {Target}";
		}
	}
}