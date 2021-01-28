using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.System.FastReflection;
using Microsoft.Data.Edm;

namespace ResidentialPortalCLI.OData.ODataProxy.Generators.Function
{
	partial class FunctionTemplate: ICodeGenerationTemplate
	{
		public string Extension => "cs";
		public FunctionTemplate(IEdmModel metadata, IEdmFunctionImport edmType, string className, string repositoryName,
			string containerName)
		{
			EdmType = edmType;
			
			FunctionClassName = $"{className}Function";
			FunctionName = className ;
			FunctionResultType = edmType.ReturnType.ResolveCsTypeName(specifyCollectionItemsAsResult:true);
			ReturnsComplexType = edmType.ReturnType?.IsComplex()??false;
			ReturnsCollection = edmType.ReturnType?.IsCollection() ?? false;

			RepositoryName = repositoryName;
			ContainerName = containerName;
			Properties =this.EdmType.Parameters
				.Select(parameter =>
				{
					var typeName = parameter.Type.ResolveCsTypeName().Replace('?', '\n');
					return new ParameterData
					{
						ParameterType = typeName,
						Name = parameter.Name,
						MaxLength = GetMaxLength(parameter.Type),
						DefaultValue=typeName=="string"
							?" = string.Empty;"
							:string.Empty
					};
				});
		}

		public bool ReturnsCollection { get; set; }

		public bool ReturnsComplexType { get; set; }

		public string ContainerName { get; set; }


		private string GetMaxLength(IEdmTypeReference property)
		{
			if (property.TypeKind() == EdmTypeKind.Primitive &&
			    ((IEdmPrimitiveType)property.Definition).PrimitiveKind == EdmPrimitiveTypeKind.String)
			{

				var maxLength = property.GetPropertyValueFastFromPropertyPath("MaxLength")?.ToString();
				return maxLength;
			}
			else return null;
		}



		public string FunctionResultType { get; set; }

		private class ParameterData
		{
			public string Name { get; set; }
			public string ParameterType { get; set; }
			public string MaxLength { get; set; }
			public string DefaultValue { get; set; }
		}

		public IEdmFunctionImport EdmType { get; }


		public string FunctionClassName { get; }
		public string FunctionName { get; }
		public string RepositoryName { get; }

		private IEnumerable<ParameterData> Properties { get; }

		public bool MustGenerate { get; } = true;
	}
}
