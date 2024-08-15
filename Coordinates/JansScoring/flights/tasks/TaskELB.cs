using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.tasks;

public abstract class TaskELB : Task
{
    protected TaskELB(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        MarkerChecks.LoadMarker(track, MarkerNumberA(), out MarkerDrop markerDropA, ref comment);
        MarkerChecks.LoadMarker(track, MarkerNumberB(), out MarkerDrop markerDropB, ref comment);
        MarkerChecks.LoadMarker(track, MarkerNumberC(), out MarkerDrop markerDropC, ref comment);
        if (markerDropA == null || markerDropB == null || markerDropC == null)
        {
            result = Double.MinValue;
            return;
        }

        MarkerChecks.CheckScoringPeriode(this, markerDropA, ref comment);
        MarkerChecks.CheckScoringPeriode(this, markerDropB, ref comment);
        MarkerChecks.CheckScoringPeriode(this, markerDropC, ref comment);


        if (markerDropA.MarkerTime > markerDropB.MarkerTime || markerDropA.MarkerTime > markerDropC.MarkerTime ||
            markerDropB.MarkerTime > markerDropC.MarkerTime)
        {
            comment += "Wrong marker order | ";
            MarkerDrop tempA = null;
            MarkerDrop tempB = null;
            MarkerDrop tempC = null;
            if (markerDropA.MarkerTime > markerDropB.MarkerTime)
            {
                if (markerDropA.MarkerTime > markerDropC.MarkerTime)
                {
                    tempC = markerDropA;
                }
                else
                {
                    tempB = markerDropA;
                }
            }
            else
            {
                tempA = markerDropA;
            }

            if (markerDropB.MarkerTime > markerDropA.MarkerTime)
            {
                if (markerDropB.MarkerTime > markerDropC.MarkerTime)
                {
                    tempC = markerDropB;
                }
                else
                {
                    tempB = markerDropB;
                }
            }
            else
            {
                tempA = markerDropB;
            }


            if (markerDropC.MarkerTime > markerDropA.MarkerTime)
            {
                if (markerDropC.MarkerTime > markerDropB.MarkerTime)
                {
                    tempC = markerDropC;
                }
                else
                {
                    tempB = markerDropC;
                }
            }
            else
            {
                tempA = markerDropC;
            }

            markerDropA = tempA;
            markerDropB = tempB;
            markerDropC = tempC;
        }

        //double a = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropB.MarkerLocation.Latitude * (PI / 180) * Math.Sin(markerDropC.MarkerLocation.Latitude * (PI / 180) + Math.Cos(markerDropB.MarkerLocation.Latitude * (PI / 180) * Math.Cos(markerDropC.MarkerLocation.Latitude * (PI / 180) * Math.Cos(markerDropB.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * (PI / 180)) * 1000;
        //double b = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropA.MarkerLocation.Latitude * (PI / 180) * Math.Sin(markerDropC.MarkerLocation.Latitude * (PI / 180) + Math.Cos(markerDropA.MarkerLocation.Latitude * (PI / 180) * Math.Cos(markerDropC.MarkerLocation.Latitude * (PI / 180)* Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * (PI / 180)) * 1000;
        //double c = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropA.MarkerLocation.Latitude * (PI / 180) * Math.Sin(markerDropB.MarkerLocation.Latitude * (PI / 180) + Math.Cos(markerDropA.MarkerLocation.Latitude * (PI / 180) * Math.Cos(markerDropB.MarkerLocation.Latitude * (PI / 180) * Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropB.MarkerLocation.Longitude) * (PI / 180))* 1000;


        double a = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropB.MarkerLocation.Latitude * (PI / 180)) * Math.Sin(markerDropC.MarkerLocation.Latitude * (PI / 180)) + Math.Cos(markerDropB.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropC.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropB.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * (PI / 180)) * 1000;
        double b = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropA.MarkerLocation.Latitude * (PI / 180)) * Math.Sin(markerDropC.MarkerLocation.Latitude * (PI / 180)) + Math.Cos(markerDropA.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropC.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * (PI / 180)) * 1000;
        double c = FAI_EARTH_RADIUS * Math.Acos(Math.Sin(markerDropA.MarkerLocation.Latitude * (PI / 180)) * Math.Sin(markerDropB.MarkerLocation.Latitude * (PI / 180)) + Math.Cos(markerDropA.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropB.MarkerLocation.Latitude * (PI / 180)) * Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropB.MarkerLocation.Longitude) * (PI / 180))* 1000;

        double cosBetta = (a*a +  c*c - b*b) / 2 * a*c;
        double betta = 180 / PI * Math.Acos(cosBetta);
        result = 180 - betta;
        return;
    }

    private readonly int FAI_EARTH_RADIUS = 6371;

    private readonly double PI =
        3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;

    protected abstract int MarkerNumberA();
    protected abstract int MarkerNumberB();
    protected abstract int MarkerNumberC();

    public override Coordinate[] Goals()
    {
        return Array.Empty<Coordinate>();
    }
}