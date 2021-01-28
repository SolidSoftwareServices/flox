using System.Collections.Generic;
using System.Linq;

namespace Ei.Rp.Web.DebugTools.Flows.AppFlows.Graphs.Models
{
	class FGraph
	{

		private readonly List<FNode> _nodes = new List<FNode>();
		public double AspectRatio { get; set; } = 0.7;
		public IEnumerable<FNode> Nodes => _nodes;

		public IEnumerable<FEdge> Edges => ResolveEdges();

		private IEnumerable<FEdge> ResolveEdges()
		{
			var result = new List<FEdge>();

			ResolveNodeEdges(Nodes.ToArray());

			void ResolveNodeEdges(FNode[] src)
			{
				if (src.Any())
				{
					result.AddRange(src.Where(x => x.GetType() == typeof(FNode)).SelectMany(x => x.Edges));
					ResolveNodeEdges(src.Where(x => x is FCluster).Cast<FCluster>().SelectMany(x => x.Children).ToArray());
				}
			}

			return result;
		}

		public FRect BoundingBox { get; set; }
		//public FSettings Settings { get; set; }

		public void AddNode(FNode node)
		{
			_nodes.Add(node);
		}
		public void AddNode(string caption)
		{
			this.AddNode(new FNode(caption));
		}

		public void AddCluster(FCluster cluster)
		{
			AddNode(cluster);
		}

	}
}
