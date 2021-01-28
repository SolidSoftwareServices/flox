using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.System.FastReflection;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library.Values;

namespace ResidentialPortalCLI.OData.ODataProxy.Generators.Dto
{
	partial class DtoTemplate: ICodeGenerationTemplate
	{
		public DtoTemplate(ODataSettingsFile input, IEdmModel metadata, IEdmSchemaElement edmType, string className,
			string repositoryName, string containerName)
		{
			EdmType = edmType;
				
			ClassName = className;
			RepositoryName = repositoryName;
			ContainerName = containerName;
			ResolveOperations();

			ResolveProperties();
			ResolvePrimaryKeys();

			void ResolvePrimaryKeys()
			{
				if(((IEdmStructuredType)this.EdmType).TypeKind==EdmTypeKind.Entity)
				{
					PrimaryKeys = (((IEdmEntityType)this.EdmType).DeclaredKey).Select(property => property.Name).ToArray();
				}
			}
			void ResolveProperties()
			{
				var numericTypesAsStrings = input.EntitiesWhichUpdatesHackOData.Contains(ClassName);

				Properties = (((IEdmStructuredType) this.EdmType).DeclaredProperties)
					.Select(property =>
					{
						var annotations = metadata.DirectValueAnnotationsManager.GetDirectValueAnnotations(property).ToArray();
						//because the aPI forces us to serialize to json
						var typeName = property.Type.ResolveCsTypeName(numericTypesAsStrings:numericTypesAsStrings);
						var isNullable = typeName.EndsWith("Dto") || typeName.EndsWith("Dto>") || property.Type.IsNullable;
						return new PropertyData
						{
							ReturnType = typeName,
							Name = property.Name,
							DefaultValue = GetPropertyDefaultValue(property,isNullable,numericTypesAsStrings),
							IsNullable = isNullable,
							MaxLength = GetMaxLength(property),
							CanSort= ResolveAnnotation(annotations, "sortable"),
							CanFilter = ResolveAnnotation(annotations, "filterable"),
						};
					});
			}

			void ResolveOperations()
			{
				var entitySet = metadata.SchemaElements.Where(x => x is IEdmEntityContainer)
					.Cast<IEdmEntityContainer>()
					.SelectMany(x => x.Elements.Where(y => y is IEdmEntitySet && ((IEdmEntitySet) y).ElementType == edmType))
					.SingleOrDefault();

				if (entitySet != null)
				{
					CollectionName = entitySet.Name;
					MustGenerate = !input.ExcludeEntities.Contains(CollectionName);
					var annotations = metadata.DirectValueAnnotationsManager.GetDirectValueAnnotations(entitySet).ToArray();
					Operations.CanAdd = ResolveAnnotation(annotations,"creatable");
					Operations.CanUpdate = ResolveAnnotation(annotations, "updatable");
					Operations.CanDelete = ResolveAnnotation(annotations, "deletable"); 
				}

				
			}
			bool ResolveAnnotation(IEdmDirectValueAnnotation[] annotations, string attrName)
			{
				return annotations.Any(
					       x => x.Name == attrName && Equals(((EdmStringConstant) x.Value).Value, true.ToString()))
				       || annotations.All(x => x.Name != attrName);
			}
		}

		public string ContainerName { get; set; }

		public IEnumerable<string> PrimaryKeys { get; set; } = new string[0];

		public string CollectionName { get; set; }

		private string GetMaxLength(IEdmProperty property)
		{
			if (property.Type.TypeKind() == EdmTypeKind.Primitive &&
			    ((IEdmPrimitiveType) property.Type.Definition).PrimitiveKind == EdmPrimitiveTypeKind.String)
			{
				
			var maxLength = property.Type.GetPropertyValueFastFromPropertyPath("MaxLength")?.ToString();
				return maxLength;
			}
			else return null;
		}

		private CrudOperations Operations { get; }=new CrudOperations();
		private class CrudOperations
		{
			public bool CanQuery { get; set; } = true;
			public bool CanAdd { get; set; } = false;
			public bool CanUpdate{ get; set; } = false;
			public bool CanDelete{ get; set; } = false;
		}

		private class PropertyData
		{
			public string Name { get; set; }
			public string ReturnType { get; set; }
			public string DefaultValue { get; set; }
			public bool IsNullable { get; set; }
			public string MaxLength { get; set; }
			public bool CanSort { get; set; }
			public bool CanFilter { get; set; }
		}

		public IEdmSchemaElement EdmType { get; }
		public string ClassName { get; }
		public string RepositoryName { get; }

		private IEnumerable<PropertyData> Properties { get; set; }

		private string GetPropertyDefaultValue(IEdmProperty property, bool isNullable, bool allTypesString)
		{
			var result = string.Empty;
			switch (property.Type.TypeKind())
			{
				case EdmTypeKind.Collection:
					var edmCollectionType = (IEdmCollectionType) property.Type.Definition;
					var definition = (IEdmNamedElement) edmCollectionType.ElementType.Definition;
					if (!property.Type.IsNullable)
					{
						result = $" = new List<{definition.Name}Dto>();";
					}
					break;
				case EdmTypeKind.Primitive:
					var primitiveProperty = (IEdmPrimitiveType)property.Type.Definition;
					switch (primitiveProperty.PrimitiveKind)
					{
						
						case EdmPrimitiveTypeKind.Single:
						case EdmPrimitiveTypeKind.Double:
						case EdmPrimitiveTypeKind.Decimal:
							if (allTypesString && !property.Type.IsNullable)
							{
								result = " = \"0.0\";";
							}
							break;
						case EdmPrimitiveTypeKind.Int16:
						case EdmPrimitiveTypeKind.Int32:
						case EdmPrimitiveTypeKind.Int64:
							if (allTypesString && !property.Type.IsNullable)
							{
								result = " = \"0\";";
							}

							break;
						case EdmPrimitiveTypeKind.String:
							if (!isNullable)
							{
								result = $" = string.Empty;";
							}
							break;
						
						
					}

					break;
			}

			return result;
		}

		public bool MustGenerate { get; private set; } = true;

		public string Extension => "cs";
	}
}
