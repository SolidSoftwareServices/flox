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
    public partial class ContractDateDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractDates";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, ContractStartDate, DateID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
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
        public virtual DateTime ContractStartDate
        {
            get => _ContractStartDate;
            set => _ContractStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _ContractStartDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        [Filterable]
        public virtual string DateID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(40)]
        public virtual string Description
        {
            get;
            set;
        }

        public virtual DateTime? DateValue
        {
            get => _DateValue;
            set => _DateValue = CompensateSapDateTimeBug(value);
        }

        private DateTime? _DateValue;
    }
}