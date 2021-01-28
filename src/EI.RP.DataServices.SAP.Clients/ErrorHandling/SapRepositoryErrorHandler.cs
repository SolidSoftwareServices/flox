using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Serialization;
using EI.RP.DataServices.SAP.Clients.Infrastructure.Mappers;
using Ei.Rp.DomainErrors;
using NLog;

namespace EI.RP.DataServices.SAP.Clients.ErrorHandling
{
	class SapRepositoryErrorHandler : ISapResultStatusHandler
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IUserSessionProvider _userSessionProvider;
        private readonly ISapErrorMapper _errorMapper;

        public SapRepositoryErrorHandler(IUserSessionProvider userSessionProvider,
            ISapErrorMapper errorMapper)
        {
            _userSessionProvider = userSessionProvider;
            _errorMapper = errorMapper;
        }

		public async Task EnsureSuccessfulResponse(HttpResponseMessage response, bool isOData = true, bool clearSessionOnAuthenticationError = true)
		{
			
			if (!response.IsSuccessStatusCode)
			{
				switch (response.StatusCode)
				{
					case HttpStatusCode.Unauthorized:
                        if (clearSessionOnAuthenticationError)

                        {
                            await _userSessionProvider.EndCurrentSession();
                        }
						throw new DomainException(ResidentialDomainError.AuthenticationError);
					case HttpStatusCode.Forbidden:
						throw new DomainException(ResidentialDomainError.AuthorizationError);
					default:
						await BuildAndThrowDomainException();
						break;
				}
			}

			async Task<(string errorCode, string errorMessage, string errorDto)> PopulateErrorData()
			{
				var result = await response.Content.ReadAsStringAsync();
				string errorCode;
				string errorMessage;
				errorCode = isOData ? ParseODataError() : ParseNoOdataError();

				return (errorCode, errorMessage, result);

				string ParseNoOdataError()
				{
					var sapException = result.JsonToObject<NoODataException>();
					errorCode = sapException.Error.Code;
					errorMessage = sapException.Error.Message?.Value;
					return errorCode;
				}

				string ParseODataError()
				{
					ODataException oDataException;
					try
					{
						oDataException = result.XmlToObject<ODataException>();
						errorCode = oDataException.ErrorCode;
						errorMessage = oDataException.ErrorMessage?.Value;
					}
					catch (InvalidOperationException)
					{
						errorCode = "Unknown";
						errorMessage = result;
					}

					return errorCode;
				}
			}

			async Task BuildAndThrowDomainException()
			{
				string errorCode;
				string errorMessage;
				string errorDto;
				(errorCode, errorMessage, errorDto) = await PopulateErrorData();
                if (errorCode.Equals("0"))
                {
                    return;
                }

				var domainError = _errorMapper.Map(errorCode, errorMessage);
                if (domainError.Equals(DomainError.Undefined))
                {
                    Logger.Warn(() => $"The following error was unhandled:{Environment.NewLine}"
                                      + $"Http verb:{response.RequestMessage.Method}-{response.RequestMessage.RequestUri}{Environment.NewLine}"
                                      + $"{errorDto}");
				}
                throw new DomainException(domainError, extraInfo: errorDto);
			}
		}
	}
}