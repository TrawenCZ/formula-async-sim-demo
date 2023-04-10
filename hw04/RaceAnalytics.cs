using hw04.Car;
using hw04.Race;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Text;

namespace hw04;

public static class RaceAnalytics
{
    private static readonly string _raceHasntFinishedMsg = "Race has not been finished";
    public static List<(RaceCar Car, TimeSpan TotalTime)> GetOrder(this Race.Race race)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        return race.RaceResults.Select(carResult => Task.Run(() =>
        {
            var totalTime = TimeSpan.FromMilliseconds(carResult.Value.Sum(lap => lap.CompletionTime.Milliseconds));
            return (carResult.Key, totalTime);
        }))
            .Select(task => task.Result)
            .ToList();
    }

    public static List<(string Driver, int FastestLapNumber)> GetFastestLaps(this Race.Race race)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        return race.RaceResults.Select(carResult => Task.Run(() => 
        {
            int fastestLapNumber = carResult.Value.OrderBy(lap => lap.CompletionTime).First().Number;
            return (carResult.Key.Driver, fastestLapNumber);
        }))
            .Select(task => task.Result)
            .ToList();
    }

    public static void PrintOrderTableOfLap(this Race.Race race, int lap)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        int currentPlace = 1;
        race.RaceResults.Select(carResult => Task.Run(() =>
        {
            TimeSpan timeSum = TimeSpan.Zero;
            for (int i = 0; i < lap; i++)
            {
                timeSum = carResult.Value.Count > i ? timeSum.Add(carResult.Value[i].CompletionTime) : TimeSpan.MaxValue;
            }
            return (carResult.Key.Driver, timeSum);
        }))
            .Select(task => task.Result)
            .OrderBy(result => result.timeSum)
            .ToList()
            .ForEach(result => { Console.WriteLine($"{currentPlace}. {result.Driver}"); currentPlace++; });
    }

    public static void PrintTrackPointsStatistics(this Race.Race race)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)> fastestDrive = new ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)>();
        ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)> longestWait = new ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)>();
        race.RaceResults.Select(carResult => Task.Run(() =>
        {
            var tasks = new List<Task>();
            carResult.Value.ForEach(lap =>
            {
                tasks.Add(Task.Run(() =>
                {
                    lap.TrackPointPasses.ForEach(p =>
                    {
                        var value = fastestDrive.GetOrAdd(p.TrackPoint, (lap, p.DrivingTime));
                        while (value != (lap, p.DrivingTime) && value.TimeTaken > p.DrivingTime)
                        {
                            fastestDrive.TryUpdate(p.TrackPoint, (lap, p.DrivingTime), value);
                            fastestDrive.TryGetValue(p.TrackPoint, out value);
                        }

                        value = longestWait.GetOrAdd(p.TrackPoint, (lap, p.WaitingTime));
                        while (value != (lap, p.WaitingTime) && value.TimeTaken < p.WaitingTime)
                        {
                            longestWait.TryUpdate(p.TrackPoint, (lap, p.WaitingTime), value);
                            longestWait.TryGetValue(p.TrackPoint, out value);
                        }
                    });
                }));
            });
            Task.WaitAll(tasks.ToArray());
        }))
            .Select(async task => await task);
        foreach (ITrackPoint trackPoint in fastestDrive.Keys)
        {
            string fastestDriveDataString = ExtractData(fastestDrive[trackPoint]);
            string longestWaitDataString = ExtractData(longestWait[trackPoint]);
            Console.WriteLine($"{trackPoint.Description}\n\tFastest Drive Trough: {fastestDriveDataString}\n\tLongest Wait: {longestWaitDataString}");
        }
    }
    
    private static string ExtractData((Lap Lap, TimeSpan TimeTaken) data)
    {
        return new StringBuilder()
            .Append("\n\t\tDriver: " + data.Lap.RaceCar.Driver)
            .Append("\n\t\tTime taken: " + data.TimeTaken.ToString(@"mm\:ss\.ff"))
            .Append("\n\t\tLap: " + data.Lap.Number)
            .ToString();
    }
}

