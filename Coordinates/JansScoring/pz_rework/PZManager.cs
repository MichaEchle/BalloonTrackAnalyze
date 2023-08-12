using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using JansScoring.pz_rework.type;
using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace JansScoring.pz_rework;

public class PZManager
{
    private List<PZ> pzs = new();

    public PZManager()
    {
        pzs.Add(new RedPZ(1, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 472850, 5367350, 3000),CoordinateHelpers.ConvertToMeter(3000d), 500));
        pzs.Add(new RedPZ(2, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479900 , 5364120, 3000),CoordinateHelpers.ConvertToMeter(3000d), 500));
        pzs.Add(new RedPZ(3, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 471780 , 5357200, 3000),CoordinateHelpers.ConvertToMeter(3000d), 500));
        pzs.Add(new RedPZ(4, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485750 , 5358850, 3000),CoordinateHelpers.ConvertToMeter(3000d), 500));

        pzs.Add(new BluePZ(21, , CoordinateHelpers.ConvertToMeter(9000), Double.MaxValue));
    }

    public string checkPZ(Flight flight, Track track)
    {
        String comment = "";


        foreach (PZ pz in pzs)
        {
            Coordinate lastTrackPoint = null;

            List<PZInfrigement> infrigements = new List<PZInfrigement>();
            double distanceInPZ = 0;

            DateTime infrigementBegin = DateTime.MinValue;


            foreach (Coordinate trackTrackPoint in track.TrackPoints)
            {
                if (pz.IsInsidePz(flight, track, trackTrackPoint, out String ignore))
                {
                    if (lastTrackPoint != null && trackTrackPoint != null)
                    {
                        distanceInPZ += CalculationHelper.Calculate2DDistance(trackTrackPoint, lastTrackPoint,
                            flight.getCalculationType());
                    }
                    else
                    {
                        infrigementBegin = trackTrackPoint.TimeStamp;
                    }

                    lastTrackPoint = trackTrackPoint;
                }
                else
                {
                    if (lastTrackPoint != null)
                    {
                        infrigements.Add(new PZInfrigement(infrigementBegin, trackTrackPoint.TimeStamp, distanceInPZ));
                        lastTrackPoint = null;
                        infrigementBegin = DateTime.MinValue;
                        distanceInPZ = 0;
                    }
                }
            }

            if (infrigements.Count != 0)
            {
                String infigement = "";
                foreach (PZInfrigement pzInfrigement in infrigements)
                {
                    infigement += pzInfrigement.infrigementBegin + " " + pzInfrigement.infrigementEnd + " " +
                                  pzInfrigement.distance + "m | ";
                }

                comment +=
                    $"Pilot has {infrigements.Count} {pz.GetType().Name} infringement(s) with PZ: '{pz.ID}' [{infigement}] | ";
            }
        }

        return comment;
    }
}