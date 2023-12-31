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

    public async Task<Race.Race> SimulateRaceAsync(List<RaceCar> cars, int numberOfLaps)
    {
        var newRace = new Race.Race(cars, _track, numberOfLaps);
        await newRace.StartAsync();
        return newRace;
    }

    public async Task<List<Lap>> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        var race = new Race.Race(new List<RaceCar>() { car }, _track, numberOfLaps);
        await race.StartAsync();
        return race.RaceResults == null ? new List<Lap>() : race.RaceResults[car].ToList();
    }
}