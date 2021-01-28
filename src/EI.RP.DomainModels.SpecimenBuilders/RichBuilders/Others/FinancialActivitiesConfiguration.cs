using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.Activity;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Others
{
	public class FinancialActivitiesConfiguration
	{
		private DomainFacade Domain { get; }
		private int _numInvoices;
		private DateTime? _minInvoiceDate;
		private DateTime? _maxInvoiceDate;
		private bool _hasOverDueInvoice;

		public FinancialActivitiesConfiguration(DomainFacade domain)
		{
			Domain = domain;
		}
		public IEnumerable<AccountBillingActivity> AccountActivities { get; private set; }

		public FinancialActivitiesConfiguration WithInvoices(
			int numInvoices, 
			DateTime? minDate = null,
			DateTime? maxDate = null)
		{
			_numInvoices = numInvoices;
			_minInvoiceDate = minDate;
			_maxInvoiceDate = maxDate;
			return this;
		}

		public FinancialActivitiesConfiguration WithOverDueInvoice()
		{
			_hasOverDueInvoice = true;
			return this;
		}

		public void Execute(string accountNumber, PaymentMethodType invoicesPaymentType)
		{
			ConfigureInvoices(accountNumber, invoicesPaymentType);
		}

		private void ConfigureInvoices(string accountNumber, PaymentMethodType paymentType)
		{
			if (_numInvoices == 0) return;
			AccountBillingActivity[] accountBillingActivities;
			do
			{
				accountBillingActivities = Domain.ModelsBuilder.Build<AccountBillingActivity>()
					.With(x => x.AccountNumber, accountNumber)
					.With(x => x.ReadingType, MeterReadingCategoryType.Actual)
					.With(x => x.PaymentMethod, paymentType)
					.CreateMany(_numInvoices * 3)
					.Where(x => x.Source == AccountBillingActivity.ActivitySource.Invoice).Select(x =>
					{
						x.Description = "bill";
						return x;
					})
					.ToArray();
			} while (accountBillingActivities.All(x => x.Source != AccountBillingActivity.ActivitySource.Invoice));

			AccountActivities = (AccountActivities ?? new AccountBillingActivity[0].AsEnumerable())
				.Union(accountBillingActivities)
				.OrderByDescending(x => x.OriginalDate);

			ArrangeInvoices();
			ArrangeAll();
			if (_hasOverDueInvoice)
			{
				var invoice = AccountActivities.First(x => x.Description == "bill");
				invoice.DueDate = DateTime.Now.AddDays(-10);
				invoice.InvoiceStatus = InvoiceStatus.Open;
			}

			void ArrangeInvoices()
			{
				var query = new AccountBillingActivityQuery
				{
					AccountNumber = accountNumber,
					Source = AccountBillingActivity.ActivitySource.Invoice,
				};
				var invoices = accountBillingActivities.Where(x=>x.IsBill()).ToArray();
				Domain
					.QueryResolver
					.ExpectQuery(query, invoices);

				query = query.CloneDeep();
				query.MinDate = _minInvoiceDate ?? query.MinDate;
				query.MaxDate = _maxInvoiceDate ?? query.MaxDate;
				Domain
					.QueryResolver
					.ExpectQuery(query, invoices);
			}
			void ArrangeAll()
			{
				var query = new AccountBillingActivityQuery
				{
					AccountNumber = accountNumber,
				};
				Domain
					.QueryResolver
					.ExpectQuery(query, accountBillingActivities);

				query = query.CloneDeep();
				query.MinDate = _minInvoiceDate ?? query.MinDate;
				query.MaxDate = _maxInvoiceDate ?? query.MaxDate;
				Domain
					.QueryResolver
					.ExpectQuery(query, accountBillingActivities);
			}
		}
	}
}