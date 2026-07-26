using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Shared.Api.Models;

namespace Booking.Infrastructure.Communication;

public class AuthenticationHandler(HttpClient httpClient) : DelegatingHandler
{
    private string? _token;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_token is null)
            await RenewAuthToken(cancellationToken);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await RenewAuthToken(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task RenewAuthToken(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("/auth/token", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
        _token = result!.Token;
    }
}
