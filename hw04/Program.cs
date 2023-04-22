using hw04;
using hw04.Race;
using hw04.TrackPoints;
using System.Text;

CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

//await new Race(CurrentF1.Cars.All, CurrentF1.Tracks.Silverstone, 8).StartAsync();
//CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

//var race = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);
//CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

var race2 = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 23);
CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

Console.WriteLine("\nFinish times of racers:");
foreach (var (car, totalTime) in race2.GetOrder())
{
     Console.WriteLine($"{car.Driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
}

Console.WriteLine("\nFastest laps for racers:");
foreach (var (driver, lapNum) in race2.GetFastestLaps())
{
    Console.WriteLine($"{driver}'s fastest lap is {lapNum}");
}

int position = 0;
Console.WriteLine($"\nOrder of finishes in lap 2:");
foreach (var driver in race2.GetDriverOrderOfLap(2))
{
    Console.WriteLine($"{++position}. {driver}");
}



Console.WriteLine("\nTrackpoints statistics:");
var (fastestDrives, longestWaits) = race2.GetTrackpointsStatistics();
foreach (ITrackPoint trackPoint in fastestDrives.Keys)
{
    Console.WriteLine($"{trackPoint.Description}\n\tFastest Drive Trough: {ExtractData(fastestDrives[trackPoint])}\n\tLongest Wait: {ExtractData(longestWaits[trackPoint])}");
}
static string ExtractData((Lap Lap, TimeSpan TimeTaken) data)
{
    return new StringBuilder()
        .Append("\n\t\tDriver: " + data.Lap.RaceCar.Driver)
        .Append("\n\t\tTime taken: " + data.TimeTaken.ToString(@"mm\:ss\.ff"))
        .Append("\n\t\tLap: " + data.Lap.Number)
        .ToString();
}