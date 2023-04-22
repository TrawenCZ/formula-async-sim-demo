using hw04.Car;
using hw04.Race;
using hw04.TrackPoints;
using System.Collections.Concurrent;

namespace hw04;

public static class RaceAnalytics
{
    public static List<(RaceCar Car, TimeSpan TotalTime)> GetOrder(this Race.Race race)
    {
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
        return race.RaceResults.AsParallel().Select(carResult =>
        {
            int fastestLapNumber = carResult.Value.OrderBy(lap => lap.CompletionTime).First().Number;
            return (carResult.Key.Driver, fastestLapNumber);
        }).ToList();
    }

    public static List<string> GetDriverOrderOfLap(this Race.Race race, int lap)
    {
        return race.RaceResults.AsParallel()
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
            .Select(result => result.Driver)
            .ToList();
    }

    public static (Dictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)> FastestDrives, Dictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)> LongestWaits) GetTrackpointsStatistics(this Race.Race race)
    {
        ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)> fastestDrives = new ConcurrentDictionary<ITrackPoint, (Lap FastestDriveTroughLap, TimeSpan TimeTaken)>();
        ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)> longestWaits = new ConcurrentDictionary<ITrackPoint, (Lap LongestWaitingLap, TimeSpan TimeTaken)>();
        race.RaceResults.AsParallel().ForAll(carResult =>
        {
            carResult.Value.ForEach(lap =>
            {
                lap.TrackPointPasses.ForEach(p =>
                {
                    fastestDrives.AddOrUpdate(p.TrackPoint, (lap, p.DrivingTime), (key, value) =>
                    {
                        if (value.TimeTaken > p.DrivingTime)
                        {
                            return (lap, p.DrivingTime);
                        }
                        return value;
                    });

                    longestWaits.AddOrUpdate(p.TrackPoint, (lap, p.WaitingTime), (key, value) =>
                    {
                        if (value.TimeTaken < p.WaitingTime)
                        {
                            return (lap, p.WaitingTime);
                        }
                        return value;
                    });
                });
            });
        });

        return (fastestDrives.OrderBy(x => int.TryParse(x.Key.Description.Split()[0], out var res) ? res : int.MaxValue).ToDictionary(x => x.Key, x => x.Value), 
            longestWaits.OrderBy(x => int.TryParse(x.Key.Description.Split()[0], out var res) ? res : int.MaxValue).ToDictionary(x => x.Key, x => x.Value));
    }
}