using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl;

public class Task03 : Task
{
    public Task03(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 03;
    }

    public override string[] score(Track track)
    {
        string result = "";
        string comment = "";


        Coordinate firstTrackPoint = null;
        Coordinate lastTrackPoint = null;

        MarkerDrop marker6 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);

        DateTime logger6DropTime = DateTime.MaxValue;

        if (marker6 != null)
        {
            logger6DropTime = marker6.MarkerTime;
        }
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No markerdrop in 5" };
        }

        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop #5 outside SP | ";
        }

        (double latitude, double longitude) = CoordinateHelpers.ConvertUTMToLatitudeLongitude("32U", 655000, 5524000);
        foreach (Coordinate trackPoint in track.TrackPoints)
        {
            if (firstTrackPoint == null)
            {

                if (trackPoint.Latitude < latitude && trackPoint.Longitude > longitude)
                {
                    firstTrackPoint = trackPoint;
                    comment += $"StartPoint: {trackPoint.TimeStamp.ToString("HH:mm:ss")} | ";
                    track.AdditionalPropertiesFromScoring.Add("firstTrackPoint", firstTrackPoint);
                }
            }
            else
            {
                if (trackPoint.Latitude >= latitude)
                {
                    lastTrackPoint = trackPoint;
                    comment += $"LastPoint: {trackPoint.TimeStamp.ToString("HH:mm:ss")} (Northing) | ";

                    track.AdditionalPropertiesFromScoring.Add("lastTrackPoint", lastTrackPoint);
                    return new[]
                    {
                        NumberHelper.formatDoubleToStringAndRound(
                            CalculationHelper.Calculate2DDistance(firstTrackPoint, lastTrackPoint,flight.getCalculationType())),
                        comment
                    };
                }

                if (trackPoint.TimeStamp.Subtract(firstTrackPoint.TimeStamp).TotalMinutes >= 20)
                {
                    lastTrackPoint = trackPoint;
                    comment += $"LastPoint: {trackPoint.TimeStamp.ToString("HH:mm:ss")} (Time) | ";
                    track.AdditionalPropertiesFromScoring.Add("lastTrackPoint", lastTrackPoint);
                    break;
                }

                if (trackPoint.TimeStamp > (markerDrop.MarkerTime))
                {
                    lastTrackPoint = trackPoint;
                    comment += $"LastPoint: {trackPoint.TimeStamp.ToString("HH:mm:ss")} (MarkerDrop #5) | ";
                    track.AdditionalPropertiesFromScoring.Add("lastTrackPoint", lastTrackPoint);
                    break;
                }

                if (logger6DropTime != DateTime.MaxValue && trackPoint.TimeStamp >logger6DropTime)
                {
                    lastTrackPoint = trackPoint;
                    comment += $"LastPoint: {trackPoint.TimeStamp.ToString("HH:mm:ss")} (MarkerDrop #6) | ";
                    track.AdditionalPropertiesFromScoring.Add("lastTrackPoint", lastTrackPoint);
                    break;
                }
            }
        }

        if (firstTrackPoint == null)
        {
            comment += track.TrackPoints[100].Longitude + " | ";
            comment += track.TrackPoints[100].Latitude + " | ";
            return new[] { "No Result", comment + $"Not flown over {latitude} and below {longitude}" };
        }



        if (markerDrop.MarkerTime.CompareTo(firstTrackPoint.TimeStamp) <= 0)
        {
            comment += "Dropped marker before entering valid zone. | ";
            return new[] { "No Result", comment };
        }

        if (markerDrop.MarkerTime.Subtract(firstTrackPoint.TimeStamp).TotalMinutes >= 20)
        {
            comment +=
                $"Dropped marker after 20 min. [{markerDrop.MarkerTime.Subtract(firstTrackPoint.TimeStamp).TotalSeconds} s] | ";
        }


        return new[]
        {
            NumberHelper.formatDoubleToStringAndRound(
                CalculationHelper.Calculate2DDistance(firstTrackPoint, markerDrop.MarkerLocation, flight.getCalculationType())),
            comment
        };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 01, 13, 00, 00);
    }
}