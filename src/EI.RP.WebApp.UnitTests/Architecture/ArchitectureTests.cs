using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using Castle.DynamicProxy.Internal;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using EI.RP.DataServices;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;

namespace EI.RP.WebApp.UnitTests.Architecture
{
	[TestFixture]
	public class ArchitectureTests
	{

		public static IEnumerable GetPresentationViewModels(string namePrefix)
		{
			foreach (var type in typeof(ResidentialPortalFlowType).Assembly.GetTypes().Where(x => !x.IsAnonymous()
				&& x.BaseTypes().Any(_ => _ == typeof(UiFlowScreenModel) || _ == typeof(FlowComponentViewModel))))
			{
				yield return new TestCaseData(type).SetName($"{namePrefix}:{type.FullName}");
			}
		}
		[TestCaseSource(sourceName:nameof(GetPresentationViewModels),methodParams:new object[]{nameof(PresentationModels_HasNotPublicFields)})]
		public void PresentationModels_HasNotPublicFields(Type viewModelType)
		{
			var fields = viewModelType.GetFields(BindingFlags.Instance | BindingFlags.Public|BindingFlags.FlattenHierarchy)
				.Where(x=>x.GetCustomAttribute(typeof(Newtonsoft.Json.JsonIgnoreAttribute))==null && x.GetCustomAttribute(typeof(System.Text.Json.Serialization.JsonIgnoreAttribute))==null)
				.ToArray();

			Assert.IsFalse(fields.Any(),string.Join(Environment.NewLine, fields.Select(x=>$"{viewModelType.FullName}--> {x.Name} uses domain type {x.FieldType}")));
		}

		[TestCaseSource(sourceName:nameof(GetPresentationViewModels),methodParams:new object[]{nameof(PresentationModels_AreNot_CoupledTo_DomainDefinitions)})]
		public void PresentationModels_AreNot_CoupledTo_DomainDefinitions(Type viewModelType)
		{
			var properties = viewModelType.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy);

			Func<Type, bool> propIsDomainType = y => y == typeof(IQueryResult) || y == typeof(IDomainCommand) 
			                                                            || y.IsGenericType && y.GetGenericArguments().Any(ga =>  typeof(IQueryResult).IsAssignableFrom(ga) || typeof(IDomainCommand).IsAssignableFrom(ga));
			var illegalProperties = properties
				.Where(x => x.PropertyType.GetAllInterfaces().Any(propIsDomainType))
				.Where(x => x.GetCustomAttribute(typeof(Newtonsoft.Json.JsonIgnoreAttribute)) == null && x.GetCustomAttribute(typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) == null)
				.ToArray();
			
			Assert.IsFalse(illegalProperties.Any(),string.Join(Environment.NewLine, illegalProperties.Select(x=>$"{viewModelType.FullName}--> {x.PropertyType.FullName} {x.Name}")));
		}

		[Test]
		public void ControllersHierarchy_IsCorrect()
		{
			var controllerTypes = typeof(Program).Assembly.GetTypes()
				.Where(x => x.IsSubclassOf(typeof(Controller))
				            && !x.IsSubclassOf(typeof(ControllerBase))
							&& x != typeof(ControllerBase)
				            && !x.GetCustomAttributes(typeof(AllowAnonymousAttribute),true).Any()
				            ).Select(x => x.FullName).ToArray();
			if (controllerTypes.Any())
			{
				Assert.Fail(
					$"The following controllers must implement {nameof(ControllerBase)}:{Environment.NewLine}{string.Join(Environment.NewLine, controllerTypes)}");
			}
		}

		public static IEnumerable GetPresentationTypes()
		{
			foreach (var type in typeof(ResidentialPortalFlowType).Assembly.GetTypes().Where(x=>!x.IsAnonymous() && !typeof(IResidentialPortalHealthCheck).IsAssignableFrom(x)))
			{
				yield return new TestCaseData(type).SetName($"No Direct DataAccessFrom Ui:{type.FullName}");
			}
		}
		[TestCaseSource(nameof(GetPresentationTypes))]
		[Test]
		public void RepositoriesAreNotDirectlyBeingAccessedFromPresentation(Type type)
		{
			var parameters = type.GetConstructors().SelectMany(y => y.GetParameters()).Where(x =>
				typeof(IDataService).IsAssignableFrom(x.ParameterType) );

			if (parameters.Any())
			{
				Assert.Fail($"Cannot user a DataService in the presentation layer. It is being illegally injected to {string.Join(',', parameters.Select(x => x.Member.DeclaringType.FullName))}");
			}
		}

		[Test,Ignore("TODO")]
		public void TestCsHtmlIllegalUsages()
		{
			Assert.Fail(@"

		domain models accesses
		actionlink instead of encryptedactionlink
		straight hrefs to relative paths
		antitamper hidden fields
		"
				);
		}
		
	}
}