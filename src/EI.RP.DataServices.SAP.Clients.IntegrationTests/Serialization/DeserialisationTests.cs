﻿using System.Collections.Generic;
using EI.RP.CoreServices.Serialization;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using NUnit.Framework;
using EI.RP.DataServices.SAP.Clients.Infrastructure;
namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Serialization
{
	[TestFixture]
	class DeserialisationTests
	{

		public static IEnumerable<TestCaseData> CanDeserialize_Cases()
		{
			yield return new TestCaseData("{\r\n  \"__metadata\": {\r\n    \"id\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSales(guid'000D3AB0-1648-1EDA-ADAC-EA093E0406A2')\",\r\n    \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSales(guid'000D3AB0-1648-1EDA-ADAC-EA093E0406A2')\",\r\n    \"type\": \"CRM_UTILITIES_UMC.ContractSale\"\r\n  },\r\n  \"SalesOrderID\": \"\",\r\n  \"ContractHeaderGUID\": \"000D3AB0-1648-1EDA-ADAC-EA093E0406A2\",\r\n  \"AccountID\": \"1003658479\",\r\n  \"ContractStartDate\": \"2020-06-23T00:00:00Z\",\r\n  \"CheckModeOnly\": true,\r\n  \"ConsumerID\": \"ROI_RES_PORTAL_HOMEMOVE\",\r\n  \"MoveOutDate\": \"2020-06-22T00:00:00Z\",\r\n  \"ProductProposalResult\": {\r\n    \"__metadata\": {\r\n      \"id\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')\",\r\n      \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')\",\r\n      \"type\": \"CRM_UTILITIES_UMC.ProductProposalResult\"\r\n    },\r\n    \"SearchParameters\": {\r\n      \"__metadata\": {\r\n        \"type\": \"CRM_UTILITIES_UMC.ProductProposalParams\"\r\n      },\r\n      \"SalesOrigin\": \"WEB\",\r\n      \"AccountID\": \"1003658479\",\r\n      \"PaperlessBilling\": true,\r\n      \"BonusAmount\": \"0000\",\r\n      \"ElecFlag\": true,\r\n      \"ElecBusinessAgreementID\": \"904756065\",\r\n      \"ElecSalesType\": \"RETEN\",\r\n      \"ElecDuOSGroup\": \"DG1\",\r\n      \"ElecDirectDebit\": false,\r\n      \"ElecMeterType\": \"CR\",\r\n      \"ElecRegisterConfig\": \"\",\r\n      \"ElecCTF\": \"\",\r\n      \"ElecQuarterHourly\": false,\r\n      \"ElecAnnualConsumption\": \"0\",\r\n      \"GasFlag\": false,\r\n      \"GasBusinessAgreementID\": \"\",\r\n      \"GasSalesType\": \"RETEN\",\r\n      \"GasBand\": \"\",\r\n      \"GasDirectDebit\": false,\r\n      \"GasMeterType\": \"CR\",\r\n      \"GasAnnualConsumption\": \"0\"\r\n    },\r\n    \"BundleID\": \"\",\r\n    \"ElecProductID\": \"RE_ELECTRICITYPLAN\",\r\n    \"GasProductID\": \"\",\r\n    \"ElecDiscountID\": \"2051\",\r\n    \"GasDiscountID\": \"0000\",\r\n    \"ElecProduct\": {\r\n      \"__deferred\": {\r\n        \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')/ElecProduct\"\r\n      }\r\n    },\r\n    \"GasProduct\": {\r\n      \"__deferred\": {\r\n        \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')/GasProduct\"\r\n      }\r\n    },\r\n    \"ElecDiscount\": {\r\n      \"__deferred\": {\r\n        \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')/ElecDiscount\"\r\n      }\r\n    },\r\n    \"GasDiscount\": {\r\n      \"__deferred\": {\r\n        \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ProductProposalResults(BundleID='',ElecProductID='RE_ELECTRICITYPLAN',GasProductID='',ElecDiscountID='2051',GasDiscountID='0000')/GasDiscount\"\r\n      }\r\n    }\r\n  },\r\n  \"SaleDetails\": {\r\n    \"results\": [\r\n      {\r\n        \"__metadata\": {\r\n          \"id\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleDetails(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',ContractID='2012618020')\",\r\n          \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleDetails(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',ContractID='2012618020')\",\r\n          \"type\": \"CRM_UTILITIES_UMC.ContractSaleDetail\"\r\n        },\r\n        \"ContractHeaderGUID\": \"00000000-0000-0000-0000-000000000000\",\r\n        \"ContractID\": \"2012618020\",\r\n        \"DivisionID\": \"01\",\r\n        \"BusinessAgreementID\": \"904756065\",\r\n        \"PointOfDeliveryGUID\": \"00qwkCZ87kgjgacwdMdVZW\",\r\n        \"BankAccountID\": \"\",\r\n        \"IncomingPaymentMethodID\": \"\",\r\n        \"SEPAFlag\": false,\r\n        \"SmartConsentStatusID\": \"000\",\r\n        \"FixedBillingDate\": \"\",\r\n        \"ActualMoveOutDate\": \"2020-06-22T00:00:00Z\",\r\n        \"SalesProcessTypeID\": \"HOMEM\",\r\n        \"SalesProcessType\": \"Home-Move\",\r\n        \"NewConnection\": false,\r\n        \"MeterReadings\": {\r\n          \"results\": [\r\n            {\r\n              \"__metadata\": {\r\n                \"id\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleMeterReadings(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',DivisionID='01',ContractID='2012618020')\",\r\n                \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleMeterReadings(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',DivisionID='01',ContractID='2012618020')\",\r\n                \"type\": \"CRM_UTILITIES_UMC.ContractSaleMeterReading\"\r\n              },\r\n              \"ContractHeaderGUID\": \"00000000-0000-0000-0000-000000000000\",\r\n              \"DivisionID\": \"01\",\r\n              \"ContractID\": \"2012618020\",\r\n              \"MeterReadingResultID\": \"\",\r\n              \"DeviceID\": \"13268062\",\r\n              \"SerialNumber\": \"35477938\",\r\n              \"RegisterID\": \"001\",\r\n              \"ReadingDateTime\": \"2020-06-22T00:00:00Z\",\r\n              \"ReadingResult\": \"3211.00000000000000\",\r\n              \"MeterReadingReasonID\": \"03\",\r\n              \"MeterReadingCategoryID\": \"02\",\r\n              \"TimeSlot\": \"\",\r\n              \"MeterReadingNoteID\": \"\",\r\n              \"ReadingUnit\": \"\",\r\n              \"Consumption\": \"0.00000000000000\",\r\n              \"DependentMeterReadingResults\": null\r\n            },\r\n            {\r\n              \"__metadata\": {\r\n                \"id\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleMeterReadings(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',DivisionID='01',ContractID='')\",\r\n                \"uri\": \"http://virtrhl047s.cld1.tld.int:8000/sap/opu/odata/sap/CRM_UTILITIES_UMC/ContractSaleMeterReadings(ContractHeaderGUID=guid'00000000-0000-0000-0000-000000000000',DivisionID='01',ContractID='')\",\r\n                \"type\": \"CRM_UTILITIES_UMC.ContractSaleMeterReading\"\r\n              },\r\n              \"ContractHeaderGUID\": \"00000000-0000-0000-0000-000000000000\",\r\n              \"DivisionID\": \"01\",\r\n              \"ContractID\": \"\",\r\n              \"MeterReadingResultID\": \"\",\r\n              \"DeviceID\": \"13268432\",\r\n              \"SerialNumber\": \"70552598\",\r\n              \"RegisterID\": \"001\",\r\n              \"ReadingDateTime\": \"2020-06-23T00:00:00Z\",\r\n              \"ReadingResult\": \"5555.00000000000000\",\r\n              \"MeterReadingReasonID\": \"06\",\r\n              \"MeterReadingCategoryID\": \"02\",\r\n              \"TimeSlot\": \"\",\r\n              \"MeterReadingNoteID\": \"\",\r\n              \"ReadingUnit\": \"\",\r\n              \"Consumption\": \"0.00000000000000\",\r\n              \"DependentMeterReadingResults\": null\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}");
		}
		[TestCaseSource(nameof(CanDeserialize_Cases))]
		public void CanDeserialize(string jsonValue)
		{
			var actual = jsonValue.ODataJsonResultToObject<ContractSaleDto>();
		}
	}
}