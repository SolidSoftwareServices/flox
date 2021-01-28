using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ResidentialPortalCLI.Flows.CreateNewFlow.Builders
{
	
	public class FlowTypeEnumEditor
	{

		public static async Task AddFlowEnumMember(CreateNewFlowOptions opts)
		{
			var filePath = Path.Combine(opts.ProjectDir, "Flows/AppFlows", $"{opts.FlowTypesEnumName}.cs");
			filePath = Path.GetFullPath(filePath);
			if (!File.Exists(filePath))
			{
				throw new InvalidOperationException("Flow Enum type must exist");
			}

			var source = SourceText.From(await File.ReadAllTextAsync(filePath));
			var tree = SyntaxFactory.ParseSyntaxTree(source,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
			var rootNode = await tree.GetRootAsync();
			var namespaceDeclarationNode = rootNode.ChildNodes().Single();
			var enumDeclarationSyntaxNode =
				(EnumDeclarationSyntax) namespaceDeclarationNode.ChildNodes().Single(x => x is EnumDeclarationSyntax);

			var members = enumDeclarationSyntaxNode.Members;

			if (members.All(x => x.Identifier.ValueText != opts.FlowName))
			{
				var generatedEnumDeclarationSyntax =
					enumDeclarationSyntaxNode.Members.Any(x => x.Identifier.ValueText == opts.FlowName)
						? enumDeclarationSyntaxNode
						: enumDeclarationSyntaxNode.AddMembers(SyntaxFactory.EnumMemberDeclaration(opts.FlowName));
				var noflow= generatedEnumDeclarationSyntax.Members
					.Single(x => x.Identifier.Text == "NoFlow");
				var ordered= generatedEnumDeclarationSyntax.Members
					.Where(x=>x.Identifier.Text!="NoFlow")
					.OrderBy(x=>x.Identifier.ValueText).ToArray();
				generatedEnumDeclarationSyntax=generatedEnumDeclarationSyntax.RemoveNodes(generatedEnumDeclarationSyntax.Members,
					SyntaxRemoveOptions.KeepNoTrivia);
				generatedEnumDeclarationSyntax=generatedEnumDeclarationSyntax
					.AddMembers(noflow)
					.AddMembers(ordered);

				var generatedNamespaceDeclarationSyntaxNode =
					namespaceDeclarationNode.ReplaceNode(enumDeclarationSyntaxNode, generatedEnumDeclarationSyntax);
				var generatedRootNode =
					rootNode.ReplaceNode(namespaceDeclarationNode, generatedNamespaceDeclarationSyntaxNode);
				await File.WriteAllTextAsync(filePath, generatedRootNode.NormalizeWhitespace().ToFullString());
				Console.WriteLine($"Added flow enum member {opts.FlowName} to {filePath}");
			}
		}
	}
}