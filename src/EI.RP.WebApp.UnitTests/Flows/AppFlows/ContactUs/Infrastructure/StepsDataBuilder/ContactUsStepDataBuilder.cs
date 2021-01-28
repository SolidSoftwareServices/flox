using System;
using System.Collections.Generic;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using System.Linq;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.DomainModelExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScreenModel = EI.RP.WebApp.Flows.AppFlows.ContactUs.Steps.ContactUs.ScreenModel;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.ContactUs.Infrastructure.StepsDataBuilder
{
	class ContactUsStepDataBuilder
	{
		private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

		public ContactUsStepDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			//if need to support more than one do it explicitly
			_accountConfigurator = appUserConfigurator.ElectricityAndGasAccountConfigurators.Single();
		}


		public ScreenModel Create()
		{
			var builder = _accountConfigurator.DomainFacade.ModelsBuilder;

			return builder
				.Build<ScreenModel>()
				.With(x => x.AccountNumber, _accountConfigurator.Model.AccountNumber)
				.With(x => x.HasAccounts, _accountConfigurator.Model != null)
				.With(x => x.AccountList, new[]
				{
					new SelectListItem
					{
						Text = string.Join(" ", _accountConfigurator.Model.ClientAccountType,
							_accountConfigurator.Model.Description),
						Value = _accountConfigurator.Model.AccountNumber
					}
				})
				.With(x => x.SelectedAccount, _accountConfigurator.Model.AccountNumber)
				.With(x => x.Partner, _accountConfigurator.Model.Partner)
				.With(x => x.QueryTypes, GetTypeOfQueryDetails())
				.Create();

			IEnumerable<SelectListItem> GetTypeOfQueryDetails()
			{
				return new SelectListItem("Please select", "")
					.ToOneItemArray()
					.Union(ContactQueryType.AllValues.Cast<ContactQueryType>()
						.Select(x =>
							new SelectListItem
							{
								Text = x.ToDisplayValue(),
								Value = Uri.EscapeDataString(x.ToString())
							})).ToArray();
			}
		}
	}
}