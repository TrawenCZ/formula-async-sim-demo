using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
    }


    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        throw new NotImplementedException();
    }
}