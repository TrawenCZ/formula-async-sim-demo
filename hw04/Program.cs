using hw04;
using hw04.Race;

CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

//await new Race(CurrentF1.Cars.All, CurrentF1.Tracks.Silverstone, 8).StartAsync();
//CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

//var race = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);
//CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

var race2 = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 23);
CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

foreach (var (car, totalTime) in race2.GetOrder())
{
     Console.WriteLine($"{car.Driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
}

foreach (var (driver, lapNum) in race2.GetFastestLaps())
{
    Console.WriteLine($"{driver}'s fastest lap is {lapNum}");
}

race2.PrintOrderTableOfLap(22);
race2.PrintTrackPointsStatistics();
Console.WriteLine("Hovno");
race2.PrintTrackPointsStatistics();