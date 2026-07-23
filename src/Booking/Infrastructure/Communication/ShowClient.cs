using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Microsoft.Extensions.Configuration;
using Shared.Communication.Dtos;

namespace Booking.Infrastructure.Communication;

public class ShowClient : IShowProvider
{
    private readonly HttpClient _httpClient;

    public ShowClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(configuration.GetRequiredSection("ShowApi:BaseUrl").Get<string>()!);
    }

    public async Task<IReadOnlyList<Seat>> GetSeats(int roomId)
    {
        var response = await _httpClient.GetAsync($"/rooms/{roomId}/seat");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SeatDto>();
        
        return content?
            .Seats
            .Select(seat => new Seat(seat.Id, seat.Row, seat.Number))
            .ToList() ?? throw new InvalidOperationException("Failed to retrieve seats from Show API.");
    }
}