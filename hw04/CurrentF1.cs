using hw04.Car;
using hw04.Car.Tires;
using hw04.TrackPoints;

namespace hw04;

public static class CurrentF1
{
    private static class Teams
    {
        public static Team Redbull { get; } = new("Red Bull");
        public static Team Ferarri { get; } = new("Ferarri");
        public static Team Mercedes { get; } = new("Mercedes");
        public static Team Alpine { get; } = new("Alpine");
        public static Team McLaren { get; } = new("McLaren");
        public static Team AlfaRomeo { get; } = new("Alfa Romeo");
        public static Team AstonMartin { get; } = new("Aston Martin");
        public static Team Haas { get; } = new("Haas");
        public static Team AlphaTauri { get; } = new("AlphaTauri");
        public static Team Williams { get; } = new("Williams");
        private static readonly List<Team> _all = new ()
        {
            Redbull, Ferarri, Mercedes, Alpine, McLaren, AlfaRomeo, AstonMartin, Haas, AlphaTauri, Williams
        };
        public static List<Team> All => _all;
    }

    public static class Cars
    {
        private static RaceCar Verstappen { get; } = new("Verstappen", Teams.Redbull, 1.0, 1.0);
        private static RaceCar Perez { get; } = new("Perez", Teams.Redbull, 1.0, 1.0);
        private static RaceCar LeClerc { get; } = new("LeClerc", Teams.Ferarri, 1.0, 1.005);
        private static RaceCar Sainz { get; } = new("Sainz", Teams.Ferarri, 1.0, 1.005);
        private static RaceCar Hamilton { get; } = new("Hamilton", Teams.Mercedes, 1.0, 1.005);
        private static RaceCar Russell { get; } = new("Russell", Teams.Mercedes, 1.0, 1.005);
        private static RaceCar Ocon { get; } = new("Ocon", Teams.Alpine, 1.01, 1.005);
        private static RaceCar Gasly { get; } = new("Gasly", Teams.Alpine, 1.01, 1.005);
        private static RaceCar Piastri { get; } = new("Piastri", Teams.McLaren, 1.005, 1.01);
        private static RaceCar Norris { get; } = new("Norris", Teams.McLaren, 1.005, 1.01);
        private static RaceCar Bottas { get; } = new("Bottas", Teams.AlfaRomeo, 1.02, 1.005);
        private static RaceCar Zhou { get; } = new("Zhou", Teams.AlfaRomeo, 1.02, 1.005);
        private static RaceCar Stroll { get; } = new("Stroll", Teams.AstonMartin, 1.02, 1.005);
        private static RaceCar Alonzo { get; } = new("Alonzo", Teams.AstonMartin, 1.02, 1.005);
        private static RaceCar Magnussen { get; } = new("Magnussen", Teams.Haas, 1.02, 1.005);
        private static RaceCar Hulkenberg { get; } = new("Hulkenberg", Teams.Haas, 1.02, 1.005);
        private static RaceCar DeVries { get; } = new("De Vries", Teams.AlphaTauri, 1.02, 1.005);
        private static RaceCar Tsunoda { get; } = new("Tsunoda", Teams.AlphaTauri, 1.02, 1.005);
        private static RaceCar Albon { get; } = new("Albon", Teams.Williams, 1.02, 1.005);
        private static RaceCar Sargeant { get; } = new("Sargeant", Teams.Williams, 1.02, 1.005);
        private static readonly List<RaceCar> _all = new ()
        {
            Verstappen, Perez, LeClerc, Sainz, Hamilton, Russell, Ocon, Gasly, Piastri, Norris,
            Bottas, Zhou, Stroll, Alonzo, Magnussen, Hulkenberg, DeVries, Tsunoda, Albon, Sargeant
        };
        public static List<RaceCar> All => _all;
    }

    public static class TireTypes
    {
        public static TireType Soft { get; } = new("Soft", 1.0, 0.0014, 5, 15);
        public static TireType Medium { get; } = new("Medium", 1.01, 0.0011, 5, 25);
        public static TireType Hard { get; } = new("Hard", 1.02, 0.0007, 40, 40);
    }

    public static class Tracks
    {
        public static Track Silverstone { get; } = new Track()
            .AddStraight("1 / Hamilton Straight after grid", TimeSpan.FromMilliseconds(49))
            .AddTurn("2 / Turn 1 - Abbey", TimeSpan.FromMilliseconds(20), 3)
            .AddTurn("3 / Turn 2 - Farm Curve", TimeSpan.FromMilliseconds(55), 2)
            .AddTurn("4 / Turn 3 - Village", TimeSpan.FromMilliseconds(63), 1)
            .AddTurn("5 / Turn 4 - The Loop", TimeSpan.FromMilliseconds(69), 1)
            .AddTurn("6 / Turn 5 - Aintree", TimeSpan.FromMilliseconds(37), 2)
            .AddStraight("7 / Wellington Straight", TimeSpan.FromMilliseconds(106))
            .AddTurn("8 / Turn 6 - Brooklands", TimeSpan.FromMilliseconds(60), 2)
            .AddTurn("9 / Turn 7 - Luffield", TimeSpan.FromMilliseconds(89), 1)
            .AddTurn("10 / Turn 8 - Woodcote", TimeSpan.FromMilliseconds(71), 1)
            .AddStraight("11 / National Pits Straight 1", TimeSpan.FromMilliseconds(88))
            .AddTurn("12 / Turn 9 - Copse", TimeSpan.FromMilliseconds(18), 2)
            .AddStraight("13 / National Pits Straight 2", TimeSpan.FromMilliseconds(77))
            .AddTurn("14 / Turn 10 - Maggots 1", TimeSpan.FromMilliseconds(17), 2)
            .AddTurn("15 / Turn 11 - Maggots 2", TimeSpan.FromMilliseconds(15), 2)
            .AddTurn("16 / Turn 12 - Becketts 1", TimeSpan.FromMilliseconds(20), 2)
            .AddTurn("17 / Turn 13 - Becketts 2", TimeSpan.FromMilliseconds(34), 2)
            .AddTurn("18 / Turn 14 - Chapel", TimeSpan.FromMilliseconds(17), 2)
            .AddStraight("19 / Hangar Straight", TimeSpan.FromMilliseconds(152))
            .AddTurn("20 / Turn 15 - Stowe", TimeSpan.FromMilliseconds(43), 2)
            .AddPitLane(TimeSpan.FromMilliseconds(230), TimeSpan.FromMilliseconds(200), Teams.All, 2)
            .AddStraight("22 / No name Straight", TimeSpan.FromMilliseconds(68))
            .AddTurn("23 / Turn 16 - Vale 1", TimeSpan.FromMilliseconds(18), 1)
            .AddTurn("24 / Turn 17 - Vale 2", TimeSpan.FromMilliseconds(52), 1)
            .AddTurn("25 / Turn 18 - Club", TimeSpan.FromMilliseconds(32), 2)
            .AddStraight("26 / Hamilton Straight before grid", TimeSpan.FromMilliseconds(43));
    }
}