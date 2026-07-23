using System.Collections.Generic;
using System.Linq;
using Show.Api.Responses;
using Show.Domain.Models;

namespace Show.Api.Mappers;

public static class ResponseMapper
{
    public static ScreeningResponse ToResponse(this IEnumerable<Screening> screenings) =>
        new(Screenings: screenings
            .Select(screening => new ScreeningResponse.Screening(            
                Id: screening.Id,
                MovieTitle: screening.Movie.Title,
                RoomNumber: screening.Room.Number,
                StartTime: screening.StartTime ))
            .ToList());
    
    public static SeatsResponse ToResponse(this IEnumerable<Seat> seats) => 
        new (Seats: seats
            .Select(seat => new SeatsResponse.Seat(
                Id: seat.Id,
                Row: seat.Row,
                Number: seat.Number))
            .ToList());
}