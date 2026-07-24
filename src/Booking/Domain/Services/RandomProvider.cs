using System;
using Booking.Domain.Abstractions;

namespace Booking.Domain.Services;

public class RandomProvider : IRandomProvider
{
    private readonly Random random = new();

    public int Next(int minValue, int maxValue) => random.Next(minValue, maxValue);
}