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


        /*
         * double halfPI = PI / 180;

        double a = FAI_EARTH_RADIUS * Math.Acos(
            Math.Sin(markerDropB.MarkerLocation.Latitude * halfPI) *
            Math.Sin(markerDropC.MarkerLocation.Latitude * halfPI) +
            Math.Cos(markerDropB.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropC.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropB.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * halfPI) * 1000;

        double b = FAI_EARTH_RADIUS * Math.Acos(
            Math.Sin(markerDropA.MarkerLocation.Latitude * halfPI) *
            Math.Sin(markerDropC.MarkerLocation.Latitude * halfPI) +
            Math.Cos(markerDropA.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropC.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropC.MarkerLocation.Longitude) * halfPI) * 1000;

        double c = FAI_EARTH_RADIUS * Math.Acos(
            Math.Sin(markerDropA.MarkerLocation.Latitude * halfPI) *
            Math.Sin(markerDropB.MarkerLocation.Latitude * halfPI) +
            Math.Cos(markerDropA.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropB.MarkerLocation.Latitude * halfPI) *
            Math.Cos(markerDropA.MarkerLocation.Longitude - markerDropB.MarkerLocation.Longitude) * halfPI) * 1000;

        double cosBetta = (a * a + c * c - b * b) / (2 * a * c);
        double betta = 180 / PI * Math.Acos(cosBetta);

        Console.WriteLine("A: " + a);
        Console.WriteLine("B: " + b);
        Console.WriteLine("C: " + c);
        Console.WriteLine("CosBetta: " + cosBetta);
        Console.WriteLine("Betta: " + betta);
        Console.WriteLine("HalfPI: " + halfPI);
        result = 180 - betta;
         */


        result = CalculateChangeOfDirection(
            (markerDropA.MarkerLocation.Longitude, markerDropA.MarkerLocation.Latitude),
            (markerDropB.MarkerLocation.Longitude, markerDropB.MarkerLocation.Latitude),
            (markerDropC.MarkerLocation.Longitude, markerDropC.MarkerLocation.Latitude)
        );

        return;
    }

    private readonly int FAI_EARTH_RADIUS = 6371;

    private readonly double PI =
        3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;

    protected abstract int MarkerNumberA();
    protected abstract int MarkerNumberB();
    protected abstract int MarkerNumberC();

    public override Coordinate[] Goals(int pilot)
    {
        return Array.Empty<Coordinate>();
    }

    public static double CalculateChangeOfDirection((double X, double Y) pointA, (double X, double Y) pointB,
        (double X, double Y) pointC)
    {
        // Berechne die Vektoren BA und BC
        var vectorBA = (X: pointA.X - pointB.X, Y: pointA.Y - pointB.Y);
        var vectorBC = (X: pointC.X - pointB.X, Y: pointC.Y - pointB.Y);
        Console.WriteLine("vectorBA: " + vectorBA);
        Console.WriteLine("vectorBC: " + vectorBC);

        // Berechne das Skalarprodukt der Vektoren
        double dotProduct = vectorBA.X * vectorBC.X + vectorBA.Y * vectorBC.Y;
        Console.WriteLine("dotProduct: " + dotProduct);

        // Berechne die Längen der Vektoren
        double magnitudeBA = Math.Sqrt(vectorBA.X * vectorBA.X + vectorBA.Y * vectorBA.Y);
        double magnitudeBC = Math.Sqrt(vectorBC.X * vectorBC.X + vectorBC.Y * vectorBC.Y);
        Console.WriteLine("magnitudeBA: " + magnitudeBA);
        Console.WriteLine("magnitudeBC: " + magnitudeBC);

        // Berechne den Kosinus des Winkels
        double cosAngle = dotProduct / (magnitudeBA * magnitudeBC);
        Console.WriteLine("CosAngle: " + cosAngle);

        // Kosinuswert auf den Bereich [-1, 1] beschränken, um Rundungsfehler zu vermeiden
        cosAngle = Math.Max(-1, Math.Min(1, cosAngle));
        Console.WriteLine("CosAngle: " + cosAngle);

        // Berechne den Winkel in Grad
        double angleABC = Math.Acos(cosAngle) * (180.0 / Math.PI);
        Console.WriteLine("AngleABC: " + angleABC);
        // Ergebnis: 180 Grad - Winkel ABC
        double result = 180.0 - angleABC;

        return result;
    }

}