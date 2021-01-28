using EI.RP.CoreServices.ErrorHandling;

namespace EI.RP.DataServices.SAP.Clients.Infrastructure.Mappers
{
    public interface ISapErrorMapper
    {
        DomainError Map(string errorCode, string errorMessage);
    }
}
