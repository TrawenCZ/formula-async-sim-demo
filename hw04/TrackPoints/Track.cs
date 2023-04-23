using hw04.Car;

namespace hw04.TrackPoints;

public class Track
{
    private readonly List<ITrackPoint> _trackPoints;
    private List<ITrackPoint>? _lapWithPitLaneEntry;
    private List<ITrackPoint>? _lapWithPitLaneExit;

    public Track()
    {
        _trackPoints = new List<ITrackPoint>();
    }

    public Track AddTurn(string description, TimeSpan averageTime, int carsAllowed)
    {
        var newTurn = new Turn(description, averageTime, carsAllowed);
        _trackPoints.Add(newTurn);
        if (_lapWithPitLaneExit != null) _lapWithPitLaneExit.Add(newTurn);
        return this;
    }

    public Track AddStraight(string description, TimeSpan averageTime)
    {
        var newStraight = new Straight(description, averageTime);
        _trackPoints.Add(newStraight);
        if (_lapWithPitLaneExit != null) _lapWithPitLaneExit.Add(newStraight);
        return this;
    }

    public Track AddPitLane(TimeSpan entryTime, TimeSpan exitTime, List<Team> teams,
        int nextPoint)
    {
        PitLane pitLane = new PitLane("PitLane", teams);
        Turn pitLaneEntry = new Turn("PitLane Entry", entryTime, 1);
        Turn pitLaneExit = new Turn("PitLane Exit", exitTime, 1);

        _lapWithPitLaneEntry = new List<ITrackPoint>(_trackPoints) { pitLaneEntry }.ToList();

        _lapWithPitLaneExit = new List<ITrackPoint>() { pitLane, pitLaneExit }.Concat(_trackPoints.Skip(nextPoint)).ToList();

        return this;
    }

    /// <summary>
    /// Vrací seznam trackpoints s ohledem na to,
    /// zda má auto jít na konci kola vyměnit pneumatiky nebo ne
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    public IEnumerable<ITrackPoint> GetLap(RaceCar car)
    {
        if (_lapWithPitLaneEntry == null || _lapWithPitLaneExit == null) throw new Exception("PitLane is not set");
        if (car.WentToPitStop)
        {
            car.WentToPitStop = false;
            return _lapWithPitLaneExit;
        }
        if (car.ShouldChangeTires()) 
        {
            car.WentToPitStop = true;
            return _lapWithPitLaneEntry;
        }
        return _trackPoints;
    }

}