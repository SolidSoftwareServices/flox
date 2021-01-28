using System;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Metering
{
    public class RegisterTypeInfo : IQueryResult
    {
        public virtual string RegisterTypeId { get; set; } //PrimaryKey not null

        public virtual string Description { get; set; } // not null
        public static RegisterTypeInfo From(RegisterTypeDto registerTypeDto)
        {
            return registerTypeDto != null
                ? new RegisterTypeInfo()
                {
                    RegisterTypeId = registerTypeDto.RegisterTypeID,
                    Description = registerTypeDto.Description,
                }
                : null;
        }
    }
}
