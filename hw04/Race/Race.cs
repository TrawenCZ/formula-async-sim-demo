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
    public ConcurrentDictionary<RaceCar, List<Lap>>? RaceResults { get; private set; }
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps, bool verbose)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
        _verbose = verbose;
        RaceResults = null;
    }
    
    public Task StartAsync()
    {
        return Task.Run(() =>
        {
            DateTime raceStartDateTime = DateTime.Now;
            bool shouldEnd = false;
            ConcurrentDictionary<int, TimeSpan> lapBestCompletionTimes = new ConcurrentDictionary<int, TimeSpan>();
            ConcurrentDictionary<RaceCar, List<Lap>> carsLaps = new ConcurrentDictionary<RaceCar, List<Lap>>();
            var tasks = new List<Task>();
            foreach (var car in _cars)
            {
                tasks.Add(Task.Run(() =>
                {
                    carsLaps.TryAdd(car, new List<Lap>());
                    while (!shouldEnd)
                    {
                        List<TrackPointPass> trackPointsPasses = new List<TrackPointPass>();
                        DateTime dateTimeWhenLapStarted = DateTime.Now;
                        foreach (var trackPoint in _track.GetLap(car))
                        {
                            trackPointsPasses.Add(trackPoint.PassAsync(car).Result);
                        }
                        TimeSpan lapCompletionTime = DateTime.Now - dateTimeWhenLapStarted;
                        carsLaps[car].Add(new Lap(car, car.Lap, trackPointsPasses, lapCompletionTime));

                        if (_verbose)
                        {
                            TimeSpan lapBestCompletionTime = TimeSpan.FromMilliseconds(0);
                            if (lapBestCompletionTimes.TryGetValue(car.Lap, out lapBestCompletionTime))
                            {
                                Console.WriteLine($"{car.Driver}: +{(lapCompletionTime - lapBestCompletionTime).ToString(@"mm\:ss\.ff")}");
                            }
                            else if (lapBestCompletionTimes.TryAdd(car.Lap, lapCompletionTime))
                            {
                                Console.WriteLine($"\nLap: {car.Lap}\n{car.Driver}: {(DateTime.Now - raceStartDateTime).ToString(@"mm\:ss\.ff")}");
                            }
                            else 
                            {
                                Console.WriteLine($"{car.Driver}: {(lapCompletionTime - lapBestCompletionTimes[car.Lap]).ToString(@"mm\:ss\.ff")}");
                            }
                        }
                        if (car.Lap == _numberOfLaps) shouldEnd = true;
                        car.Lap++;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (RaceCar car in _cars) car.Reset();
            RaceResults = carsLaps;
        });
    }
}