namespace hw04.Car.Tires;

public class TireType
{
    public string Name { get; set; }
    public double Speed { get; set; }
    public double SlowdownPerLap { get; set; }
    public int OptimalLaps { get; set; }
    public int MaxLaps { get; set; }

    public TireType(string name, double speed, double slowdownPerLap, int optimalLaps, int maxLaps)
    {
        Name = name;
        Speed = speed;
        SlowdownPerLap = slowdownPerLap;
        OptimalLaps = optimalLaps;
        MaxLaps = maxLaps;
    }
}