using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class Straight : ITrackPoint
{
    public string Description { get; set; }
    private readonly TimeSpan _averageTime;

    public Straight(string description, TimeSpan averageTime)
    {
        Description = description;
        _averageTime = averageTime;
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.FromResult(new TrackPointPass(this, TimeSpan.Zero, _averageTime * car.StraightSpeed * car.GetCurrentTire().GetSpeed()));
    }
}