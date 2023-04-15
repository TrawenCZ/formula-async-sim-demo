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
    public ConcurrentDictionary<RaceCar, List<Lap>>? RaceResults { get; private set; }
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
        RaceResults = null;
    }
    
    public Task StartAsync()
    {
        return Task.Run(async () =>
        {
            bool shouldEnd = false;
            ConcurrentDictionary<int, TimeSpan> lapBestCompletionTimes = new ConcurrentDictionary<int, TimeSpan>();
            ConcurrentDictionary<RaceCar, List<Lap>> carsLaps = new ConcurrentDictionary<RaceCar, List<Lap>>();
            var tasks = new List<Task>();
            SemaphoreSlim printAsyncMutex = new SemaphoreSlim(1, 1);    // Mutex itself cannot be async, so this is a workaround
            ManualResetEventSlim startSignal = new ManualResetEventSlim(false);
            //CountdownEvent allCarsReady = new CountdownEvent(_cars.Count());
            //var lapsHeaderPrinted = Enumerable.Repeat(false, _numberOfLaps + 1).ToList();   // + 1 so I can use lap number as index
            foreach (RaceCar car in _cars)
            {
                carsLaps.TryAdd(car, new List<Lap>());
            }

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
                        bestTimeElapsedSinceStart = lapBestCompletionTimes.GetOrAdd(car.Lap, timeElapsedSinceStart);
                        if (bestTimeElapsedSinceStart.Equals(timeElapsedSinceStart))
                        {
                            Console.WriteLine($"\nLap: {car.Lap}\n{car.Driver}: {(timeElapsedSinceStart - totalDriveTime).ToString(@"mm\:ss\.ff")}");
                        }
                        else if (!lapBestCompletionTimes.ContainsKey(car.Lap + 1))
                        {
                            Console.WriteLine($"{car.Driver}: +{(timeElapsedSinceStart - bestTimeElapsedSinceStart).ToString(@"mm\:ss\.ff")}");
                        }
                        printAsyncMutex.Release();
                        carsLaps[car].Add(new Lap(car, car.Lap, trackPointsPasses, timeElapsedSinceStart - totalDriveTime));
                        totalDriveTime = timeElapsedSinceStart;
                        if (car.Lap == _numberOfLaps) shouldEnd = true;
                        car.Lap++;
                    }
                }));
            }
            //allCarsReady.Wait();
            raceStopwatch.Start();
            startSignal.Set();
            await Task.WhenAll(tasks.ToArray());
            foreach (RaceCar car in _cars) car.Reset();
            RaceResults = carsLaps;
        });
    }
}