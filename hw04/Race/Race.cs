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
            foreach (RaceCar car in _cars)
            {
                carsLaps.TryAdd(car, new List<Lap>());
            }
            foreach (RaceCar car in _cars)
            {
                car.Stopwatch.Start();
            }
            foreach (var car in _cars)
            {
                tasks.Add(Task.Run(async () =>
                {
                    List<TrackPointPass> trackPointsPasses;
                    TimeSpan bestTimeElapsedSinceStart;
                    TimeSpan timeElapsedSinceStart;
                    TimeSpan timeBeforeLap;
                    while (!shouldEnd)
                    {
                        timeBeforeLap = car.Stopwatch.Elapsed;
                        trackPointsPasses = new List<TrackPointPass>();
                        foreach (var trackPoint in _track.GetLap(car))
                        {
                            trackPointsPasses.Add(await trackPoint.PassAsync(car));
                        }
                        carsLaps[car].Add(new Lap(car, car.Lap, trackPointsPasses, car.Stopwatch.Elapsed - timeBeforeLap));

                        timeElapsedSinceStart = car.Stopwatch.Elapsed;
                        bestTimeElapsedSinceStart = lapBestCompletionTimes.GetOrAdd(car.Lap, timeElapsedSinceStart);
                        if (lapBestCompletionTimes.ContainsKey(car.Lap + 1))
                        {
                            car.Lap++;
                            continue;
                        }
                        if (bestTimeElapsedSinceStart.Equals(timeElapsedSinceStart))
                        {
                            Console.WriteLine($"\nLap: {car.Lap}\n{car.Driver}: {(car.Stopwatch.Elapsed - timeBeforeLap).ToString(@"mm\:ss\.ff")}");
                        }
                        else
                        {
                            Console.WriteLine($"{car.Driver}: +{(car.Stopwatch.Elapsed - bestTimeElapsedSinceStart).ToString(@"mm\:ss\.ff")}");
                        }
                        if (car.Lap == _numberOfLaps) shouldEnd = true;
                        car.Lap++;
                    }
                }));
            }
            await Task.WhenAll(tasks.ToArray());
            foreach (RaceCar car in _cars) car.Reset();
            RaceResults = carsLaps;
        });
    }
}