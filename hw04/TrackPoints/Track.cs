using hw04.Car;
using System.Collections.Immutable;

namespace hw04.TrackPoints;

public class Track
{
    private readonly List<ITrackPoint> _trackPoints;
    private Turn? _pitLaneEntry;
    private Turn? _pitLaneExit;
    private PitLane? _pitLane;
    private ImmutableList<ITrackPoint> _lapWithPitLaneEntry;
    private ImmutableList<ITrackPoint> _lapWithPitLaneExit;

    public Track()
    {
        _trackPoints = new List<ITrackPoint>();
        _lapWithPitLaneEntry = ImmutableList.Create<ITrackPoint>();
        _lapWithPitLaneExit = ImmutableList.Create<ITrackPoint>();
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

        _lapWithPitLaneEntry = new List<ITrackPoint>(_trackPoints) { _pitLaneEntry }.ToImmutableList();

        _lapWithPitLaneExit = new List<ITrackPoint>() { _pitLane, _pitLaneExit }.Concat(_trackPoints.Skip(nextPoint)).ToImmutableList();

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