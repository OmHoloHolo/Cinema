using System;

namespace Booking.Application.Services;

public class RandomGenerator : IRandomGenerator
{
    private readonly Random random = new();

    public int Next(int maxValue) => random.Next(0, maxValue);
}
