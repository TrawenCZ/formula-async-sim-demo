using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Race;

public class Race
{
    private Track _track;
    private readonly IEnumerable<RaceCar> _cars;
    private readonly int _numberOfLaps;
    public Dictionary<RaceCar, List<Lap>> RaceResults { get; private set; }
    public Race(IEnumerable<RaceCar> cars, Track track, int numberOfLaps)
    {
        _track = track;
        _numberOfLaps = numberOfLaps;
        _cars = cars;
        RaceResults = cars.ToDictionary(car => car, car => new List<Lap>());
    }
    
    public async Task StartAsync()
    {
        bool shouldEnd = false;
        bool shouldEndPrinting = false;
        SemaphoreSlim printingTicketGiver = new SemaphoreSlim(0, _cars.Count() + 1);
        SemaphoreSlim timeGetterMutex = new SemaphoreSlim(1, 1);
        ConcurrentQueue<(TimeSpan TimeSinceStart, string Driver, int Lap)> printingQueue = new ConcurrentQueue<(TimeSpan, string, int)>();
        ManualResetEventSlim startSignal = new ManualResetEventSlim(false);
        var tasks = new List<Task>();
        var ticksAtRaceStart = 0L;

        foreach (var car in _cars)
        {
            tasks.Add(Task.Run(async () =>
            {
                List<TrackPointPass> trackPointsPasses;
                TimeSpan timeElapsedSinceStart;
                TimeSpan totalDriveTime = TimeSpan.Zero;
                List<Lap> laps = new List<Lap>();
                int lap = 1;
                startSignal.Wait();
                while (!shouldEnd)
                {
                    trackPointsPasses = new List<TrackPointPass>();
                    foreach (var trackPoint in _track.GetLap(car))
                    {
                        trackPointsPasses.Add(await trackPoint.PassAsync(car));
                    }

                    await timeGetterMutex.WaitAsync();
                    timeElapsedSinceStart = Stopwatch.GetElapsedTime(ticksAtRaceStart);
                    timeGetterMutex.Release();

                    printingQueue.Enqueue((timeElapsedSinceStart, car.Driver, lap));
                    printingTicketGiver.Release();      // sending printing ticket

                    if (lap == _numberOfLaps) shouldEnd = true;
                    RaceResults[car].Add(new Lap(car, lap, trackPointsPasses, timeElapsedSinceStart - totalDriveTime));
                    totalDriveTime = timeElapsedSinceStart;                   
                    car.GetCurrentTire().AddLap();
                    lap++;
                }
            }));
        }

        var printingTask = Task.Run(async () =>
        {
            string timeFormattingString = @"mm\:ss\.ff";
            int lapPrintCounter = 0;
            TimeSpan currentBestTimeSinceStart = TimeSpan.Zero;
            while (true)
            {
                await printingTicketGiver.WaitAsync();      // waiting for printing ticket
                while (printingQueue.TryDequeue(out var queueItem))
                {
                    if (queueItem.Lap > lapPrintCounter)
                    {
                        Console.WriteLine($"{(queueItem.Lap == 1 ? string.Empty : '\n')}Lap: {queueItem.Lap}\n{queueItem.Driver}: {queueItem.TimeSinceStart.ToString(timeFormattingString)}");
                        currentBestTimeSinceStart = queueItem.TimeSinceStart;
                        lapPrintCounter++;
                    } else if (queueItem.Lap == lapPrintCounter)
                    {
                        Console.WriteLine($"{queueItem.Driver}: +{(queueItem.TimeSinceStart - currentBestTimeSinceStart).ToString(timeFormattingString)}");
                    }
                }
                if (shouldEndPrinting && printingQueue.Count == 0) return;
            }
        });

        ticksAtRaceStart = Stopwatch.GetTimestamp();
        startSignal.Set();
        await Task.WhenAll(tasks);
        shouldEndPrinting = true;
        printingTicketGiver.Release();
        await printingTask;
    }
}