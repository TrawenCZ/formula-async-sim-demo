using hw04.Car;
using hw04.Race;
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

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(() =>
            {
                var turnEnteredDateTime = DateTime.Now;
                Stopwatch stopwatch = Stopwatch.StartNew();

                _semaphore.Wait();
                stopwatch.Stop();
                TimeSpan waitingTime = stopwatch.Elapsed;
                Thread.Sleep(DriveInTime);
                _semaphore.Release();
                
                TimeSpan drivingTime = _averageTime * car.TurnSpeed * car.GetCurrentTire().GetSpeed();
                Thread.Sleep(drivingTime);
                return new TrackPointPass(this, waitingTime, DriveInTime + drivingTime);
           });
    }
}