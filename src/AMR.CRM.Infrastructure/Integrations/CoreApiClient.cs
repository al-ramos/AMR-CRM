using System.Net.Http.Json;
using System.Text.Json;
using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AMR.CRM.Infrastructure.Integrations;

public class CoreApiClient(HttpClient httpClient, ILogger<CoreApiClient> logger) : ICoreApiClient
{
    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<ClienteCoreDto>> GetClientesAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<ClienteCoreDto>>(
                "api/clientes", _jsonOpts, ct);
            return result ?? [];
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "AMR-Core offline ao buscar clientes.");
            throw new CoreApiUnavailableException("AMR-Core está offline ou inacessível.", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Timeout ao contatar AMR-Core.");
            throw new CoreApiUnavailableException("Timeout ao contatar AMR-Core.", ex);
        }
    }
}
