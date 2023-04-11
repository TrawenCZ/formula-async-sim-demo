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
            var waitingTime = TimeSpan.Zero;

            if (currentPitStopExchanges.TryGetValue(car.Team, out var currentPitStopExchange))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                await currentPitStopExchange;
                stopwatch.Stop();
                waitingTime += stopwatch.Elapsed;
            }
            var pitStopTime = TimeSpan.FromMilliseconds(_random.Next(50, 101));
            currentPitStopExchanges[car.Team] = Task.Delay(pitStopTime);
            await currentPitStopExchanges[car.Team];
            car.ChangeTires();
            return new TrackPointPass(this, waitingTime, pitStopTime);
        });
    }
}