using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }
    private readonly Random _random = new Random();
    private ConcurrentDictionary<Team, Task> currentPitStopExchanges = new ConcurrentDictionary<Team, Task>();

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(async () =>
        {   
            var waitingTime = TimeSpan.Zero;
            Task? currentPitStopExchange = null;
            if (currentPitStopExchanges.TryGetValue(car.Team, out currentPitStopExchange))
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