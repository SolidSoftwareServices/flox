using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using EI.RP.DomainServices.Commands;
using EI.RP.DomainServices.Infrastructure;
using Moq.AutoMock;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Architecture
{
	[TestFixture]
	public class ArchitectureTests
	{
		public static IEnumerable GetDomainTypes()
		{
			var ns = typeof(CompilerExtensions).Namespace;

			foreach (var type in TypesFinder.Resolver.FindTypesByNamespace(ns))
				yield return new TestCaseData(type).SetName($"Domain Access is correct in {type.FullName}");
		}
		
		[Test]
		[TestCaseSource(nameof(GetDomainTypes))]
		public void DispatchersAreNotUsedInTheDomain(Type type)
		{
			var parameters = type.GetConstructors().SelectMany(y => y.GetParameters()).Where(x =>
				typeof(IDomainQueryResolver).IsAssignableFrom(x.ParameterType) ||
				typeof(IDomainCommandDispatcher).IsAssignableFrom(x.ParameterType)).ToArray();

			if (parameters.Any())
				Assert.Fail(
					$"Cannot user this type outside the presentation layer. Use IQueryHandler or ICommandHandler instead. It is being illegally injected to {string.Join(',', parameters.Select(x => x.Member.DeclaringType.FullName))}");
		}

		public static IEnumerable GetMappingTypes()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<TypedStringValue>())
				yield return new TestCaseData(type).SetName($"MappingValueTypesAreWellFormed: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(GetMappingTypes))]
		public void MappingValueTypesAreWellFormed(Type type)
		{
			var cctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes,
				null);
			Assert.IsNotNull(cctor,
				$"type {type.FullName} must have an empty constructor either public or private(preferred)");

			if (!cctor.GetCustomAttributes(typeof(JsonConstructorAttribute), false).Any())
				Assert.Fail($"type {type.FullName} must have an empty constructor decorated with JsonConstructor");

			if (typeof(TypedStringValue).IsAssignableFrom(type) &&
			    !type.ExtendsOpenGeneric(typeof(TypedStringValue<>)))
				Assert.Fail($"type {type.FullName} must implement generic {typeof(TypedStringValue<>).FullName}");
		}

		public static IEnumerable GetQueriesAndCommands()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IQueryModel,IDomainCommand>())
				yield return new TestCaseData(type).SetName($"QueriesAndCommandsImplementIEquatable: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(GetQueriesAndCommands))]
		public void QueriesAndCommandsImplementIEquatable(Type type)
		{
			var interfaceType = typeof(IEquatable<>).MakeGenericType(type);
			if (!type.Implements(interfaceType))
			{
				Assert.Fail($"{type.FullName} must implement {interfaceType.Name}: Resharper Alt+Insert might help");
			}
		}

		public static IEnumerable CommandIsImmutableCases()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IDomainCommand>())
				yield return new TestCaseData(type).SetName($"Commands are immutable: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(CommandIsImmutableCases))]
		public void CommandIsImmutable(Type type)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.CanWrite && !x.SetMethod.IsFamily)
				.Select(x => x.Name)
				.ToArray();
			string error = null;
			if (properties.Any())
			{
				error=$"*Invalid public property writers:{Environment.NewLine}{string.Join(Environment.NewLine,properties)}";
			}
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
				.Where(x=>!x.IsFamily)
				.Select(x => x.Name)
				.ToArray();
			if (fields.Any())
			{
				error +=
					$"{Environment.NewLine}* Invalid public fields: {Environment.NewLine}{string.Join( Environment.NewLine, properties)}";
			}

			if (error != null)
			{
				Assert.Fail(error);
			}
		}

		public static IEnumerable QuerySpecificCases()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IQueryModel>())
				yield return new TestCaseData(type).SetName($"Queries specific cases: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(QuerySpecificCases))]
		public void QuerySpecificTests(Type type)
		{
			var mocker = new AutoMocker();
			var sut = mocker.CreateInstance(type);

			var expected = type.FullName;
			var actual = sut.ToString();
			Assert.AreNotEqual(expected,actual,$"{type} must implement {nameof(object.ToString)}");
			Assert.IsFalse(actual.StartsWith(type.Name),$"{type}.{nameof(ToString)} must not contain start by the typename");
		}

	
		public static IEnumerable GetQueriesAndCommandHandlers()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOfOpenGeneric(typeof(IQueryHandler<>),typeof(ICommandHandler<>)))
				yield return new TestCaseData(type).SetName($"QueriesAndCommandHandlersAreNotPublic: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(GetQueriesAndCommandHandlers))]
		public void QueriesAndCommandHandlersConstraints(Type type)
		{
			
			if (type.IsPublic)
			{
				Assert.Fail($"{type.FullName} must be private or internal");
			}

			var errors = type.GetFields().Where(x => x.FieldType.ImplementsOpenGeneric(typeof(IQueryHandler<>)))
				.Select(x => $"Illegal QueryHandler={x.Name}");

			errors = errors.Union(type.GetFields()
				.Where(x => x.FieldType.ImplementsOpenGeneric(typeof(ICommandHandler<>)))
				.Select(x => $"Illegal CommandHandler={x.Name}"));
			errors = errors.Union(type.GetFields()
				.Where(x => x.FieldType.Implements(typeof(ICacheProvider)))
				.Select(x => $"Illegal usage of cache provider={x.Name}"));
			if(errors.Any())
			{
				var lst=errors.ToList();
				lst.Insert(0,"use query and command dispatchers bypassing or sugar syntax classes ");
				Assert.Fail(string.Join(Environment.NewLine,lst));
			}
		}
		public static IEnumerable DataAccessTypesAreNotPartOfTheModelsCases()
		{

			var domainModels = TypesFinder.Resolver
				.FindConcreteTypesOfOpenGeneric(typeof(IQueryHandler<>), typeof(ICommandHandler<>))
				.Union(TypesFinder.Resolver.FindConcreteTypesOf<IQueryResult, IDomainCommand>());
			foreach (var type in domainModels)
				yield return new TestCaseData(type).SetName($"{type.FullName} does not have data models");
		}
		[Test]
		[TestCaseSource(nameof(DataAccessTypesAreNotPartOfTheModelsCases))]
		public void DataAccessTypesAreNotPartOfTheModels(Type type)
		{
			var properties = type.GetPropertiesFast(BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Instance|BindingFlags.Static|BindingFlags.GetProperty|BindingFlags.SetProperty,
				p=>typeof(ODataDtoModel).IsAssignableFrom(p.PropertyType)).Select(x=>x.Name).ToArray();
			CollectionAssert.IsEmpty(properties,$"Cannot use data access object as part of the domain models - {type.FullName}::{string.Join(',',properties)}");
		}

		public static IEnumerable AllCommandsHaveEventBuildersCases()
		{
			foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IDomainCommand>())
				yield return new TestCaseData(type).SetName($"Command has event builder: {type.FullName}");
		}
		[Test]
		[TestCaseSource(nameof(AllCommandsHaveEventBuildersCases))]
		public void AllCommandsHaveEventBuilders(Type type)
		{
			var actual=TypesFinder.Resolver.FindConcreteTypesOf(typeof(ICommandEventBuilder<>).MakeGenericType(type),includePublicOnly:false).ToArray();
			CollectionAssert.IsNotEmpty(actual);
			Assert.AreEqual(1,actual.Length);

		}
	}
}