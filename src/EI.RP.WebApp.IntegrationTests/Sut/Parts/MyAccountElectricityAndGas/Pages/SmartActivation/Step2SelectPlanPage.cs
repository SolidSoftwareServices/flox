using System;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation.Components;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class Step2SelectPlanPage : SmartActivationPage
	{
		public Step2SelectPlanPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("2. Price Plans | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='step2-select-plan']");

		public PlanCardPageComponent[] PlanCards => Document.QuerySelectorAll(".plan-card").Cast<IHtmlDivElement>().Select(x => new PlanCardPageComponent(App, x)).ToArray();

		public Step2SelectPlanPage AssertPlan(SmartPlan smartPlan)
		{
			var card = GetPlanByDomainModel(smartPlan);
			if (!smartPlan.IsActive)
			{
				Assert.IsNull(card);
				return this;
			}

			Assert.IsNotNull(card);
			Assert.AreEqual(smartPlan.PlanName, card.PlanName);
			Assert.AreEqual(smartPlan.BonusDescription, card.BadgeText);

			if (smartPlan.FullPricingInformation != null)
			{
				Assert.IsNotNull(card.FullPricingInfoLink);
				AssertElectricityPrices(smartPlan, card);
				AssertGasPrices(smartPlan, card);
				if (string.IsNullOrWhiteSpace(smartPlan.FullPricingInformation.PricesValidityMessage))
				{
					Assert.IsNull(card.PriceValidityMessage);
				}
				else
				{
					Assert.AreEqual(smartPlan.FullPricingInformation.PricesValidityMessage, card.PriceValidityMessage.TextContent.Trim());
				}

				if (string.IsNullOrWhiteSpace(smartPlan.FullPricingInformation.LowCostPricingMessage))
				{
					Assert.IsNull(card.LowCostMessage);
				}
				else
				{
					Assert.AreEqual(smartPlan.FullPricingInformation.LowCostPricingMessage, card.LowCostMessage.TextContent.Trim());
				}

				if (string.IsNullOrWhiteSpace(smartPlan.FullPricingInformation.EstimatedAnnualBillMessage))
				{
					Assert.IsNull(card.EstimatedAnnualBillMessage);
				}
				else
				{
					Assert.AreEqual(smartPlan.FullPricingInformation.EstimatedAnnualBillMessage, card.EstimatedAnnualBillMessage.TextContent.Trim());
				}
			}


			//TODO: Assert the rest of the card


			return this;
		}

		public PlanCardPageComponent GetPlanByDomainModel(SmartPlan smartPlan)
		{
			return PlanCards.SingleOrDefault(x => x.PlanName.Equals(smartPlan.PlanName));
		}

		public async Task<ResidentialPortalApp> SelectPlan(SmartPlan smartPlan)
		{
			return await GetPlanByDomainModel(smartPlan).ClickSelectPlan();

		}

		public void AssertPricingTableHeader(IHtmlTableElement table, string firstColumnText)
		{
			var headerRow = table?.Head?.Rows?.FirstOrDefault();
			Assert.IsNotNull(headerRow);
			Assert.AreEqual(3, headerRow.ChildElementCount);
			Assert.AreEqual(firstColumnText, headerRow.Cells[0].TextContent);
			Assert.AreEqual("ex. VAT", headerRow.Cells[1].TextContent);
			Assert.AreEqual("inc. VAT", headerRow.Cells[2].TextContent);
		}

		public void AssertElectricityPrices(SmartPlan smartPlan, PlanCardPageComponent card)
		{
			if (smartPlan.FullPricingInformation?.ElectricityPricingInfo == null)
			{
				Assert.IsNull(card.ElectricityUnitPrices);
				Assert.IsNull(card.ElectricityStandingCharges);
				Assert.IsNull(card.ElectricityPublicServiceObligationLevy);
				return;
			}

			AssertUnitPrices();
			AssertStandingCharges();
			AssertElectricityPublicServiceObligationLevy();

			void AssertUnitPrices()
			{
				if (smartPlan.FullPricingInformation?.ElectricityPricingInfo?.UnitPrices?.Any() != true)
				{
					Assert.IsNull(card.ElectricityUnitPrices);
					return;
				}

				Assert.IsNotNull(card.ElectricityUnitPrices);
				AssertPricingTableHeader(card.ElectricityUnitPrices, "Electricity prices per unit (cent per kWh)");
				var unitPricesRows = card.ElectricityUnitPrices.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(unitPricesRows);
				var unitPricesIterator = -1;
				foreach (var unitPrice in smartPlan.FullPricingInformation.ElectricityPricingInfo.UnitPrices)
				{
					var row = unitPricesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Unit price {unitPrice.Description}", row.Cells[0].TextContent.Trim());

					row = unitPricesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					var addSup = !string.IsNullOrWhiteSpace(smartPlan.FullPricingInformation.LowCostPricingMessage) &&
								 (unitPrice.Description.Equals("Free Weekend Day", StringComparison.OrdinalIgnoreCase) ||
								  unitPrice.Description.Equals("Night Boost", StringComparison.OrdinalIgnoreCase));
					Assert.AreEqual($"Standard {GetUnitPriceDescription(unitPrice.Description)}{(addSup ? " 3" : "")}", row.Cells[0].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.StandardPrice.ExcludingVat.ToStringCents()}/kWh", row.Cells[1].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.StandardPrice.IncludingVat.ToStringCents()}/kWh", row.Cells[2].TextContent.Trim());

					row = unitPricesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Effective {GetUnitPriceDescription(unitPrice.Description)} with {unitPrice.EffectiveDirectDebitDiscountPercentage}% direct debit and online billing discount 1", row.Cells[0].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.EffectivePrice.ExcludingVat.ToStringCents()}/kWh", row.Cells[1].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.EffectivePrice.IncludingVat.ToStringCents()}/kWh", row.Cells[2].TextContent.Trim());
				}

				string GetUnitPriceDescription(string description)
				{
					if (string.IsNullOrWhiteSpace(description))
					{
						return "unit price";
					}
					if (description.Equals("Free Weekend Day", StringComparison.OrdinalIgnoreCase))
					{
						return "Free Saturday or Sunday unit price";
					}
					return $"{description} unit price";
				}
			}

			void AssertStandingCharges()
			{
				if (smartPlan.FullPricingInformation?.ElectricityPricingInfo?.StandingCharges?.Any() != true)
				{
					Assert.IsNull(card.ElectricityStandingCharges);
					return;
				}

				Assert.IsNotNull(card.ElectricityStandingCharges);
				AssertPricingTableHeader(card.ElectricityStandingCharges, "Electricity standing charges per year");

				var chargesRows = card.ElectricityStandingCharges.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(chargesRows);
				var unitPricesIterator = -1;
				foreach (var standingCharge in smartPlan.FullPricingInformation.ElectricityPricingInfo.StandingCharges)
				{
					var row = chargesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"{standingCharge.ChargeDescription} standing charge", row.Cells[0].TextContent.Trim());

					row = chargesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Standing charge 24 hour {standingCharge.ChargeDescription.ToLower()} per year", row.Cells[0].TextContent.Trim());
					Assert.AreEqual(standingCharge.StandingCharge24H.ExcludingVat.ToString(), row.Cells[1].TextContent.Trim());
					Assert.AreEqual(standingCharge.StandingCharge24H.IncludingVat.ToString(), row.Cells[2].TextContent.Trim());

					row = chargesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Low usage standing charge {standingCharge.ChargeDescription.ToLower()} per year 2", row.Cells[0].TextContent.Trim());
					Assert.AreEqual(standingCharge.LowUsageStandingCharge.ExcludingVat.ToString(), row.Cells[1].TextContent.Trim());
					Assert.AreEqual(standingCharge.LowUsageStandingCharge.IncludingVat.ToString(), row.Cells[2].TextContent.Trim());
				}
			}

			void AssertElectricityPublicServiceObligationLevy()
			{
				if (smartPlan.FullPricingInformation?.ElectricityPricingInfo?.ElectricityPublicServiceObligationLevy ==
					null)
				{
					Assert.IsNull(card.ElectricityPublicServiceObligationLevy);
					return;
				}

				Assert.IsNotNull(card.ElectricityPublicServiceObligationLevy);
				AssertPricingTableHeader(card.ElectricityPublicServiceObligationLevy, "Electricity PSO levy per year");

				var pso = smartPlan.FullPricingInformation.ElectricityPricingInfo
					.ElectricityPublicServiceObligationLevy;
				var chargesRows = card.ElectricityPublicServiceObligationLevy.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(chargesRows);
				var row = chargesRows[0];
				Assert.IsNotNull(row);
				Assert.AreEqual(3, row.ChildElementCount);
				Assert.AreEqual("Public Service Obligation levy per year", row.Cells[0].TextContent.Trim());
				Assert.AreEqual(pso.ExcludingVat.ToString(), row.Cells[1].TextContent.Trim());
				Assert.AreEqual(pso.IncludingVat.ToString(), row.Cells[2].TextContent.Trim());

			}
		}

		public void AssertGasPrices(SmartPlan smartPlan, PlanCardPageComponent card)
		{
			if (smartPlan.FullPricingInformation.GasPricingInfo == null)
			{
				Assert.IsNull(card.GasUnitPrices);
				Assert.IsNull(card.GasStandingCharges);
				Assert.IsNull(card.GasCarbonTax);
				return;
			}

			AssertUnitPrices();
			AssertCarbonTax();
			AssertStandingCharges();

			void AssertUnitPrices()
			{
				if (smartPlan.FullPricingInformation?.GasPricingInfo?.UnitPrices?.Any() != true)
				{
					Assert.IsNull(card.GasUnitPrices);
					return;
				}

				Assert.IsNotNull(card.GasUnitPrices);
				AssertPricingTableHeader(card.GasUnitPrices, "Gas price per unit (cent per kWh)");
				var unitPricesRows = card.GasUnitPrices.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(unitPricesRows);
				var unitPricesIterator = -1;
				foreach (var unitPrice in smartPlan.FullPricingInformation.GasPricingInfo.UnitPrices)
				{
					var row = unitPricesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Standard gas {unitPrice.Description} unit price per kWh", row.Cells[0].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.StandardPrice.ExcludingVat.ToStringCents("N3")}/kWh", row.Cells[1].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.StandardPrice.IncludingVat.ToStringCents("N3")}/kWh", row.Cells[2].TextContent.Trim());

					row = unitPricesRows[++unitPricesIterator];
					Assert.IsNotNull(row);
					Assert.AreEqual(3, row.ChildElementCount);
					Assert.AreEqual($"Effective gas unit price per kWh with {unitPrice.EffectiveDirectDebitDiscountPercentage}% dual fuel, direct debit and online billing discount 1", row.Cells[0].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.EffectivePrice.ExcludingVat.ToStringCents("N3")}/kWh", row.Cells[1].TextContent.Trim());
					Assert.AreEqual($"{unitPrice.EffectivePrice.IncludingVat.ToStringCents("N3")}/kWh", row.Cells[2].TextContent.Trim());
				}
			}

			void AssertCarbonTax()
			{
				if (smartPlan.FullPricingInformation?.GasPricingInfo?.CarbonTax == null)
				{
					Assert.IsNull(card.GasCarbonTax);
					return;
				}

				Assert.IsNotNull(card.GasCarbonTax);
				AssertPricingTableHeader(card.GasCarbonTax, "Gas carbon tax per unit (cent per kWh)");

				var carbonTax = smartPlan.FullPricingInformation?.GasPricingInfo?.CarbonTax;
				var chargesRows = card.GasCarbonTax.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(chargesRows);
				var row = chargesRows[0];

				Assert.IsNotNull(row);
				Assert.AreEqual(3, row.ChildElementCount);
				Assert.AreEqual("Carbon tax per kWh", row.Cells[0].TextContent.Trim());
				Assert.AreEqual($"{carbonTax.ExcludingVat.ToStringCents("N3")}/kWh", row.Cells[1].TextContent.Trim());
				Assert.AreEqual($"{carbonTax.IncludingVat.ToStringCents("N3")}/kWh", row.Cells[2].TextContent.Trim());
			}

			void AssertStandingCharges()
			{
				if (smartPlan.FullPricingInformation?.GasPricingInfo?.StandingCharge == null)
				{
					Assert.IsNull(card.GasStandingCharges);
					return;
				}

				Assert.IsNotNull(card.GasStandingCharges);
				AssertPricingTableHeader(card.GasStandingCharges, "Gas standing charge per year");

				var standingCharge = smartPlan.FullPricingInformation?.GasPricingInfo?.StandingCharge;
				var chargesRows = card.GasStandingCharges.Bodies.FirstOrDefault()?.Rows;
				Assert.IsNotNull(chargesRows);
				var row = chargesRows[0];

				Assert.IsNotNull(row);
				Assert.AreEqual(3, row.ChildElementCount);
				Assert.AreEqual($"Standing charge per year", row.Cells[0].TextContent.Trim());
				Assert.AreEqual(standingCharge.ExcludingVat.ToString(), row.Cells[1].TextContent.Trim());
				Assert.AreEqual(standingCharge.IncludingVat.ToString(), row.Cells[2].TextContent.Trim());
			}
		}
	}
}