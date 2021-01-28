using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using NUnit.Framework;

namespace EI.RP.DataModels.UnitTests.Sap
{
	public class SapDataModelsTests
	{
		
		[Test]
		public void SapDtoTypesAreWellFormed()
		{
			var type = typeof(AccountDto);
			var strings = type.Namespace.Split('.');
			strings = strings.Take(strings.Length - 2).ToArray();
			var ns = string.Join(".", strings);
			

			var dtoTypes = type.Assembly.GetTypes().Where(x =>!x.IsCompilerGenerated() &&  x.Namespace!=null && x.Namespace.StartsWith(ns) && !x.IsStatic() && !x.IsNested);

			var errorMessages = new List<string>();
			foreach (var dtoType in dtoTypes)
			{
				Console.WriteLine(dtoType.FullName);

				if (dtoType.Namespace.Split('.').Any(x => x == "Dtos"))
				{
					VerifyDto(dtoType);
				}
				else if (dtoType.Namespace.Split('.').Any(x => x == "Functions"))
				{
					VerifyFunction(dtoType);
				}
				
			}

			if (errorMessages.Any())
			{
				Assert.Fail(string.Join(Environment.NewLine,errorMessages));
			}

			void VerifyDto(Type dtoType)
			{
				if (!dtoType.Name.EndsWith("Dto"))
				{
					errorMessages.Add($"{dtoType.FullName} must end in 'Dto' ");
				}

				if (!typeof(ODataDtoModel).IsAssignableFrom(dtoType))
				{
					errorMessages.Add($"{dtoType.FullName} must implement '{nameof(ODataDtoModel)}' ");
				}

				foreach (var prop in dtoType.GetProperties().Where(x => !x.CanWrite || !x.CanRead))
				{
					errorMessages.Add($"{dtoType.FullName}.{prop.Name} must be R/W");
				}

				foreach (var field in dtoType.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					errorMessages.Add(
						$"{dtoType.FullName}.{field.Name} must be made property in order to be able to deserialize");
				}
			}

			void VerifyFunction(Type dtoType)
			{
				if (!dtoType.Name.EndsWith("Function"))
				{
					errorMessages.Add($"{dtoType.FullName} must end in 'Function' ");
				}

				if (!dtoType.ExtendsOpenGeneric(typeof(ODataFunction<>)))
				{
					errorMessages.Add($"{dtoType.FullName} must implement 'ODataFunction' ");
				}

				foreach (var prop in dtoType.GetProperties().Where(x =>x.DeclaringType==dtoType&& !x.CanWrite || !x.CanRead))
				{
					errorMessages.Add($"{dtoType.FullName}.{prop.Name} must be R/W");
				}

				foreach (var field in dtoType.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					errorMessages.Add(
						$"{dtoType.FullName}.{field.Name} must be made property in order to be able to deserialize");
				}
			}
		}

		
	}
}