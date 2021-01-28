using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.ReadOnlyCollections
{
    public partial class RegionDetails : IQueryResult
    {
        public virtual string CountryID { get; set; } //PrimaryKey not null

        public virtual string RegionID { get; set; } //PrimaryKey not null

        public virtual string Name { get; set; } // not null

    }
}