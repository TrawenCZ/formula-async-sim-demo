namespace hw04.Car.Tires;

/// <summary>
/// Třída reprezentující 4 pneumatiky formule
/// </summary>
public class Tire
{
    private readonly TireType _tireType;
    private readonly int _changeAfter;
    private int _age;

    public Tire(TireType tireType, int changeAfter)
    {
        _tireType = tireType;
        _changeAfter = changeAfter;
        _age = 0;
    }
    public void AddLap()
    {
        _age++;
    }

    public bool NeedsChange()
    {
        return _age >= _changeAfter;
    }
    
    public double GetSpeed()
    {
        return _tireType.Speed + (int.Max(_age - _tireType.OptimalLaps, 0) * _tireType.SlowdownPerLap)
                              + (int.Max(_age - _tireType.MaxLaps, 0) * _tireType.SlowdownPerLap);
    }
}