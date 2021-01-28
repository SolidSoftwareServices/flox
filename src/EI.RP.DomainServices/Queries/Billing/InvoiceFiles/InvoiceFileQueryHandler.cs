using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Billing;
using EI.RP.DomainServices.Infrastructure;
using EI.RP.DomainServices.Infrastructure.InvoicePdf;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;

namespace EI.RP.DomainServices.Queries.Billing.InvoiceFiles
{
	internal class InvoiceFileQueryHandler : QueryHandler<InvoiceFileQuery>
	{
		private readonly IStreamServeRepository _filesRepository;
		private readonly IPdfInvoiceImageResolver _invoiceImageResolver;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ISapRepositoryOfErpUmc _repository;

		public InvoiceFileQueryHandler(ISapRepositoryOfErpUmc repository, IStreamServeRepository filesRepository,
			IPdfInvoiceImageResolver invoiceImageResolver, IDomainQueryResolver queryResolver)
		{
			_repository = repository;
			_filesRepository = filesRepository;
			_invoiceImageResolver = invoiceImageResolver;
			_queryResolver = queryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(InvoiceFile)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			InvoiceFileQuery query)
		{
			//TODO: DO EVENT BUILDERS

			var invoice = await GetInvoice(query);
			var invoiceDocNumber = invoice.ReferenceDocNo.TrimStart('0');

			var result = await BuildFile(invoiceDocNumber, invoice);

			return result.ToOneItemArray().Cast<TQueryResult>();
		}

		
		private async Task<InvoiceFile> BuildFile(string invoiceDocNumber, InvoiceDto invoice)
		{
			//TODO: this is an application service and it should be called from the presentation rules instead
			InvoiceFile result;
			try
			{
				var contractAccountBillToAccount = invoice.ContractAccount.BillToAccount;
				var account = await _queryResolver.GetAccountInfoByAccountNumber(
					invoice.ContractAccount.ContractAccountID,true);
				using (var invoiceDataStream = await _filesRepository.GetInvoiceFileStream(invoiceDocNumber))
				{
					using (var pdfReader = new PdfReader(await invoiceDataStream.ReadAsStreamAsync()))
					{
						using (var ms = new MemoryStream())
						{
							using (var pdfDocument = new PdfDocument(pdfReader, new PdfWriter(ms)))
							{
								var p = new Paragraph();
								for (var pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
								{
									var imgData = await _invoiceImageResolver.ResolvePdfInvoicePagePicture(pageNum,
										string.IsNullOrEmpty(contractAccountBillToAccount.LanguageISO)
											? "EN"
											: contractAccountBillToAccount.LanguageISO,
										account.ClientAccountType);
									var img = new Image(imgData).ScaleAbsolute(595, 842)
										.SetFixedPosition(pageNum, 0, 0);
									var under = new PdfCanvas(pdfDocument.GetPage(pageNum).NewContentStreamBefore(),
										pdfDocument.GetPage(pageNum).GetResources(), pdfDocument);
									p.Add(img);
									new Canvas(under, pdfDocument, pdfDocument.GetDefaultPageSize()).Add(p);
								}
							}

							result = new InvoiceFile(invoiceDocNumber)
							{
								FileData = ms.ToArray()
							};
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new DomainException(ResidentialDomainError.ErrorGeneratingInvoiceFile, ex);
			}

			return result;
		}

		private async Task<InvoiceDto> GetInvoice(InvoiceFileQuery query)
		{
			//TODO: [MM] optimize query
			var contractAccountDto = await _repository
				.NewQuery<ContractAccountDto>()
				.Key(query.AccountNumber)
				.Expand(x => x.Invoices)
				.Expand(x => x.BillToAccount)
				.GetOne();
			//TODO: [MM] client is not hydrating this properly despite the other navigation direction is correct
			var invoice =
				contractAccountDto.Invoices.SingleOrDefault(x =>
					x.ReferenceDocNo == query.ReferenceDocNumber && x.IsBill());
			invoice.ContractAccount = contractAccountDto;

			if (!invoice.PrintDate.HasValue ||
			    string.IsNullOrWhiteSpace(invoice.ReferenceDocNo))
				throw new DomainException(ResidentialDomainError.TheInvoiceDoesNotHaveAFile);

			return invoice;
		}
	}
}