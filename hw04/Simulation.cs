using hw04.Car;
using hw04.Race;
using hw04.TrackPoints;

namespace hw04;

public class Simulation
{
    private readonly Track _track;
    public Simulation(Track track)
    {
        _track = track;
    }

    public Task<Race.Race> SimulateRaceAsync(List<RaceCar> cars, int numberOfLaps)
    {
        return Task.Run(() => new Race.Race(cars, _track, numberOfLaps, true));
    }

    public Task<List<Lap>> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        throw new NotImplementedException();
    }
}