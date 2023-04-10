using hw04.Car;
using hw04.Race;

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
        _semaphore = new SemaphoreSlim(carsAllowed);
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(() =>
            {
                var turnEnteredDateTime = DateTime.Now;
                _semaphore.Wait();
                var waitingTime = DateTime.Now - turnEnteredDateTime;
                Thread.Sleep(DriveInTime);
                _semaphore.Release();              
                var drivingTime = _averageTime * car.TurnSpeed * car.GetCurrentTire().GetSpeed();
                Thread.Sleep(drivingTime);
                return new TrackPointPass(this, waitingTime, DriveInTime + drivingTime);
           });
    }
}