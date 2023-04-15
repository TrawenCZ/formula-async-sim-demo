using hw04.Car.Tires;
using hw04.Race;
using System.Diagnostics;
using System.Timers;

namespace hw04.Car;

public class RaceCar
{
    
    /// <summary>
    /// Seznam pneumatik v pořadí, v jakém je bude auto při závodu měnit. 
    /// </summary>
    public List<Tire> TireStrategy { get; set; }
    public string Driver { get; set; }
    public Team Team { get; set; }
    public double TurnSpeed { get; set; }
    public double StraightSpeed { get; set; }
    public Stopwatch Stopwatch { get; set; }
    public bool WentToPitStop { get; set; }
    private int _lap = 1;
    public int Lap
    {
        get { return _lap; }
        set {
            if (value == _lap + 1) GetCurrentTire().AddLap();
            _lap = value;
        } }
    private int _currentTireIndex = 0;
    

    /// <param name="driver">Jméno řidiče formule</param>
    /// <param name="team">Tým, pod který formule patří</param>
    /// <param name="turnSpeed">Rychlost auta v zatáčce</param>
    /// <param name="straightSpeed">Rychlost auta na rovince</param>
    public RaceCar(string driver, Team team, double turnSpeed, double straightSpeed)
    {
        Driver = driver;
        Team = team;
        TurnSpeed = turnSpeed;
        StraightSpeed = straightSpeed;
        TireStrategy = new List<Tire>();
        Stopwatch = new Stopwatch();
        WentToPitStop = false;
    }

    public Tire GetCurrentTire() => TireStrategy[_currentTireIndex];
    public bool ShouldChangeTires() => (_currentTireIndex + 1 != TireStrategy.Count) && GetCurrentTire().NeedsChange();

    public void ChangeTires() => _currentTireIndex++;

    public void Reset()
    {
        _lap = 1;
        _currentTireIndex = 0;
        Stopwatch.Reset();
    }
}