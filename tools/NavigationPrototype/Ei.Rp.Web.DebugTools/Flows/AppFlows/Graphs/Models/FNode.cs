using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ei.Rp.Web.DebugTools.Flows.AppFlows.Graphs.Models
{
	internal class FNode:FElement
	{
		
		
		public FNode(string caption,FNodeColor color=null)
		{
			this.Label = caption;
			if (color != null)
			{
				this.Stroke = color.Stroke;
				this.Fill = color.Fill;
			}
		}

		public string Label { get; }

		private readonly List<FEdge> _edges = new List<FEdge>();
		[JsonIgnore]
		public IEnumerable<FEdge> Edges => _edges;
		public FEdge AddEdge( FNode destination, string label)
		{
			var edge = new FEdge(this, destination, label);
			_edges.Add(edge);
			return edge;
		}

		public string Stroke { get; set; }
		public int Thickness { get; set; } = 1;
		public string Fill { get; set; }

		public virtual bool IsCluster { get; } = false;

		public override string ToString()
		{
			return $"{nameof(Label)}: {Label}, {nameof(IsCluster)}: {IsCluster}";
		}

		public class FNodeColor
		{

			public static readonly FNodeColor Gray = new FNodeColor("lightgray", "darkgray");
			public static readonly FNodeColor Green = new FNodeColor("lightgreen", "darkgreen");
			public static readonly FNodeColor Blue = new FNodeColor("lightblue", "darkblue");

			public string Fill { get; }
			public string Stroke { get; }

			

			private FNodeColor(string fill,string stroke)
			{
				Fill = fill;
				Stroke = stroke;
			}
		}
	}
}