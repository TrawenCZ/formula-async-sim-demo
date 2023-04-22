using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private int _numberOfLaps;
    private readonly IEnumerable<RaceCar> _cars;
    public Dictionary<RaceCar, List<Lap>> RaceResults { get; private set; }
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
        RaceResults = new Dictionary<RaceCar, List<Lap>>();
    }
    
    public async Task StartAsync()
    {
        bool shouldEnd = false;
        ConcurrentDictionary<int, (TimeSpan BestTimeElapsedSinceStart, RaceCar Identifier)> lapBestCompletionTimes = new ConcurrentDictionary<int, (TimeSpan, RaceCar)>();
        ConcurrentDictionary<RaceCar, List<Lap>> carsLaps = new ConcurrentDictionary<RaceCar, List<Lap>>();
        var tasks = new List<Task<List<Lap>>>();
        SemaphoreSlim printAsyncMutex = new SemaphoreSlim(1, 1);    // Mutex itself cannot be async, so this is a workaround
        ManualResetEventSlim startSignal = new ManualResetEventSlim(false);
        var ticksAtRaceStart = 0L;
        //CountdownEvent allCarsReady = new CountdownEvent(_cars.Count());

        foreach (var car in _cars)
        {
            tasks.Add(Task.Run(async () =>
            {
                List<TrackPointPass> trackPointsPasses;
                TimeSpan bestTimeElapsedSinceStart;
                TimeSpan timeElapsedSinceStart;
                TimeSpan totalDriveTime = TimeSpan.Zero;
                List<Lap> laps = new List<Lap>();
                string _timeFormattingString = @"mm\:ss\.ff";
                RaceCar identifier;
                int lap = 1;
                //allCarsReady.Signal();
                startSignal.Wait();
                while (!shouldEnd)
                {
                    trackPointsPasses = new List<TrackPointPass>();
                    foreach (var trackPoint in _track.GetLap(car))
                    {
                        trackPointsPasses.Add(await trackPoint.PassAsync(car));
                    }

                    await printAsyncMutex.WaitAsync();
                    timeElapsedSinceStart = Stopwatch.GetElapsedTime(ticksAtRaceStart);
                    (bestTimeElapsedSinceStart, identifier) = lapBestCompletionTimes.GetOrAdd(lap, (timeElapsedSinceStart, car));
                    if (car == identifier)
                    {
                        Console.WriteLine($"\nLap: {lap}\n{car.Driver}: {(timeElapsedSinceStart).ToString(_timeFormattingString)}");
                    }
                    else if (!lapBestCompletionTimes.ContainsKey(lap + 1))
                    {
                        Console.WriteLine($"{car.Driver}: +{(timeElapsedSinceStart - bestTimeElapsedSinceStart).ToString(_timeFormattingString)}");
                    }
                    if (lap == _numberOfLaps) shouldEnd = true;
                    printAsyncMutex.Release();
                    //if (lap == 22) Console.WriteLine(car.Driver + " " + (timeElapsedSinceStart - totalDriveTime).ToString(_timeFormattingString));
                    laps.Add(new Lap(car, lap, trackPointsPasses, timeElapsedSinceStart - totalDriveTime));
                    totalDriveTime = timeElapsedSinceStart;                   
                    car.GetCurrentTire().AddLap();
                    lap++;
                }
                return laps;
            }));
        }
        //allCarsReady.Wait();
        ticksAtRaceStart = Stopwatch.GetTimestamp();
        startSignal.Set();
        await Task.WhenAll(tasks);
        for (int i = 0; i < _cars.Count(); i++)
        {
            RaceResults.Add(_cars.ElementAt(i), await tasks[i]);
        }
    }
}