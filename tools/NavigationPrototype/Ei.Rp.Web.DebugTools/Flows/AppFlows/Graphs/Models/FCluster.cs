using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ei.Rp.Web.DebugTools.Flows.AppFlows.Graphs.Models
{
	class FCluster : FNode
	{
		public FCluster(string caption) : base(caption)
		{
		}
		private readonly List<FNode> _children = new List<FNode>();

		public IEnumerable<FNode> Children => _children;

		public FMargin Margin { get; set; }=new FMargin
		{
			Left = 16
		};

		public FNode AddNode(string nodeText)
		{
			return AddNode(new FNode(nodeText));
		}

		public FNode AddNode(FNode node)
		{
			_children.Add(node);
			return node;
		}

		public override bool IsCluster { get; } = true;
	}
}