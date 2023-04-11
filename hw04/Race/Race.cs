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
            foreach (var car in _cars)
            {
                tasks.Add(Task.Run(async () =>
                {
                    carsLaps.TryAdd(car, new List<Lap>());
                    TimeSpan timeElapsedSinceStart = TimeSpan.Zero;
                    while (!shouldEnd)
                    {
                        List<TrackPointPass> trackPointsPasses = new List<TrackPointPass>();
                        TimeSpan lapCompletionTime = TimeSpan.Zero;
                        foreach (var trackPoint in _track.GetLap(car))
                        {
                            var trackPointResult = await trackPoint.PassAsync(car);
                            lapCompletionTime += trackPointResult.WaitingTime + trackPointResult.DrivingTime;
                            trackPointsPasses.Add(trackPointResult);
                        }
                        carsLaps[car].Add(new Lap(car, car.Lap, trackPointsPasses, lapCompletionTime));

                        timeElapsedSinceStart += lapCompletionTime;
                        TimeSpan bestTimeElapsedSinceStart = lapBestCompletionTimes.GetOrAdd(car.Lap, timeElapsedSinceStart);
                        if (lapBestCompletionTimes.ContainsKey(car.Lap + 1))
                        {
                            car.Lap++;
                            continue;
                        }
                        if (bestTimeElapsedSinceStart.Equals(timeElapsedSinceStart))
                        {
                            Console.WriteLine($"\nLap: {car.Lap}\n{car.Driver}: {timeElapsedSinceStart.ToString(@"mm\:ss\.ff")}");
                        }
                        else
                        {
                            Console.WriteLine($"{car.Driver}: +{(timeElapsedSinceStart - bestTimeElapsedSinceStart).ToString(@"mm\:ss\.ff")}");
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