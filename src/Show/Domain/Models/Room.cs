using System.Collections.Generic;

namespace Show.Domain.Models;

public record Room(int Id, int Number, IReadOnlyList<Seat> Seats);
