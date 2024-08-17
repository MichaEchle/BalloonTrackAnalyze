using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using OfficeOpenXml;
using System;

namespace JansScoring.flights.impl._04.tasks;

public class Task18 : TaskFON
{
    public Task18(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 18;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            return true;
        }

        DeclarationChecks.CheckMaxReDeclaration(track, DeclarationNumber(), 3, ref comment);

        DeclarationChecks.CheckIfDeclarationGoalWasAboveHeight(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1500), ref comment);

        DeclarationChecks.CheckHeightBetweenDeclarationPointAndDeclaredGoal(Flight, declaration,
            CoordinateHelpers.ConvertToMeter(1000), ref comment);

        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 2000, ref comment);

        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, declaration, 2000, ref comment);
        return false;
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            result = Double.MinValue;
            return;
        }

        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            result = Double.MinValue;
            return;
        }

        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);


        DeclarationChecks.CheckIfDeclarationWasBeforeMarkerDrop(declaration, markerDrop, ref comment);

        double distanceFromMarkerToDeclaration = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal,
            markerDrop.MarkerLocation, Flight.useGPSAltitude(), Flight.getCalculationType());
        double distanceFromDeclarationToDeclaredGoal = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal,
            declaration.PositionAtDeclaration, Flight.getCalculationType());

        comment += $"Dheight: {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)} | DPHeight: {NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.ConvertToFeet(declaration.PositionAtDeclaration.AltitudeBarometric))}";
        comment += $"Dheight m: {declaration.DeclaredGoal.AltitudeBarometric} | DPHeight m: {declaration.PositionAtDeclaration.AltitudeBarometric}";
        result = distanceFromMarkerToDeclaration / distanceFromDeclarationToDeclaredGoal;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 16, 07, 00, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 9;
    }
}