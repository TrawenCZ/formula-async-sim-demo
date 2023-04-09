using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private int _numberOfLaps;
    private readonly IEnumerable<RaceCar> _cars;
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
    }
    
    public Task StartAsync()
    {
        return Task.Run(() =>
        {
            DateTime startTime = DateTime.Now;
            bool shouldEnd = false;
            ConcurrentDictionary<int, TimeSpan> lapCompletionTimes = new ConcurrentDictionary<int, TimeSpan>();
            var tasks = new List<Task>();
            foreach (var car in _cars)
            {
                tasks.Add(Task.Run(() =>
                {
                    while (!shouldEnd)
                    {
                        foreach (var trackPoint in _track.GetLap(car))
                        {
                            var bla = trackPoint.PassAsync(car).Result;
                        }
                        TimeSpan lapCompletionTime = TimeSpan.FromMilliseconds(0);
                        if (lapCompletionTimes.TryGetValue(car.Lap.Number, out lapCompletionTime))
                        {
                            Console.WriteLine($"{car.Driver}: +{(DateTime.Now - lapCompletionTime - startTime).ToString(@"m\:ss\.ff")}");
                        }
                        else if (lapCompletionTimes.TryAdd(car.Lap.Number, DateTime.Now - startTime))
                        {
                            Console.WriteLine($"\nLap: {car.Lap.Number}\n{car.Driver}: {lapCompletionTimes[car.Lap.Number].ToString(@"m\:ss\.ff")}");
                        }
                        if (car.Lap.Number == _numberOfLaps) shouldEnd = true;
                        car.AddLap();
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
        });
    }
}