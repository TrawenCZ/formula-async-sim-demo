using hw04.Car;

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
}