
using System;
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
using Show.Api.Configurations;
using Show.Api.Responses;
using Show.Domain.Abstractions;
using Show.Domain.Models;

namespace Show.IntegrationTests.Api;

public class WebAppConfiguratorTests
{
    private readonly IShowService _showService;
    private readonly HttpClient _httpClient;

    public WebAppConfiguratorTests()
    {
        var builder = WebApplication.CreateBuilder([]);
        builder.WebHost.UseTestServer();
        _showService = Substitute.For<IShowService>();
        builder.Services.AddSingleton(_showService);
        var app = builder.Build();
        app.ConfigureRoutes();
        app.Start();
        _httpClient = app.GetTestClient();
    }

    [Fact]
    public async Task WebAppRoutes_Screenings()
    {
        var screening = new Screening(
            Id: 1, 
            Room: new Room(Id: 1, Number: 1, Seats: []), 
            Movie: new Movie(Id: 1, Title: "Good Movie"), 
            StartTime: new DateTime(2026, 7, 24, 12, 0, 0));
        _showService.GetScreenings().Returns([screening]);

        var response = await _httpClient.GetAsync("/screenings");
        var actual = await response.Content.ReadFromJsonAsync<ScreeningResponse>();

        var expected = new ScreeningResponse(
            Screenings: 
            [
                new ScreeningResponse.Screening(
                    Id: screening.Id,
                    MovieTitle: screening.Movie.Title,
                    RoomNumber: screening.Room.Number,
                    StartTime: screening.StartTime)
            ]);
        Assert.Equivalent(expected: expected, actual);
    }

    [Fact]
    public async Task WebAppRoutes_Seats()
    {
        var screeningId = 1;
        var seat = new Seat(
            Id: 1, 
            Row: "A",
            Number: 1);
        _showService.GetSeats(Arg.Is(screeningId)).Returns([seat]);

        var response = await _httpClient.GetAsync($"/screenings/{screeningId}/seats");
        var actual = await response.Content.ReadFromJsonAsync<SeatDto>();

        var expected = new SeatDto(
            Seats: 
            [
                new SeatDto.Seat(Id: seat.Id, Row: seat.Row, Number: seat.Number)
            ]);
        Assert.Equivalent(expected: expected, actual);
    }
}