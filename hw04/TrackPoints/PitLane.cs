using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }
    private Dictionary<Team, SemaphoreSlim> currentPitStopExchanges;    // only reading pointers, so it's thread-safe, and regular Dictionary is faster than ConcurrentDictionary

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
        currentPitStopExchanges = teams.ToDictionary(team => team, team => new SemaphoreSlim(1, 1));
    }

    public async Task<TrackPointPass> PassAsync(RaceCar car)
    {
        var timeBeforeWait = car.Stopwatch.Elapsed;
        await currentPitStopExchanges[car.Team].WaitAsync();
        var waitingTime = car.Stopwatch.Elapsed - timeBeforeWait;

        var random = new Random();
        await Task.Delay(TimeSpan.FromMilliseconds(Enumerable.Repeat(0, 4).Select(x => random.Next(50, 101)).Max()));       // because generating one random number will be lower in long-term statistics
        car.ChangeTires();
        currentPitStopExchanges[car.Team].Release();
        return new TrackPointPass(this, waitingTime, car.Stopwatch.Elapsed - (waitingTime + timeBeforeWait));
    }
}