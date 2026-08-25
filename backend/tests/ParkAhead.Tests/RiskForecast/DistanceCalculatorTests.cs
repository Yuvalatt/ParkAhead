using ParkAhead.Application.RiskForecast;

namespace ParkAhead.Tests.RiskForecast;

public class DistanceCalculatorTests
{
    [Fact]
    public void Distance_between_identical_points_is_zero()
    {
        var distance = DistanceCalculator.CalculateKilometers(32.08, 34.78, 32.08, 34.78);

        Assert.Equal(0, distance, 6);
    }

    [Fact]
    public void Distance_is_symmetric()
    {
        var a = DistanceCalculator.CalculateKilometers(32.08, 34.78, 32.10, 34.80);
        var b = DistanceCalculator.CalculateKilometers(32.10, 34.80, 32.08, 34.78);

        Assert.Equal(a, b, 9);
    }

    [Fact]
    public void One_degree_of_latitude_is_approximately_111_kilometers()
    {
        var distance = DistanceCalculator.CalculateKilometers(0, 0, 1, 0);

        Assert.InRange(distance, 110.5, 111.5);
    }

    [Fact]
    public void Known_distance_between_tel_aviv_landmarks_is_plausible()
    {
        // Rothschild Blvd (32.0656, 34.7742) to Charles Clore Park (32.0575, 34.7638):
        // roughly 1.3 km apart in reality.
        var distance = DistanceCalculator.CalculateKilometers(32.0656, 34.7742, 32.0575, 34.7638);

        Assert.InRange(distance, 1.0, 1.6);
    }
}
