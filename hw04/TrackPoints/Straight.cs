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

    public async Task<TrackPointPass> PassAsync(RaceCar car)
    {
        var timeBeforeDrive = car.Stopwatch.Elapsed;
        var drivingTime = _averageTime * car.StraightSpeed * car.GetCurrentTire().GetSpeed();
        await Task.Delay(drivingTime);
        return new TrackPointPass(this, TimeSpan.Zero, car.Stopwatch.Elapsed - timeBeforeDrive); 
    }
}