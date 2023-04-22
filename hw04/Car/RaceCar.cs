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
    private int _currentTireIndex = 0;
    private List<Tire> _tireStrategy;
    public List<Tire> TireStrategy { 
        get {
            return _tireStrategy;
        }
        set {
            _currentTireIndex = 0;
            _tireStrategy = value;
        } 
    }
    public string Driver { get; set; }
    public Team Team { get; set; }
    public double TurnSpeed { get; set; }
    public double StraightSpeed { get; set; }
    public bool WentToPitStop { get; set; }
    


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
        _tireStrategy = new List<Tire>();
        WentToPitStop = false;
    }

    public Tire GetCurrentTire() => _tireStrategy[_currentTireIndex];
    public bool ShouldChangeTires() => (_currentTireIndex + 1 != _tireStrategy.Count) && GetCurrentTire().NeedsChange();

    public void ChangeTires() => _currentTireIndex++;
}