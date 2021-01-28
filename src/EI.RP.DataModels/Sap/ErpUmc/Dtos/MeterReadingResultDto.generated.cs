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
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class MeterReadingResultDto : ODataDtoModel
    {
        public override string CollectionName() => "MeterReadingResults";
        public override object[] UniqueId()
        {
            return new object[]{MeterReadingResultID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(20)]
        public virtual string MeterReadingResultID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceID
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
        public virtual string ReadingResult
        {
            get;
            set;
        }

        = "0.0";
        public virtual DateTime? ReadingDateTime
        {
            get => _ReadingDateTime;
            set => _ReadingDateTime = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ReadingDateTime;
        [StringLength(3)]
        public virtual string ReadingUnit
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

        public virtual string Consumption
        {
            get;
            set;
        }

        [StringLength(2)]
        public virtual string MeterReadingReasonID
        {
            get;
            set;
        }

        [StringLength(2)]
        public virtual string MeterReadingCategoryID
        {
            get;
            set;
        }

        [StringLength(1)]
        public virtual string MeterReadingStatusID
        {
            get;
            set;
        }

        [StringLength(18)]
        public virtual string SerialNumber
        {
            get;
            set;
        }

        public virtual bool? MultipleMeterReadingReasonsFlag
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string Vkont
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(241)]
        public virtual string Email
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string Vertrag
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string Lcpe
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string FmoRequired
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string PchaRequired
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(1)]
        public virtual string SkipUsrCheck
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual MeterReadingReasonDto MeterReadingReason
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<MeterReadingReasonDto> MeterReadingReasons
        {
            get;
            set;
        }

        = new List<MeterReadingReasonDto>();
        [Sortable]
        [Filterable]
        public virtual MeterReadingNoteDto MeterReadingNote
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual MeterReadingCategoryDto MeterReadingCategory
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DeviceDto Device
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<MeterReadingResultDto> DependentMeterReadingResults
        {
            get;
            set;
        }

        = new List<MeterReadingResultDto>();
        [Sortable]
        [Filterable]
        public virtual MeterReadingStatusDto MeterReadingStatus
        {
            get;
            set;
        }
    }
}