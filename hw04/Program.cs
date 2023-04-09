using hw04;
using hw04.Race;

CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());

await new Race(CurrentF1.Cars.All, CurrentF1.Tracks.Silverstone, 52).StartAsync();

/*
var race = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);
var race2 = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 52);
*/

// foreach (var (car, totalTime) in race.GetOrder())
// {
//     Console.WriteLine($"{car.Driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
// }