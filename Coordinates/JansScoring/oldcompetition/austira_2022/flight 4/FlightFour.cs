using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;

namespace JansScoring.flights.flight_4;

public class FlightFour : Flight
{
    public override int getFlightNumber()
    {
        return 4;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 10, 03, 55, 00);
    }

    public override int launchPeriode()
    {
        return 65;
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
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 4\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task15(this), new Task16(this), new Task17(this), new Task18(this), new Task20(this) };
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

    public class Task15 : Task
    {
        public Task15(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 15;
        }

        public override string[] score(Track track)
        {
            string result = "";
            string comment = "";

            Declaration decleration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);
            if (decleration == null)
            {
                return new[] { "No Result", "No decleration found" };
            }

            double distanceBetweenDecPointAndGoal = CalculationHelper.Calculate2DDistance(
                decleration.PositionAtDeclaration, decleration.DeclaredGoal, flight.getCalculationType());

            if (distanceBetweenDecPointAndGoal < 1000)
            {
                comment +=
                    $"Distance between decleration point and goal is to small ({NumberHelper.formatDoubleToStringAndRound(distanceBetweenDecPointAndGoal)} <> Penalties: {NumberHelper.formatDoubleToStringAndRound(CalculationHelper.calculateDistancePanelties(1000, distanceBetweenDecPointAndGoal))}) | ";
            }

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
            if (markerDrop == null)
            {
                return new[] { "No Result", "No marker found" };
            }


            result = NumberHelper.formatDoubleToStringAndRound(CoordinateHelpers.Calculate3DDistance(
                decleration.DeclaredGoal, markerDrop.MarkerLocation, flight.useGPSAltitude(),
                flight.getCalculationType()));
            return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 07, 00, 00);
        }
    }

    public class Task16 : Task
    {
        public Task16(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 16;
        }

        public override string[] score(Track track)
        {
            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);

            if (markerDrop == null)
            {
                return new[] { "No Result", "No Marker drop" };
            }

            List<double> distances = null;
            if ((flight.useGPSAltitude()
                    ? markerDrop.MarkerLocation.AltitudeGPS
                    : markerDrop.MarkerLocation.AltitudeBarometric) > flight.getSeperationAltitudeMeters())
            {
                distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.useGPSAltitude(),
                    flight.getCalculationType());
            }
            else
            {
                distances = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.getCalculationType());
            }

            double result = Double.MaxValue;
            foreach (double distance in distances)
            {
                if (distance < result)
                {
                    result = distance;
                }
            }


            if (result < 30)
            {
                comment +=
                    $"The distance is less than the MMA <> the result must be 30m ({NumberHelper.formatDoubleToStringAndRound(result)})";
                result = 30;
            }

            if (result == Double.MaxValue)
                return new[] { "No Result", "There was no distances to goals calculated" };

            return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
        }

        public override Coordinate[] goals()
        {
            return new[]
            {
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 576870, 5224360,
                    CoordinateHelpers.ConvertToMeter(928))
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 07, 00, 00);
        }
    }

    public class Task17 : Task
    {
        public Task17(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 17;
        }

        public override string[] score(Track track)
        {
            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);

            if (markerDrop == null)
            {
                return new[] { "No Result", "No Marker drop" };
            }

            List<double> distances = null;
            if ((flight.useGPSAltitude()
                    ? markerDrop.MarkerLocation.AltitudeGPS
                    : markerDrop.MarkerLocation.AltitudeBarometric) > flight.getSeperationAltitudeMeters())
            {
                distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.useGPSAltitude(),
                    flight.getCalculationType());
            }
            else
            {
                distances = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.getCalculationType());
            }

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
                comment +=
                    $"The distance is less than the MMA <> the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})";
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
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 578431, 5223976,
                    CoordinateHelpers.ConvertToMeter(1207))
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 07, 30, 00);
        }
    }

    public class Task18 : Task
    {
        public Task18(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 18;
        }

        public override string[] score(Track track)
        {
            string comment = "";

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);

            if (markerDrop == null)
            {
                return new[] { "No Result", "No Marker drop" };
            }

            List<double> distances = null;
            if ((flight.useGPSAltitude()
                    ? markerDrop.MarkerLocation.AltitudeGPS
                    : markerDrop.MarkerLocation.AltitudeBarometric) > flight.getSeperationAltitudeMeters())
            {
                distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.useGPSAltitude(),
                    flight.getCalculationType());
            }
            else
            {
                distances = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                    flight.getCalculationType());
            }

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
                comment +=
                    $"The distance is less than the MMA <> the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})";
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
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 584045, 5221738,
                    CoordinateHelpers.ConvertToMeter(961))
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 07, 30, 00);
        }
    }

    public class Task20 : Task
    {
        public Task20(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 20;
        }

        public override string[] score(Track track)
        {
            string result = "";
            string comment = "";

            MarkerDrop a = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
            MarkerDrop b = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);
            MarkerDrop c = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 7);

            if (a == null)
            {
                return new[] { "No Result", "No Markerdrop A" };
            }

            if (b == null)
            {
                return new[] { "No Result", "No Markerdrop B" };
            }

            if (c == null)
            {
                return new[] { "No Result", "No Markerdrop C" };
            }


            if (a.MarkerTime > b.MarkerTime || a.MarkerTime > c.MarkerTime || b.MarkerTime > c.MarkerTime)
            {
                comment += "Wrong marker order | ";
                MarkerDrop tempA = null;
                MarkerDrop tempB = null;
                MarkerDrop tempC = null;
                if (a.MarkerTime > b.MarkerTime)
                {
                    if (a.MarkerTime > c.MarkerTime)
                    {
                        tempC = a;
                    }
                    else
                    {
                        tempB = a;
                    }
                }
                else
                {
                    tempA = a;
                }

                if (b.MarkerTime > a.MarkerTime)
                {
                    if (b.MarkerTime > c.MarkerTime)
                    {
                        tempC = b;
                    }
                    else
                    {
                        tempB = b;
                    }
                }
                else
                {
                    tempA = b;
                }
                
                
                if (c.MarkerTime > a.MarkerTime)
                {
                    if (c.MarkerTime > b.MarkerTime)
                    {
                        tempC = c;
                    }
                    else
                    {
                        tempB = c;
                    }
                }
                else
                {
                    tempA = c;
                }

                a = tempA;
                b = tempB;
                c = tempC;
            }


            double lengthAB =
                CalculationHelper.Calculate2DDistance(a.MarkerLocation, b.MarkerLocation, flight.getCalculationType());
            double lengthBC =
                CalculationHelper.Calculate2DDistance(b.MarkerLocation, c.MarkerLocation, flight.getCalculationType());
            double lengthCA =
                CalculationHelper.Calculate2DDistance(c.MarkerLocation, a.MarkerLocation, flight.getCalculationType());


            double penalties = 0;


            if (lengthAB < 2000)
            {
                double additionalPanelties = CalculationHelper.calculateDistancePanelties(2000, lengthAB);
                penalties += additionalPanelties;
                comment +=
                    $"AB is to short ({NumberHelper.formatDoubleToStringAndRound(lengthAB)}) (added {NumberHelper.formatDoubleToStringAndRound(additionalPanelties)} Penalties) | ";
            }
            else if (lengthAB > 2500)
            {
                double additionalPanelties = CalculationHelper.calculateDistancePanelties(2500, lengthAB);
                penalties += additionalPanelties;
                comment +=
                    $"AB is to long ({NumberHelper.formatDoubleToStringAndRound(lengthAB)}) (added {NumberHelper.formatDoubleToStringAndRound(additionalPanelties)} Penalties) | ";
            }

            if (lengthBC < 2000)
            {
                double additionalPanelties = CalculationHelper.calculateDistancePanelties(2000, lengthBC);
                penalties += additionalPanelties;
                comment +=
                    $"BC is to short ({NumberHelper.formatDoubleToStringAndRound(lengthBC)}) (added {NumberHelper.formatDoubleToStringAndRound(additionalPanelties)} Penalties) | ";
            }
            else if (lengthBC > 2500)
            {
                double additionalPanelties = CalculationHelper.calculateDistancePanelties(2500, lengthBC);
                penalties += additionalPanelties;
                comment +=
                    $"BC is to long ({NumberHelper.formatDoubleToStringAndRound(lengthBC)}) (added {NumberHelper.formatDoubleToStringAndRound(additionalPanelties)} Penalties) | ";
            }
            

            double s = (lengthAB + lengthBC + lengthCA) / 2;
            result = NumberHelper.formatDoubleToStringAndRound(Math.Sqrt(s * (s - lengthAB) * (s - lengthBC) *
                                                                         (s - lengthCA)));
            if(penalties != 0)
                comment += $"All Penalties: {NumberHelper.formatDoubleToStringAndRound(penalties)}";
            return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 10, 07, 30, 00);
        }
    }
}