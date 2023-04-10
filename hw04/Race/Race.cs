using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private int _numberOfLaps;
    private readonly IEnumerable<RaceCar> _cars;
    private readonly bool _verbose;
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps, bool verbose)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = new List<RaceCar>(cars);
        _verbose = verbose;
    }
    
    public Task StartAsync()
    {
        return Task.Run(() =>
        {
            DateTime startTime = DateTime.Now;
            bool shouldEnd = false;
            ConcurrentDictionary<int, TimeSpan> lapCompletionTimes = new ConcurrentDictionary<int, TimeSpan>();
            ConcurrentDictionary<RaceCar, List<TrackPointPass>> carsTrackPointPasses = new ConcurrentDictionary<RaceCar, List<TrackPointPass>>();
            var tasks = new List<Task>();
            foreach (var car in _cars)
            {
                tasks.Add(Task.Run(() =>
                {
                    while (!shouldEnd)
                    {
                        foreach (var trackPoint in _track.GetLap(car))
                        {
                            carsTrackPointPasses[car].Add(trackPoint.PassAsync(car).Result);
                        }
                        TimeSpan lapCompletionTime = TimeSpan.FromMilliseconds(0);
                        if (lapCompletionTimes.TryGetValue(car.Lap, out lapCompletionTime) && _verbose)
                        {
                            Console.WriteLine($"{car.Driver}: +{(DateTime.Now - lapCompletionTime - startTime).ToString(@"mm\:ss\.ff")}");
                        }
                        else if (lapCompletionTimes.TryAdd(car.Lap, DateTime.Now - startTime) && _verbose)
                        {
                            if (_verbose) Console.WriteLine($"\nLap: {car.Lap}\n{car.Driver}: {lapCompletionTimes[car.Lap].ToString(@"mm\:ss\.ff")}");
                        } else if (_verbose)
                        {
                            Console.WriteLine($"{car.Driver}: {(DateTime.Now - lapCompletionTimes[car.Lap] - startTime).ToString(@"mm\:ss\.ff")}");
                        }
                        if (car.Lap == _numberOfLaps) shouldEnd = true;
                        car.Lap++;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return carsTrackPointPasses;
        });
    }
}