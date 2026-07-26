
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
using Booking.Application.Handlers;
using Booking.Application.Models;
using Booking.Api.Configurations;
using Booking.Api.Requests;
using Booking.Api.Responses;
using Booking.Domain.Models;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;
using System.IO;

namespace Booking.IntegrationTests.Api;

public class WebAppConfiguratorTests
{
    private readonly ICreateReservationHandler _createReservationHandler;
    private readonly ICreateReservationsHandler _createReservationsHandler;
    private readonly ICancelReservationHandler _cancelReservationHandler;
    private readonly IGetAvailableSeatsHandler _getAvailableSeatsHandler;
    private readonly HttpClient _httpClient;

    public WebAppConfiguratorTests()
    {
        var builder = WebApplication.CreateBuilder([]);
        builder.WebHost.UseTestServer();
        
        _createReservationHandler = Substitute.For<ICreateReservationHandler>();
        _createReservationsHandler = Substitute.For<ICreateReservationsHandler>();
        _cancelReservationHandler = Substitute.For<ICancelReservationHandler>();
        _getAvailableSeatsHandler = Substitute.For<IGetAvailableSeatsHandler>();
        builder.Services.AddSingleton(_createReservationHandler);
        builder.Services.AddSingleton(_createReservationsHandler);
        builder.Services.AddSingleton(_cancelReservationHandler);
        builder.Services.AddSingleton(_getAvailableSeatsHandler);
        var app = builder.Build();
        app.ConfigureRoutes();
        app.Start();
        _httpClient = app.GetTestClient();
    }

    [Fact]
    public async Task WebAppRoutes_AvailableSeats()
    {
        var screeningId = 1;
        var seat = new Seat(Id: 1, Row: "A", Number: 1, Reservation: null);
        _getAvailableSeatsHandler.Handle(Arg.Is(screeningId)).Returns([seat]);

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
        var screeningId = 1;
        var request = new ReservationCreationRequest(SeatId: 2);
        var createdReservation = new Reservation.Existing(
            Id: reservationId, 
            ScreeningId: screeningId, 
            SeatId: request.SeatId!.Value);
        _createReservationHandler.Handle(Arg.Is(screeningId), Arg.Is(request.SeatId)).Returns(createdReservation);

        var response = await _httpClient.PostAsJsonAsync($"screenings/{screeningId}/reservations", request);
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
        var request = new ReservationCreationRequest(SeatId: 2);
        _cancelReservationHandler
            .Handle(Arg.Any<int>(), Arg.Any<int>())
            .ThrowsAsync<InvalidDataException>();

        var response = await _httpClient.PostAsJsonAsync("screenings/1/reservations", request);

        Assert.Equal(expected: HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task WebAppRoutes_CancelReservation_WhenResourceExists()
    {
        var actual = await _httpClient.DeleteAsync("screenings/1/reservations/1");

        Assert.Equal(expected: HttpStatusCode.NoContent, actual.StatusCode);
    }

    [Fact]
    public async Task WebAppRoutes_CancelReservation_WhenResourceDoesntExist()
    {
        var reservationId = 1;
        var screeningId = 1;
        _cancelReservationHandler
            .Handle(Arg.Is(screeningId), Arg.Is(reservationId))
            .ThrowsAsync<InvalidDataException>();

        var actual = await _httpClient.DeleteAsync($"screenings/{screeningId}/reservations/{reservationId}");

        Assert.Equal(expected: HttpStatusCode.Conflict, actual.StatusCode);
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
        var createdReservations = new Reservation.Existing[]
        {
            new(Id: 1, ScreeningId: 1, SeatId: 2),
            new(Id: 2, ScreeningId: 1, SeatId: 3)
        };
        _createReservationsHandler.Handle(Arg.Any<IReadOnlyList<ReservationRequest>>()).Returns(createdReservations);

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
        _createReservationsHandler
            .Handle(Arg.Any<IReadOnlyList<ReservationRequest>>())
            .ThrowsAsync<InvalidDataException>();

        var response = await _httpClient.PostAsJsonAsync("/multiple-reservations", request);

        Assert.Equal(expected: HttpStatusCode.Conflict, response.StatusCode);
    }
}
