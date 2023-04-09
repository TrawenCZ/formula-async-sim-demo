using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class Turn : ITrackPoint
{
    private static readonly TimeSpan DriveInTime = TimeSpan.FromMilliseconds(5);
    public string Description { get; set; }
    private readonly TimeSpan _averageTime;
    private readonly int _carsAllowed;
    private List<Task<TimeSpan>> _currentTasksInEntry;
    private SemaphoreSlim _semaphore;

    public Turn(string description, TimeSpan averageTime, int carsAllowed)
    {
        Description = description;
        _averageTime = averageTime;
        _carsAllowed = carsAllowed;
        _currentTasksInEntry = new List<Task<TimeSpan>>();
        _semaphore = new SemaphoreSlim(_carsAllowed);
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(() =>
            {
                var turnEnteredDateTime = DateTime.Now;
                var waitingTime = DriveInTime;
                _semaphore.Wait();
                Thread.Sleep(waitingTime);
                _semaphore.Release();
                waitingTime += DateTime.Now - turnEnteredDateTime;
                var drivingTime = _averageTime * car.TurnSpeed * car.GetCurrentTire().GetSpeed();
                Thread.Sleep(drivingTime);
                return new TrackPointPass(this, waitingTime, drivingTime);
           });
    }
}