using hw04.Car;

namespace hw04.TrackPoints;

public class Track
{
    private readonly List<ITrackPoint> _trackPoints;
    private Turn? _pitLaneEntry;
    private Turn? _pitLaneExit;
    private PitLane? _pitLane;

    public Track()
    {
        _trackPoints = new List<ITrackPoint>();
    }

    public Track AddTurn(string description, TimeSpan averageTime, int carsAllowed)
    {
        _trackPoints.Add(new Turn(description, averageTime, carsAllowed));
        return this;
    }

    public Track AddStraight(string description, TimeSpan averageTime)
    {
        _trackPoints.Add(new Straight(description, averageTime));
        return this;
    }

    public Track AddPitLane(TimeSpan entryTime, TimeSpan exitTime, List<Team> teams,
        int nextPoint)
    {
        _pitLane = new PitLane("PitLane", teams);
        _pitLaneEntry = new Turn("PitLane Entry", entryTime, 1);
        _pitLaneExit = new Turn("PitLane Exit", exitTime, 1);
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
        if (_pitLane == null || _pitLaneEntry == null || _pitLaneExit == null) throw new Exception("PitLane is not set");
        var lapWithoutPitLane = _trackPoints.Where(trackPoint => trackPoint != _pitLaneEntry && trackPoint != _pitLane && trackPoint != _pitLaneExit);
        var lapWithPitLaneEntry = _trackPoints.TakeWhile(trackPoint => trackPoint != _pitLane);
        var lapWithPitLaneExit = _trackPoints.SkipWhile(trackPoint => trackPoint != _pitLane).Skip(1);
        if (car.Lap.Number == 0) return _trackPoints;
    }

}