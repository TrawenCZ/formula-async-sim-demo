using hw04.Car;

namespace hw04.Race;

public class Lap
{
    public RaceCar RaceCar { get; }
    public int Number { get; }

    public Lap(RaceCar car, int number)
    {
        RaceCar = car;
        Number = number;
    }
    
}