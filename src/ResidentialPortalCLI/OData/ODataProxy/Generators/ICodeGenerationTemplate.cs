namespace ResidentialPortalCLI.OData.ODataProxy.Generators
{
	public interface ICodeGenerationTemplate
	{
		string TransformText();
		bool MustGenerate { get; }

		string Extension { get; }
	}
}