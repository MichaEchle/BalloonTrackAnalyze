using Competition;
using Coordinates;
using JansScoring.calculation;
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
                                $"Pilot has a Blue-PZ infringement [{trackPoint.TimeStamp.ToString("HH:mm:ss")}] | ";
                            break;
                        }
                    }

                    break;
                case PZType.RED:
                    foreach (Coordinate trackPoint in track.TrackPoints)
                    {
                        double disctanceBetweenRedPZ =
                            CalculationHelper.Calculate2DDistance(trackPoint, pz.center, flight.getCalculationType());
                        if (disctanceBetweenRedPZ <= pz.radius &&
                            (flight.useGPSAltitude() ? trackPoint.AltitudeGPS : trackPoint.AltitudeBarometric) <=
                            pz.height)
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
                    double distanceBetweenStartAndYellowPZ =
                        CalculationHelper.Calculate2DDistance(launchPoint, pz.center, flight.getCalculationType());
                    if (distanceBetweenStartAndYellowPZ <= pz.radius)
                    {
                        comment +=
                            $"Pilot has a Yellow-PZ infringement for starting. | ";
                    }

                    double distanceBetweenLandingAndYellowPZ =
                        CalculationHelper.Calculate2DDistance(track.TrackPoints.Last(), pz.center,
                            flight.getCalculationType());
                    if (distanceBetweenLandingAndYellowPZ <= pz.radius)
                    {
                        comment +=
                            $"Pilot has a Yellow-PZ infringement for landing. | ";
                    }

                    break;
            }
        }

        return comment;
    }
}