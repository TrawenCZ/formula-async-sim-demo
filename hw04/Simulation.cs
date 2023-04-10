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
        return Task.Run(() =>
        {
            var newRace = new Race.Race(cars, _track, numberOfLaps, true);
            newRace.StartAsync();
            return newRace;
        });
    }

    public Task<List<Lap>> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        return Task.Run(() =>
        {
            return new Race.Race(new List<RaceCar>(new RaceCar[] { car }), _track, numberOfLaps, true).StartAsync().Result[car];
        });
    }
}