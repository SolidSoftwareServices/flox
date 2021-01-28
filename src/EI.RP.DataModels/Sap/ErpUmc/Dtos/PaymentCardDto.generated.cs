using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class PaymentCardDto : ODataDtoModel
    {
        public override string CollectionName() => "PaymentCards";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, PaymentCardID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string PaymentCardID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string PaymentCardTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(25)]
        public virtual string CardNumber
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool StandardFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? ValidFrom
        {
            get => _ValidFrom;
            set => _ValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidFrom;
        public virtual DateTime? ValidTo
        {
            get => _ValidTo;
            set => _ValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ValidTo;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Cardholder
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Issuer
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? IssueDate
        {
            get => _IssueDate;
            set => _IssueDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _IssueDate;
        [Sortable]
        [Filterable]
        public virtual PaymentCardTypeDto PaymentCardType
        {
            get;
            set;
        }
    }
}