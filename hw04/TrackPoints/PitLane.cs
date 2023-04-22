using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }
    private readonly Dictionary<Team, SemaphoreSlim> _currentPitStopExchanges;    // only reading pointers, so it's thread-safe, and regular Dictionary is faster than ConcurrentDictionary

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
        _currentPitStopExchanges = teams.ToDictionary(team => team, team => new SemaphoreSlim(1, 1));
    }

    public async Task<TrackPointPass> PassAsync(RaceCar car)
    {
        var ticksAtStart = Stopwatch.GetTimestamp();
        await _currentPitStopExchanges[car.Team].WaitAsync();
        TimeSpan waitingTime = Stopwatch.GetElapsedTime(ticksAtStart);

        var random = new Random();
        await Task.Delay(TimeSpan.FromMilliseconds(Enumerable.Repeat(0, 4).Select(x => random.Next(50, 101)).Max()));       // because generating one random number will be lower in long-term statistics
        car.ChangeTires();
        _currentPitStopExchanges[car.Team].Release();
        return new TrackPointPass(this, waitingTime, Stopwatch.GetElapsedTime(waitingTime.Ticks + ticksAtStart));
    }
}