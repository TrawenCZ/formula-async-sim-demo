using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private int _numberOfLaps;
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
    }
    
    public Task StartAsync()
    {
        throw new NotImplementedException();
    }
}