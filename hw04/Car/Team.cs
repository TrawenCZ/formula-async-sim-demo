using hw04.Race;

namespace hw04.Car;

public class Team
{
    public string Name { get; }
    public Task<TrackPointPass>? CurrentExchange { get; set; }
    public Team(string name)
    {
        Name = name;
        CurrentExchange = null;
    }
}