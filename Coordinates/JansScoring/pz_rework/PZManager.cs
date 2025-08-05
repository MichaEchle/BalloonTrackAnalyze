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
    private readonly List<PZ> pzs = new();

    public PZManager()
    {
        pzs.Add(new BluePZ(01, CoordinateHelpers.ConvertToMeter(9000), Double.MaxValue));
        pzs.Add(new RedPZ(02, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 511130, 5327080), CoordinateHelpers.ConvertToMeter(2000), 500));
        pzs.Add(new RedPZ(03, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 503670, 5328130), CoordinateHelpers.ConvertToMeter(1500), 200));
        pzs.Add(new RedPZ(04, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508870, 5327880), CoordinateHelpers.ConvertToMeter(1500), 200));
        pzs.Add(new RedPZ(05, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 515630, 5325190), CoordinateHelpers.ConvertToMeter(2000), 300));

        pzs.Add(new YellowPZ(06, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 501900, 5325070),200));
        pzs.Add(new YellowPZ(07, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 506840, 5328380),500));
        pzs.Add(new YellowPZ(08, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 524410, 5329570),500));
    }

    public string CheckPz(Flight flight, Track track)
    {
        String comment = "";

        Console.WriteLine($"Start checking PZ for Pilot {track.Pilot.PilotNumber}.");

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
                                  NumberHelper.formatDoubleToStringAndRound(pzInfrigement.distance) + "m | ";
                }

                comment +=
                    $"Pilot has {infrigements.Count} {pz.GetType().Name} infringement(s) with PZ: '{pz.ID}' [{infigement}] | ";
            }
        }
        Console.WriteLine($"Finish checking PZ for Pilot {track.Pilot.PilotNumber}.");

        return comment;
    }
}