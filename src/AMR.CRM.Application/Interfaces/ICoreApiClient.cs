using AMR.CRM.Application.DTOs;

namespace AMR.CRM.Application.Interfaces;

public interface ICoreApiClient
{
    Task<List<ClienteCoreDto>> GetClientesAsync(CancellationToken ct = default);
}

public class CoreApiUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
