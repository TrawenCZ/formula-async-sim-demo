using hw04.Car;
using hw04.Race;
using hw04.TrackPoints;

namespace hw04;

public class Simulation
{
    public Simulation(Track track)
    {
    }

    public Task<Race.Race> SimulateRaceAsync(List<RaceCar> cars, int numberOfLaps)
    {
        throw new NotImplementedException();
    }

    public Task<List<Lap>> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        throw new NotImplementedException();
    }
}