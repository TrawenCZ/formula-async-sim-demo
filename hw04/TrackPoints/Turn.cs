using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class Turn : ITrackPoint
{
    private static readonly TimeSpan DriveInTime = TimeSpan.FromMilliseconds(5);
    public string Description { get; set; }
    private readonly TimeSpan _averageTime;
    private SemaphoreSlim _semaphore;

    public Turn(string description, TimeSpan averageTime, int carsAllowed)
    {
        Description = description;
        _averageTime = averageTime;
        _semaphore = new SemaphoreSlim(carsAllowed, carsAllowed);
    }

    public async Task<TrackPointPass> PassAsync(RaceCar car)
    {
        var ticksAtStart = Stopwatch.GetTimestamp();
        await _semaphore.WaitAsync();
        TimeSpan waitingTime = Stopwatch.GetElapsedTime(ticksAtStart);
        await Task.Delay(DriveInTime);
        _semaphore.Release();

        await Task.Delay(TimeSpan.FromMilliseconds(_averageTime.TotalMilliseconds * car.TurnSpeed * car.GetCurrentTire().GetSpeed()));
        return new TrackPointPass(this, waitingTime, Stopwatch.GetElapsedTime(waitingTime.Ticks + ticksAtStart));
    }
}