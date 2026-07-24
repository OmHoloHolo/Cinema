using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using Shared.Communication.Dtos;

namespace Booking.Infrastructure.Communication;

public class ShowClient(HttpClient httpClient) : IShowProvider
{
    public async Task<IReadOnlyList<Seat>> GetSeats(int screeningId)
    {
        var response = await httpClient.GetAsync($"/screenings/{screeningId}/seats");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SeatDto>();
        
        return content?
            .Seats
            .Select(seat => new Seat(seat.Id, seat.Row, seat.Number))
            .ToList() ?? throw new InvalidOperationException("Failed to retrieve seats from Show API.");
    }
}