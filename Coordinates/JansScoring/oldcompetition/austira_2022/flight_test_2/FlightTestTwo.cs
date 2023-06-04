using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.flight_test_2;

public class FlightTestTwo : Flight
{
    public override int getFlightNumber()
    {
        return 1;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 07, 04, 00, 00);
    }

    public override int launchPeriode()
    {
        return 180;
    }



    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\training flight 3\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task7(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }


    private class Task7 : Task
    {
        public Task7(Flight flight) : base(flight)
        {
        }
        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 07, 08, 00, 00);
        }

        public override int number()
        {
            return 7;
        }

        public override string[] score(Track track)
        {
            string comment = "";
            Declaration baseDecleration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);
            if (baseDecleration == null)
            {
                return new[] { "No Result", $"No decleration found" };
            }

            Coordinate launchPoint;
            if (TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out launchPoint, out _))
            {
                double distance = CalculationHelper.Calculate2DDistance(launchPoint, baseDecleration.DeclaredGoal,
                    flight.getCalculationType());
                if (distance <= flight.distanceToAllGoals())
                {
                    comment +=
                        $"The distance of the startposition is to close to the decleration ({NumberHelper.formatDoubleToStringAndRound(distance)}m)";
                }
            }
            else
            {
                comment += "Cannot check start Position to all goals | ";
            }

            for (int i = 1; i <= 9; i++)
            {
                Declaration trackDeclaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == i);
                if (baseDecleration == trackDeclaration) continue;
                if (trackDeclaration == null) continue;
                if (trackDeclaration.DeclaredGoal == null) continue;
                if (trackDeclaration.PositionAtDeclaration == null) continue;

                double distanceGoalToDeclerationPoint = CalculationHelper.Calculate2DDistance(
                    baseDecleration.DeclaredGoal,
                    trackDeclaration.DeclaredGoal, flight.getCalculationType());
                if (distanceGoalToDeclerationPoint <= 1000)
                {
                    return new[]
                    {
                        "No Result",
                        $"The Declared Goal was to close to goal {trackDeclaration.GoalNumber}  ({NumberHelper.formatDoubleToStringAndRound(distanceGoalToDeclerationPoint)}m)"
                    };
                }
            }

            Declaration goalDecleration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);
            if (goalDecleration == null) return new[] { "No Result", "No decleration" };
            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
            if (markerDrop == null) return new[] { "No Result", "No marker" };
            return new[]
            {
                NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.Calculate3DDistance(
                    goalDecleration.DeclaredGoal,
                    markerDrop.MarkerLocation, flight.useGPSAltitude(), flight.getCalculationType())),
                comment
            };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }
    }
}