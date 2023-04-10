using hw04.Car;

namespace hw04.Race;

public class Lap
{
    public RaceCar RaceCar { get; }
    public int Number { get; }

    public List<TrackPointPass> TrackPointPasses { get; }
    public TimeSpan CompletionTime { get; }

    public Lap(RaceCar car, int number, List<TrackPointPass> trackPointPasses, TimeSpan completionTime)
    {
        RaceCar = car;
        Number = number;
        TrackPointPasses = trackPointPasses;
        CompletionTime = completionTime;
    }
    
}