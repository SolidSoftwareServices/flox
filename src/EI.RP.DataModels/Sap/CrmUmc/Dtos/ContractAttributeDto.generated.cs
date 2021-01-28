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
    public partial class ContractAttributeDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractAttributes";
        public override object[] UniqueId()
        {
            return new object[]{ContractID, ContractStartDate, AttributeID, };
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
        [StringLength(30)]
        [Filterable]
        public virtual string AttributeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(30)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Value
        {
            get;
            set;
        }

        = string.Empty;
    }
}