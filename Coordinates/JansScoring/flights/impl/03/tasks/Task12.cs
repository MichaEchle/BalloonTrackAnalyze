using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;
using Windows.Globalization;

namespace JansScoring.flights.impl._03.tasks;

public class Task12 : Task3DTDounat
{
    public Task12(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 12;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);
        if (declaration == null)
        {
            return true;
        }

        int amountDeclaration = track.Declarations.FindAll(dec => dec.GoalNumber == DeclarationNumber()).Count;
        comment += "Amount Declaration: " + amountDeclaration.ToString() + "| ";
        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 3000, ref comment);
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 18, 00, 00);
    }

    public override int DeclarationNumber()
    {
        return 2;
    }

    public override bool PilotDeclared()
    {
        return true;
    }

    public override double OuterRadiusMeters()
    {
        return 2000;
    }

    public override double InnerRadiusInMeters()
    {
        return 1000;
    }

    public override double MinHeightInMeters()
    {
        return CoordinateHelpers.ConvertToMeter(1500);
    }

    public override double MaxHeightInMeters()
    {
        return Double.MaxValue;
    }

    public override bool ReEnter()
    {
        return true;
    }
}