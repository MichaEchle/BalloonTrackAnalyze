using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.flight_1;

public class FlightOne : Flight
{
    public override int getFlightNumber()
    {
        return 1;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 08, 04, 30, 00);
    }

    public override int launchPeriode()
    {
        return 60;
    }


    public override bool useGPSAltitude()
    {
        return false;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 1\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task1(this), new Task5(this), new Task6(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.UTMPrecise;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }

    public override Coordinate getBackupCoordinates()
    {
        throw new NotImplementedException();
    }

    public class Task1 : Task
    {
        public Task1(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 1;
        }

        public override DateTime getScoringPeriodUntil()
        {
            return new DateTime(2022, 09, 07, 06, 30, 00);
        }

        public override string[] score(Track track)
        {
            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
            if (markerDrop == null)
            {
                return new[] { "No Result", "No electronical markerdrop" };
            }

            string comment = "";
            string scoringPeriodeComment;
            if (checkScoringPeriodeForMarker(track, markerDrop, out scoringPeriodeComment))
            {
                comment += scoringPeriodeComment;
            }

            List<double> distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                flight.useGPSAltitude(),
                flight.getCalculationType());


            double result = Double.MaxValue;
            foreach (double distance in distances)
            {
                if (distance < result)
                {
                    result = distance;
                }
            }


            if (result < 50)
            {
                comment += $"The distance is less than the MMA, the result must be 50m ({result})";
                result = 50;
            }

            if (result == Double.MaxValue)
                return new[] { "No Result", "There was no distances to goals calculated" };

            return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
        }

        public override Coordinate[] goals()
        {
            return new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 574440, 5221810,
                    CoordinateHelpers.ConvertToMeter(1180))
            };
        }
    }


    //Maximum Distance
    public class Task5 : Task
    {
        public Task5(Flight flight) : base(flight)
        {
            box1 = new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 586000, 5233000),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 587000, 5234000)
            };
            box2 = new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 587000, 5234000),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 588000, 5235000)
            };
            box3 = new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 588000, 5233000),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 589000, 5234000)
            };
            box4 = new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 587000, 5232000),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 588000, 5233000)
            };
            box5 = new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 587000, 5233000),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 588000, 5234000)
            };
        }

        public override int number()
        {
            return 5;
        }

        private Coordinate circleMiddlePoint =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 584000, 5235000);

        private Coordinate circleCalculationPoint =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 575949, 5224649);

        public override string[] score(Track track)
        {
            string result = "";

            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
            if (markerDrop == null)
            {
                return new string[] { "No Result", "No markerdrop was found" };
            }

            string scoringPeriodeComment;
            if (checkScoringPeriodeForMarker(track, markerDrop, out scoringPeriodeComment))
            {
                comment += scoringPeriodeComment;
            }

            double distanceFromCircleToMarkerdrop = CalculationHelper.Calculate2DDistance(circleMiddlePoint,
                markerDrop.MarkerLocation, flight.getCalculationType());
            if (distanceFromCircleToMarkerdrop < 1000)
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in circle ({distanceFromCircleToMarkerdrop}m)"
                };
            }


            if (isInsiteLongLatBox(markerDrop.MarkerLocation, box1))
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in Box 1"
                };
            }

            if (isInsiteLongLatBox(markerDrop.MarkerLocation, box2))
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in Box 2"
                };
            }

            if (isInsiteLongLatBox(markerDrop.MarkerLocation, box3))
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in Box 3"
                };
            }

            if (isInsiteLongLatBox(markerDrop.MarkerLocation, box4))
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in Box 4"
                };
            }

            if (isInsiteLongLatBox(markerDrop.MarkerLocation, box5))
            {
                return new string[]
                {
                    NumberHelper.formatDoubleToStringAndRound(
                        CalculationHelper.Calculate2DDistance(circleCalculationPoint, markerDrop.MarkerLocation,
                            flight.getCalculationType())),
                    $"Marked in Box 5"
                };
            }

            result = "No Result";
            comment = "Outside the scoring area";
            return new string[] { result, comment };
        }

        private bool isInsiteLongLatBox(Coordinate coordinate, Coordinate[] box)
        {
            (string markerUTMZone, int markerEasting, int markerNorthing) =
                CoordinateHelpers.ConvertLatitudeLongitudeToUTM(coordinate.Latitude, coordinate.Longitude);
            (string box1utmZone, int box1easting, int box1northing) =
                CoordinateHelpers.ConvertLatitudeLongitudeToUTM(box[0].Latitude, box[0].Longitude);
            (string box2utmZone, int box2easting, int box2northing) =
                CoordinateHelpers.ConvertLatitudeLongitudeToUTM(box[1].Latitude, box[1].Longitude);

            if (Math.Min(box1easting, box2easting) > markerEasting ||
                markerEasting > Math.Max(box1easting, box2easting))
            {
                return false;
            }

            if (Math.Min(box1northing, box2northing) > markerNorthing ||
                markerNorthing > Math.Max(box1northing, box2northing))
            {
                return false;
            }

            return true;
        }

        private Coordinate[] box1 = null;
        private Coordinate[] box2 = null;
        private Coordinate[] box3 = null;
        private Coordinate[] box4 = null;
        private Coordinate[] box5 = null;

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }


        public override DateTime getScoringPeriodUntil()
        {
            return new DateTime(2022, 09, 08, 07, 03, 00);
        }
    }

    public class Task6 : Task
    {
        public Task6(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 6;
        }

        public override string[] score(Track track)
        {
            string comment = "";
            string result = "";

            List<Declaration> declarations = track.Declarations.FindAll(declaration => declaration.GoalNumber == 2);


            if (declarations == null || !declarations.Any())
            {
                List<Declaration> declerationsIn6 =
                    track.Declarations.FindAll(declaration => declaration.GoalNumber == 6);
                if (declerationsIn6 == null || !declerationsIn6.Any())
                {
                    return new[] { "No Result", $"No Declerations" };
                }

                comment += "Used wrong decleration slot. Using decleration slot 6 | ";
                declarations = declerationsIn6;
            }

            int amountDecleration = declarations.Count;
            if (amountDecleration > 2)
            {
                return new[] { "No Result", $"More than 2 decleration ({amountDecleration})" };
            }

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);
            if (markerDrop == null)
            {
                return new[] { "No Result", "No Markerdrop found" };
            }

            Declaration lastValideDeclaration = declarations.Last();
            double goalAltitude = flight.useGPSAltitude()
                ? lastValideDeclaration.DeclaredGoal.AltitudeGPS
                : lastValideDeclaration.DeclaredGoal.AltitudeBarometric;
            double declerationAltitude = flight.useGPSAltitude()
                ? lastValideDeclaration.PositionAtDeclaration.AltitudeGPS
                : lastValideDeclaration.PositionAtDeclaration.AltitudeBarometric;

            if (goalAltitude < declerationAltitude + CoordinateHelpers.ConvertToMeter(500) &&
                goalAltitude > declerationAltitude - CoordinateHelpers.ConvertToMeter(500))
            {
                double hightDifferenz = Math.Abs(goalAltitude - declerationAltitude);
                double needed = 500 - hightDifferenz;
                double percent = (needed / 500) * 10;
                double panelties = percent * 20;
                if (percent > 25)
                {
                    comment +=
                        $"The goal was not heigher or lower than 500 above or lower the declerationpoint (Goal: {NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.ConvertToFeet(goalAltitude))}ft | Declerationpoint {NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.ConvertToFeet(declerationAltitude))}ft) [Percent: {NumberHelper.formatDoubleToStringAndRound(percent)}% | Panelties: No Result]";
                }
                else
                {
                    comment +=
                        $"The goal was not heigher or lower than 500 above or lower the declerationpoint (Goal: {NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.ConvertToFeet(goalAltitude))}ft | Declerationpoint {NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.ConvertToFeet(declerationAltitude))}ft) [Percent: {NumberHelper.formatDoubleToStringAndRound(percent)}% | Panelties: {NumberHelper.formatDoubleToStringAndRound(panelties)}]";
                }
            }

            result = NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.Calculate3DDistance(
                lastValideDeclaration.DeclaredGoal, markerDrop.MarkerLocation, flight.useGPSAltitude(),
                flight.getCalculationType()));
            return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public override DateTime getScoringPeriodUntil()
        {
            return new DateTime(2022, 09, 08, 07, 30, 00);
        }
    }
}