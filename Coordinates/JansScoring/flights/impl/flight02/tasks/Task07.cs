using Coordinates;
using System;
using Windows.ApplicationModel.Contacts;

namespace JansScoring.flights.impl.flight02.tasks;

public class Task07 : TaskPDDounat
{
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 7;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 2);

        if (declaration == null || declaration.DeclaredGoal == null)
        {
            comment += " No declaration found. | ";
            return NO_RESULT;
        }
        

        Coordinate corner = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 514000, 5353000);


        if (declaration.PositionAtDeclaration.Longitude < corner.Longitude)
        {
            comment += " Easting is too low. | ";
        }

        if (declaration.PositionAtDeclaration.Latitude < corner.Latitude)
        {
            comment += " Northing is too low. | ";
        }


        return NORMAL_CALCULATION;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 07, 30, 00);
    }

    protected override int declerationNumber()
    {
        return 2;
    }

    protected override int distanceFromDeclerationPointToCenter()
    {
        return 3000;
    }

    protected override int innerCircle()
    {
        return 2000;
    }

    protected override int outerCircle()
    {
        return 3000;
    }

    protected override bool reenter()
    {
        return true;
    }

    protected override int bottomPlateHeightM()
    {
        return 0;
    }

    protected override int maxDeclerations()
    {
        return Int32.MaxValue;
    }

    protected override bool validateStartPoint()
    {
        return false;
    }
}