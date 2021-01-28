using System;
using System.IO;
using System.Threading.Tasks;

namespace ResidentialPortalCLI.Flows.CreateNewFlow.Builders
{
	public class FlowFolderStructureBuilder
	{
		public static async Task Build(CreateNewFlowOptions opts)
		{
			var root = Path.Combine(opts.ProjectDir, "Flows/AppFlows", opts.FlowName);
			root = Path.GetFullPath(root);
			var exists = Directory.Exists(root);
			if (exists && opts.OverrideExisting)
			{
				
				Directory.Delete(root,true);
				Console.WriteLine($"Deleted existing flow folder {opts.FlowName}");
				exists = false;
			}

			
			if (!exists)
			{
				Directory.CreateDirectory(root);
				Directory.CreateDirectory(Path.Combine(root,"Components"));
				Directory.CreateDirectory(Path.Combine(root, "FlowDefinitions"));
				Directory.CreateDirectory(Path.Combine(root, "Steps"));
				Directory.CreateDirectory(Path.Combine(root, "Views"));
				Console.WriteLine($"Created flow folder structure of {opts.FlowName}");
			}
			
		}
	}


}