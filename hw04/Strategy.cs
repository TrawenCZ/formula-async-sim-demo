using hw04.Car;
using hw04.Car.Tires;

namespace hw04;

public static class Strategy
{
    public static void SetMediumHardStrategy(this RaceCar car)
    {
        car.TireStrategy = new()
        {
            new Tire(CurrentF1.TireTypes.Medium, 20),
            new Tire(CurrentF1.TireTypes.Hard, 40)
        };
    }
    
    public static void SetMediumMediumSoftStrategy(this RaceCar car)
    {
        car.TireStrategy = new()
        {
            new Tire(CurrentF1.TireTypes.Medium, 20),
            new Tire(CurrentF1.TireTypes.Medium, 20),
            new Tire(CurrentF1.TireTypes.Soft, 15)
        };
    }
    
    public static void SetMediumHardSoftStrategy(this RaceCar car)
    {
        car.TireStrategy = new()
        {
            new Tire(CurrentF1.TireTypes.Medium, 30),
            new Tire(CurrentF1.TireTypes.Hard, 10),
            new Tire(CurrentF1.TireTypes.Soft, 15)
        };
    }
}