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
        pzs.Add(new RedPZ(1, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 511170, 5357350), CoordinateHelpers.ConvertToMeter(3500), 500));
        pzs.Add(new RedPZ(2, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 508340, 5352860), CoordinateHelpers.ConvertToMeter(3000), 200));
        pzs.Add(new RedPZ(3, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 512450, 5360300), CoordinateHelpers.ConvertToMeter(3000), 500));
        pzs.Add(new RedPZ(4, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 507620, 5344240), CoordinateHelpers.ConvertToMeter(3500), 300));
        pzs.Add(new RedPZ(5, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 505230, 5342240), CoordinateHelpers.ConvertToMeter(3500), 300));
        pzs.Add(new RedPZ(6, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 504010, 5343010), CoordinateHelpers.ConvertToMeter(3500), 300));
        pzs.Add(new RedPZ(7, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 504100, 5345400), CoordinateHelpers.ConvertToMeter(3500), 300));
        pzs.Add(new RedPZ(8, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 530870, 5361290), CoordinateHelpers.ConvertToMeter(3000), 500));
        pzs.Add(new RedPZ(9, CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 531050, 5359170), CoordinateHelpers.ConvertToMeter(3000), 500));

            
        pzs.Add(new BluePZ(90, "/home/codingphoenix/Documents/balloon/competitions/2026 BWLV Sonnenbühl/maps/PZ blue 3500ft.plt",CoordinateHelpers.ConvertToMeter(3000), Double.MaxValue));
        pzs.Add(new BluePZ(91, "/home/codingphoenix/Documents/balloon/competitions/2026 BWLV Sonnenbühl/maps/PZ blue 4500ft.plt",CoordinateHelpers.ConvertToMeter(4000), Double.MaxValue));
        pzs.Add(new BluePZ(92, "/home/codingphoenix/Documents/balloon/competitions/2026 BWLV Sonnenbühl/maps/PZ blue 5500ft.plt",CoordinateHelpers.ConvertToMeter(5000), Double.MaxValue));
        pzs.Add(new BluePZ(93, "/home/codingphoenix/Documents/balloon/competitions/2026 BWLV Sonnenbühl/maps/PZ blue 7500ft.plt",CoordinateHelpers.ConvertToMeter(7000), Double.MaxValue));
        pzs.Add(new BluePZ(99, CoordinateHelpers.ConvertToMeter(9000), Double.MaxValue));
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
                            flight.CalculationType());
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