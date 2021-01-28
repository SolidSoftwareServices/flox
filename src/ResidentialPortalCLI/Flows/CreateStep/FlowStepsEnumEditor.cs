using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ResidentialPortalCLI.Flows.CreateStep
{
	
	public class FlowStepsEnumEditor
	{

		public static async Task AddFlowStepEnumMember(CreateNewStepOptions opts)
		{
			var filePath = Path.Combine(opts.ProjectDir, $"Flows/AppFlows/{opts.FlowName}/FlowDefinitions", $"{opts.FlowName}Step.cs");
			filePath = Path.GetFullPath(filePath);
			if (!File.Exists(filePath))
			{
				throw new InvalidOperationException("Flow step type must exist");
			}

			var repeat = false;
			int times = 0;
			string readAllTextAsync=null;
			do
			{
				try
				{
					readAllTextAsync = await File.ReadAllTextAsync(filePath);
					repeat = false;
				}
				catch
				{
					if (++times < 5)
					{
						repeat = true;
					}
					else
					{
						throw;
					}
					await Task.Yield();
				}
			} while (repeat);

			var source = SourceText.From(readAllTextAsync);
			var tree = SyntaxFactory.ParseSyntaxTree(source,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
			var rootNode = await tree.GetRootAsync();
			var namespaceDeclarationNode = rootNode.ChildNodes().Single(x=>x is NamespaceDeclarationSyntax);
			var classNode =
				(ClassDeclarationSyntax) namespaceDeclarationNode.ChildNodes().Single(x => x is ClassDeclarationSyntax);

			var exists = classNode.Members
				.Where(x => x is FieldDeclarationSyntax)
				.Cast<FieldDeclarationSyntax>()

				.Any(x=>x.Declaration.Variables[0].Identifier.Text==opts.StepName)
				;
			if (!exists)
			{
				var field=SyntaxFactory.ParseMemberDeclaration(
					$"public static readonly ScreenName {opts.StepName} = new ScreenName(nameof({opts.StepName}));");
				var newClassNode = classNode.AddMembers(field);

				var generatedNamespaceDeclarationSyntaxNode =
						namespaceDeclarationNode.ReplaceNode(classNode, newClassNode);
				var generatedRootNode =
						rootNode.ReplaceNode(namespaceDeclarationNode, generatedNamespaceDeclarationSyntaxNode);
					await File.WriteAllTextAsync(filePath, generatedRootNode.NormalizeWhitespace().ToFullString());
					Console.WriteLine($"Added flow step field member {opts.StepName} to {filePath}");
			}

		}
	}
}