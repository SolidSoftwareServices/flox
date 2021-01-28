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
    public partial class InstallationFactDto : ODataDtoModel
    {
        public override string CollectionName() => "InstallationFacts";
        public override object[] UniqueId()
        {
            return new object[]{InstallationID, Operand, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string InstallationID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string Operand
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(10)]
        public virtual string Season
        {
            get;
            set;
        }

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
        [StringLength(12)]
        public virtual string DocNo
        {
            get;
            set;
        }

        [StringLength(1)]
        public virtual string FactMoveOut
        {
            get;
            set;
        }

        public virtual DateTime? AltValidTo
        {
            get => _AltValidTo;
            set => _AltValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime? _AltValidTo;
        [StringLength(8)]
        public virtual string RateType
        {
            get;
            set;
        }

        [StringLength(10)]
        public virtual string RateFactGroup
        {
            get;
            set;
        }

        public virtual decimal? Value
        {
            get;
            set;
        }

        [StringLength(10)]
        public virtual string OperandValue
        {
            get;
            set;
        }
    }
}