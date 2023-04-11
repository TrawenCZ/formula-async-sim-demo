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
        return race.RaceResults.AsParallel()
            .Select(carResult =>
            {
                var totalTime = carResult.Value.Sum(lap => lap.CompletionTime.Ticks);
                return (Car: carResult.Key, TotalTime: TimeSpan.FromTicks(totalTime));
            })
            .OrderBy(result => result.TotalTime)
            .ToList();

    }

    public static List<(string Driver, int FastestLapNumber)> GetFastestLaps(this Race.Race race)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        return race.RaceResults.AsParallel().Select(carResult =>
        {
            int fastestLapNumber = carResult.Value.OrderBy(lap => lap.CompletionTime).First().Number;
            return (carResult.Key.Driver, fastestLapNumber);
        }).ToList();
    }

    public static void PrintOrderTableOfLap(this Race.Race race, int lap)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        int currentPlace = 0;
        race.RaceResults.AsParallel()
            .Select(carResult =>
            {
                TimeSpan timeSum = TimeSpan.Zero;
                for (int i = 0; i < lap; i++)
                {
                    timeSum = carResult.Value.Count > i ? timeSum.Add(carResult.Value[i].CompletionTime) : TimeSpan.MaxValue;
                }
                return (carResult.Key.Driver, TimeSum: timeSum);
            })
            .OrderBy(result => result.TimeSum)
            .ToList().ForEach(result => Console.WriteLine($"{++currentPlace}. {result.Driver}"));
    }

    public static void PrintTrackPointsStatistics(this Race.Race race)
    {
        if (race.RaceResults == null) throw new ArgumentException(_raceHasntFinishedMsg);
        ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)> fastestDrive = new ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)>();
        ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)> longestWait = new ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)>();
        Task.WhenAll(race.RaceResults.Select(carResult => Task.Run(() =>
        {
            Parallel.ForEach(carResult.Value, lap =>
            {
                Parallel.ForEach(lap.TrackPointPasses, p =>
                {
                    fastestDrive.AddOrUpdate(p.TrackPoint, (lap, p.DrivingTime), (key, value) =>
                    {
                        if (value.TimeTaken > p.DrivingTime)
                        {
                            return (lap, p.DrivingTime);
                        }
                        return value;
                    });

                    longestWait.AddOrUpdate(p.TrackPoint, (lap, p.WaitingTime), (key, value) =>
                    {
                        if (value.TimeTaken < p.WaitingTime)
                        {
                            return (lap, p.WaitingTime);
                        }
                        return value;
                    });
                });
            });
        })).ToArray()).GetAwaiter().GetResult();

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

