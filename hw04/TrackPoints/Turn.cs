using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class Turn : ITrackPoint
{
    private static readonly TimeSpan DriveInTime = TimeSpan.FromMilliseconds(5);
    public string Description { get; set; }

    public Turn(string description, TimeSpan averageTime, int carsAllowed)
    {
        Description = description;
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        throw new NotImplementedException();
    }
}