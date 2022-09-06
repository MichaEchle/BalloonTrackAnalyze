using Competition;
using Coordinates;
using System.Collections.Generic;
using System.Linq;
using Flight = JansScoring.flights.Flight;

namespace JansScoring.pz;

public class PZManager
{
    private List<PZ> pzs = new();

    public void registerPZs()
    {
        pzs.Add(new PZ(-1, PZType.BLUE, null, 10000, -1));
    }


    public string checkPZ(Flight flight, Track track)
    {
        string comment = "";
        foreach (PZ pz in pzs)
        {
            switch (pz.pzType)
            {
                case PZType.BLUE:
                    foreach (Coordinate trackPoint in track.TrackPoints)
                    {
                        if ((flight.useGPSAltitude() ? trackPoint.AltitudeGPS : trackPoint.AltitudeBarometric) >=
                            pz.height)
                        {
                            comment +=
                                $"Pilot has a Blue-PZ infringement [{trackPoint.TimeStamp.ToString("HH:mm:ss")}]  | ";
                            break;
                        }
                    }

                    break;
                case PZType.RED:
                    foreach (Coordinate trackPoint in track.TrackPoints)
                    {
                        double disctanceBetweenRedPZ = CoordinateHelpers.Calculate2DDistanceHavercos(trackPoint, pz.center);
                        if (disctanceBetweenRedPZ <= pz.radius && (flight.useGPSAltitude() ? trackPoint.AltitudeGPS : trackPoint.AltitudeBarometric) <= pz.height)
                        {
                            comment +=
                                $"Pilot has a Red-PZ infringement (PZ: {pz.id}) [{trackPoint.TimeStamp.ToString("HH:mm:ss")}]  | ";
                            break;
                        }
                    }
                    break;
                    case PZType.YELLOW:
                        Coordinate launchPoint;
                        if (!TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out launchPoint,
                                out _))
                            break;
                        double distanceBetweenStartAndYellowPZ = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, pz.center);
                        if (distanceBetweenStartAndYellowPZ <= pz.radius)
                        {
                            comment +=
                                $"Pilot has a Yellow-PZ infringement for starting. | ";
                        }

                        double distanceBetweenLandingAndYellowPZ = CoordinateHelpers.Calculate2DDistanceHavercos(track.TrackPoints.Last(), pz.center);
                        if (distanceBetweenLandingAndYellowPZ <= pz.radius)
                        {
                            comment +=
                                $"Pilot has a Yellow-PZ infringement for landing. | ";
                        }
                        break;
            }
        }

        return (comment.Length > 0 ? comment.Substring(0, comment.Length - 3) : comment);
    }
}