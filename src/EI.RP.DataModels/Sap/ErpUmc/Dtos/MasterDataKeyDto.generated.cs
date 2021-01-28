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
    public partial class MasterDataKeyDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ContractAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? Date
        {
            get => _Date;
            set => _Date = CompensateSapDateTimeBug(value);
        }

        private DateTime? _Date;
    }
}