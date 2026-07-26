namespace Booking.Application.Services;

public interface IRandomGenerator
{
    int Next(int maxValue);
}