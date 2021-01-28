using System;
using Microsoft.Data.Edm;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Edmx
{
	public static class IEdmTypeReferenceExtensions
	{
		public static string ResolveCsTypeName(this IEdmTypeReference type, bool specifyCollectionItemsAsResult=false,bool numericTypesAsStrings=false)
		{
			switch (type.TypeKind())
			{
				case EdmTypeKind.Primitive:
					return ResolvePrimitive(type, numericTypesAsStrings);

				case EdmTypeKind.Entity:
				case EdmTypeKind.Complex:
				{
					var complex = (IEdmNamedElement)type.Definition;
					return $"{complex.Name}Dto";
				}
				case EdmTypeKind.Collection:
				{
					var edmCollectionType = (IEdmCollectionType)type.Definition;
					var definition = (IEdmNamedElement)edmCollectionType.ElementType.Definition;
					var itemName = $"{definition.Name}Dto";
					return  specifyCollectionItemsAsResult?itemName :$"List<{itemName}>";
				}

				case EdmTypeKind.None:

				case EdmTypeKind.Row:


				case EdmTypeKind.EntityReference:
				case EdmTypeKind.Enum:
					throw new NotSupportedException();
				default:
					throw new ArgumentOutOfRangeException($"{type.TypeKind()} not supported");
			}
		}

		private static string ResolvePrimitive(IEdmTypeReference type, bool numericTypesAsStrings)
		{
			var primitiveProperty = (IEdmPrimitiveType) type.Definition;
			var primitiveResult = string.Empty;
			switch (primitiveProperty.PrimitiveKind)
			{
				case EdmPrimitiveTypeKind.Decimal:
					primitiveResult = numericTypesAsStrings ? "string" : "decimal";
					break;
				case EdmPrimitiveTypeKind.Double:
					primitiveResult = numericTypesAsStrings ? "string" : "double";
					break;
				case EdmPrimitiveTypeKind.Int16:
					primitiveResult = numericTypesAsStrings ? "string" : "short";
					break;

				case EdmPrimitiveTypeKind.Int32:
					primitiveResult = numericTypesAsStrings ? "string" : "int";
					break;
				case EdmPrimitiveTypeKind.Int64:
					primitiveResult = numericTypesAsStrings ? "string" : "long";
					break;

				case EdmPrimitiveTypeKind.Boolean:
					primitiveResult = "bool";
					break;
				case EdmPrimitiveTypeKind.Byte:
					primitiveResult = "byte";
					break;

				case EdmPrimitiveTypeKind.DateTime:
					primitiveResult = "DateTime";
					break;
				case EdmPrimitiveTypeKind.DateTimeOffset:
					primitiveResult = "DateTimeOffset";
					break;
				case EdmPrimitiveTypeKind.Time:
					primitiveResult = "TimeSpan";
					break;
				case EdmPrimitiveTypeKind.Guid:
					primitiveResult = "Guid";
					break;
				//following are notnullables
				case EdmPrimitiveTypeKind.String:
					return "string";
				case EdmPrimitiveTypeKind.Binary:
					return "byte[]";
				default:
					throw new NotSupportedException();
			}

			return $"{primitiveResult}{(type.IsNullable && primitiveResult != "string" ? "?" : string.Empty)}";
		}
	}
}