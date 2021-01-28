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
    public partial class ContractSaleMeterReadingDto : ODataDtoModel
    {
        public override string CollectionName() => "ContractSaleMeterReadings";
        public override object[] UniqueId()
        {
            return new object[]{ContractHeaderGUID, DivisionID, ContractID, };
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
        [StringLength(2)]
        public virtual string DivisionID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ContractID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(20)]
        public virtual string MeterReadingResultID
        {
            get;
            set;
        }

        [StringLength(18)]
        public virtual string DeviceID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string SerialNumber
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string RegisterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ReadingDateTime
        {
            get => _ReadingDateTime;
            set => _ReadingDateTime = CompensateSapDateTimeBug(value);
        }

        private DateTime _ReadingDateTime;
        [Required(AllowEmptyStrings = true)]
        public virtual string ReadingResult
        {
            get;
            set;
        }

        = "0.0";
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string MeterReadingReasonID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string MeterReadingCategoryID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(10)]
        public virtual string TimeSlot
        {
            get;
            set;
        }

        [StringLength(4)]
        public virtual string MeterReadingNoteID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ReadingUnit
        {
            get;
            set;
        }

        = string.Empty;
        public virtual string Consumption
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ContractSaleMeterReadingDto> DependentMeterReadingResults
        {
            get;
            set;
        }

        = new List<ContractSaleMeterReadingDto>();
    }
}