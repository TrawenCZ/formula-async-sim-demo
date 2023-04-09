using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }
    private readonly Random _random = new Random();

    public PitLane(string description, List<Team> teams)
    {
        Description = description;
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(() =>
        {
            var waitingTime = TimeSpan.FromMilliseconds(0);
            if (car.Team.CurrentExchange != null)
            {
                var timeBeforeWait = DateTime.Now;
                car.Team.CurrentExchange.Wait();
                waitingTime += DateTime.Now - timeBeforeWait;
            }
            var pitStopTime = TimeSpan.FromMilliseconds(_random.Next(50, 101));
            Thread.Sleep(pitStopTime);
            car.ChangeTires();
            return new TrackPointPass(this, waitingTime, pitStopTime);
        });
    }
}