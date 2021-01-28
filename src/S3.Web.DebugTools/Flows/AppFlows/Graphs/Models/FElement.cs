using System;

namespace S3.Web.DebugTools.Flows.AppFlows.Graphs.Models
{
	internal abstract class FElement
	{
		public string Id { get; } = Guid.NewGuid().ToString();
		public string ToolTip { get; set; }
	}
}