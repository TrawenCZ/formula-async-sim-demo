using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class Turn : ITrackPoint
{
    private static readonly TimeSpan DriveInTime = TimeSpan.FromMilliseconds(5);
    public string Description { get; set; }
    private readonly TimeSpan _averageTime;
    private readonly int _carsAllowed;
    private List<Tuple<DateTime, TimeSpan>> _currentCarsTasks;

    public Turn(string description, TimeSpan averageTime, int carsAllowed)
    {
        Description = description;
        _averageTime = averageTime;
        _carsAllowed = carsAllowed;
        _currentCarsTasks = new List<Tuple<DateTime, TimeSpan>>();
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        if (_currentCarsTasks.Count < _carsAllowed)
        {
            var drivingTime = _averageTime * car.TurnSpeed * car.GetCurrentTire().GetSpeed();
            _currentCarsTasks.Add(new Tuple<DateTime, TimeSpan>(DateTime.Now, drivingTime));
            return Task.FromResult(new TrackPointPass(this, DriveInTime, drivingTime));
        }

        
    }

    private 
}