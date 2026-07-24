namespace Booking.Domain.Abstractions;

public interface IRandomProvider
{
    int Next(int minValue, int maxValue);
}