using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ContractSaleDetailDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractSaleDetails";
        public override object[] UniqueId()
        {
            return new object[]{ContractHeaderGUID, ContractID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        public virtual Guid ContractHeaderGUID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ContractID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DivisionID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(12)]
        public virtual string BusinessAgreementID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(22)]
        public virtual string PointOfDeliveryGUID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(4)]
        public virtual string BankAccountID
        {
            get;
            set;
        }

        [StringLength(1)]
        public virtual string IncomingPaymentMethodID
        {
            get;
            set;
        }

        public virtual bool? SEPAFlag
        {
            get;
            set;
        }

        [StringLength(3)]
        public virtual string SmartConsentStatusID
        {
            get;
            set;
        }

        [StringLength(2)]
        public virtual string FixedBillingDate
        {
            get;
            set;
        }

        public virtual DateTime? ActualMoveOutDate
        {
            get => _ActualMoveOutDate;
            set => _ActualMoveOutDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ActualMoveOutDate;
        [StringLength(5)]
        public virtual string SalesProcessTypeID
        {
            get;
            set;
        }

        [StringLength(40)]
        public virtual string SalesProcessType
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual bool NewConnection
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractSaleMeterReadingDto> MeterReadings
        {
            get;
            set;
        }

        = new List<ContractSaleMeterReadingDto>();
    }
}