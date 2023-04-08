using hw04.TrackPoints;

namespace hw04.Race;

public record TrackPointPass (ITrackPoint TrackPoint, TimeSpan WaitingTime, TimeSpan DrivingTime);