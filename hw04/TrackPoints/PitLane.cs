using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }
    private readonly Random _random = new Random();
    private ConcurrentDictionary<Team, SemaphoreSlim> currentPitStopExchanges = new ConcurrentDictionary<Team, SemaphoreSlim>();

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
        foreach (var team in teams)
        {
            currentPitStopExchanges.TryAdd(team, new SemaphoreSlim(1, 1));
        }
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(async () =>
        {   
            var timeBeforeWait = car.Stopwatch.Elapsed;
            await currentPitStopExchanges[car.Team].WaitAsync();
            var waitingTime = car.Stopwatch.Elapsed - timeBeforeWait;

            await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(50, 101)));
            car.ChangeTires();
            currentPitStopExchanges[car.Team].Release();
            return new TrackPointPass(this, waitingTime, car.Stopwatch.Elapsed - waitingTime);
        });
    }
}