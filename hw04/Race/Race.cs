using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private int _numberOfLaps;
    private readonly IEnumerable<RaceCar> _cars;
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
    }
    
    public Task StartAsync()
    {
        return Task.Run(() =>
        {
            _cars.ToList().ForEach(car => { })
        }
        );
    }
}