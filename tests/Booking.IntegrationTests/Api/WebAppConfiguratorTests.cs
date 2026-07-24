
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NSubstitute.Extensions;
using Shared.Communication.Dtos;
using Booking.Api.Configurations;
using Booking.Api.Requests;
using Booking.Api.Responses;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using System.Collections.Generic;

namespace Booking.IntegrationTests.Api;

public class WebAppConfiguratorTests
{
    private readonly IReservationService _reservationService;
    private readonly ISeatService _seatService;
    private readonly HttpClient _httpClient;

    public WebAppConfiguratorTests()
    {
        var builder = WebApplication.CreateBuilder([]);
        builder.WebHost.UseTestServer();
        _reservationService = Substitute.For<IReservationService>();
        _seatService = Substitute.For<ISeatService>();
        builder.Services.AddSingleton(_reservationService);
        builder.Services.AddSingleton(_seatService);
        var app = builder.Build();
        app.ConfigureRoutes();
        app.Start();
        _httpClient = app.GetTestClient();
    }

    [Fact]
    public async Task WebAppRoutes_AvailableSeats()
    {
        var screeningId = 1;
        var seat = new Seat(Id: 1, Row: "A", Number: 1);
        _seatService.GetAvailableSeats(Arg.Is(screeningId)).Returns([seat]);

        var response = await _httpClient.GetAsync($"/screenings/{screeningId}/available-seats");
        var actual = await response.Content.ReadFromJsonAsync<AvailableSeatsResponse>();

        var expected = new AvailableSeatsResponse(
            Seats: [new(Id: seat.Id, Row: seat.Row, Number: seat.Number)]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task WebAppRoutes_CreateReservation_WhenResourceDoesntExist()
    {
        var reservationId = 1;
        var request = new ReservationCreationRequest(ScreeningId: 1, SeatId: 2);
        var createdReservation = new Reservation(Id: reservationId, ScreeningId: request.ScreeningId, SeatId: request.SeatId!.Value);
        _reservationService.CreateReservation(Arg.Is(request.ScreeningId), Arg.Is(request.SeatId)).Returns(createdReservation);

        var response = await _httpClient.PostAsJsonAsync("/reservations", request);
        var actual = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        var expected = new ReservationResponse(
            Id: createdReservation.Id,
            ScreeningId: createdReservation.ScreeningId,
            SeatId: createdReservation.SeatId);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(expected: expected, actual);
    }

    [Fact]
    public async Task WebAppRoutes_CreateReservation_WhenResourceExists()
    {
        var request = new ReservationCreationRequest(ScreeningId: 1, SeatId: 2);
        _reservationService.CreateReservation(Arg.Any<int>(), Arg.Any<int?>()).Returns((Reservation?)null);

        var response = await _httpClient.PostAsJsonAsync("/reservations", request);

        Assert.Equal(expected: HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task WebAppRoutes_CancelReservation_WhenResourceExists()
    {
        var reservationId = 1;
        _reservationService.CancelReservation(Arg.Is(reservationId)).Returns(true);

        var actual = await _httpClient.DeleteAsync($"/reservations/{reservationId}");

        Assert.Equal(expected: HttpStatusCode.NoContent, actual.StatusCode);
    }

    [Fact]
    public async Task WebAppRoutes_CancelReservation_WhenResourceDoesntExist()
    {
        var reservationId = 1;
        _reservationService.CancelReservation(Arg.Is(reservationId)).Returns(false);

        var actual = await _httpClient.DeleteAsync($"/reservations/{reservationId}");

        Assert.Equal(expected: HttpStatusCode.NotFound, actual.StatusCode);
    }

    [Fact]
    public async Task WebAppRoutes_CreateMultipleReservations_WhenResourceDoesntExist()
    {
        var request = new MultipleReservationCreationRequest(
            Reservations: 
            [
                new(ScreeningId: 1, SeatId: 2), 
                new(ScreeningId: 1, SeatId: 3)
            ]);
        var createdReservations = new Reservation[]
        {
            new(Id: 1, ScreeningId: 1, SeatId: 2),
            new(Id: 2, ScreeningId: 1, SeatId: 3)
        };
        _reservationService.CreateReservations(Arg.Any<IReadOnlyList<ReservationRequest>>()).Returns(createdReservations);

        var response = await _httpClient.PostAsJsonAsync("/multiple-reservations", request);
        var actual = await response.Content.ReadFromJsonAsync<MultipleReservationsResponse>();

        var expected = new MultipleReservationsResponse(Reservations: 
        [
            new(Id: 1, ScreeningId: 1, SeatId: 2),
            new(Id: 2, ScreeningId: 1, SeatId: 3)
        ]);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equivalent(expected: expected, actual);
    }

    [Fact]
    public async Task WebAppRoutes_CreateMultipleReservations_WhenResourceExists()
    {
        var request = new MultipleReservationCreationRequest(Reservations: [new(ScreeningId: 1, SeatId: 2)]);
        _reservationService.CreateReservations(Arg.Any<IReadOnlyList<ReservationRequest>>()).Returns((IReadOnlyList<Reservation>?)null);

        var response = await _httpClient.PostAsJsonAsync("/multiple-reservations", request);

        Assert.Equal(expected: HttpStatusCode.Conflict, response.StatusCode);
    }
}
