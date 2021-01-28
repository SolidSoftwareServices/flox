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
    public partial class ContractSaleDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractSales";
        public override object[] UniqueId()
        {
            return new object[]{ContractHeaderGUID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [StringLength(10)]
        public virtual string SalesOrderID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual Guid ContractHeaderGUID
        {
            get;
            set;
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
        public virtual DateTime ContractStartDate
        {
            get => _ContractStartDate;
            set => _ContractStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _ContractStartDate;
        [Required(AllowEmptyStrings = true)]
        public virtual bool CheckModeOnly
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(36)]
        public virtual string ConsumerID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? MoveOutDate
        {
            get => _MoveOutDate;
            set => _MoveOutDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _MoveOutDate;
        [Sortable]
        [Filterable]
        public virtual ProductProposalResultDto ProductProposalResult
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractSaleDetailDto> SaleDetails
        {
            get;
            set;
        }

        = new List<ContractSaleDetailDto>();
    }
}