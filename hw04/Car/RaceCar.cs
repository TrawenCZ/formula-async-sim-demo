using hw04.Car.Tires;
using hw04.Race;
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
    public Lap Lap { get; }
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
        Lap = new Lap(this, 1);
        TireStrategy = new List<Tire>();
    }

    public Tire GetCurrentTire() => TireStrategy[_currentTireIndex];

    public void ChangeTires() => _currentTireIndex++;

    public void AddLap()
    {
        Lap.Number++;
        GetCurrentTire().AddLap();
    }
}