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
        ConcurrentDictionary<int, TimeSpan> lapBestCompletionTimes = new ConcurrentDictionary<int, TimeSpan>();
        ConcurrentDictionary<RaceCar, List<Lap>> carsLaps = new ConcurrentDictionary<RaceCar, List<Lap>>();
        var tasks = new List<Task<List<Lap>>>();
        SemaphoreSlim printAsyncMutex = new SemaphoreSlim(1, 1);    // Mutex itself cannot be async, so this is a workaround
        ManualResetEventSlim startSignal = new ManualResetEventSlim(false);
        //CountdownEvent allCarsReady = new CountdownEvent(_cars.Count());

        Stopwatch raceStopwatch = new Stopwatch();
        foreach (RaceCar car in _cars)
        {
            car.Stopwatch = raceStopwatch;
        }

        foreach (var car in _cars)
        {
            tasks.Add(Task.Run(async () =>
            {
                List<TrackPointPass> trackPointsPasses;
                TimeSpan bestTimeElapsedSinceStart;
                TimeSpan timeElapsedSinceStart;
                TimeSpan totalDriveTime = TimeSpan.Zero;
                List<Lap> laps = new List<Lap>();
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
                    timeElapsedSinceStart = car.Stopwatch.Elapsed;
                    bestTimeElapsedSinceStart = lapBestCompletionTimes.GetOrAdd(lap, timeElapsedSinceStart);
                    if (bestTimeElapsedSinceStart.Equals(timeElapsedSinceStart))
                    {
                        Console.WriteLine($"\nLap: {lap}\n{car.Driver}: {(timeElapsedSinceStart).ToString(@"mm\:ss\.ff")}");
                    }
                    else if (!lapBestCompletionTimes.ContainsKey(lap + 1))
                    {
                        Console.WriteLine($"{car.Driver}: +{(timeElapsedSinceStart - bestTimeElapsedSinceStart).ToString(@"mm\:ss\.ff")}");
                    }
                    printAsyncMutex.Release();
                    laps.Add(new Lap(car, lap, trackPointsPasses, timeElapsedSinceStart - totalDriveTime));
                    totalDriveTime = timeElapsedSinceStart;
                    if (lap == _numberOfLaps) shouldEnd = true;
                    car.GetCurrentTire().AddLap();
                    lap++;
                }
                return laps;
            }));
        }
        //allCarsReady.Wait();
        raceStopwatch.Start();
        startSignal.Set();
        await Task.WhenAll(tasks);
        raceStopwatch.Stop();
        for (int i = 0; i < _cars.Count(); i++)
        {
            List<Lap> taskResult = await tasks[0];
            RaceCar car = _cars.ElementAt(i);
            car.Reset();
            RaceResults.Add(car, await tasks[i]);
        }

        //foreach (RaceCar car in _cars) car.Reset();
    }
}