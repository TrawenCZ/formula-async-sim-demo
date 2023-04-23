using hw04.Car;
using hw04.Race;
using System.Diagnostics;

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
        var ticksAtStart = Stopwatch.GetTimestamp();
        await Task.Delay(TimeSpan.FromMilliseconds(_averageTime.TotalMilliseconds * car.StraightSpeed * car.GetCurrentTire().GetSpeed()));
        return new TrackPointPass(this, TimeSpan.Zero, Stopwatch.GetElapsedTime(ticksAtStart)); 
    }
}