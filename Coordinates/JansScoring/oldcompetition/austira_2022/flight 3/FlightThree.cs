using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.flight_3;

//Implementing Methodes
public class FlightThree : Flight
{
    public override int getFlightNumber()
    {
        return 3;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2022, 09, 09, 03, 55, 00);
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
        return 0;
    }

    public override string getTracksPath()
    {
        return @"C:\balloon\comp\2022_09_Bad_Waltersdorf\flights\flight 3\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task8(this),
            /*
             * new Task9(this), new Task10(this), new Task11(this), 
            
             */ 
            new Task12(this),
            new Task13(this)
            , new Task14(this)
        };
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

    public class Task8 : Task
    {
        public Task8(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 8;
        }

        public override string[] score(Track track)
        {
            Console.WriteLine($"Start scoring Pilot {track.Pilot.PilotNumber} for task {this.number()}");

            string result = "";
            string comment = "";

            Declaration decleration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);
            if (decleration == null)
            {
                return new[] { "No Result", "No decleration found." };
            }

            Coordinate startPoint = null;
            Coordinate endPoint = null;
            foreach (Coordinate trackPoint in track.TrackPoints)
            {
                (string utmZone, double easting, double northing) =
                    CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(trackPoint);
                if (northing <= 5225500)
                {
                    if (startPoint == null)
                    {
                        startPoint = trackPoint;
                        comment += $"Found startpoint at [{startPoint.TimeStamp.ToString("HH:mm:ss")}] | ";
                    }

                    if (northing <= 5225000)
                    {
                        endPoint = trackPoint;
                        comment += $"Found endpoint at [{endPoint.TimeStamp.ToString("HH:mm:ss")}] | ";
                        break;
                    }
                }
            }

            if (double.IsNaN(decleration.DeclaredGoal.easting))
            {
                return new[] { "No Result", "The decleration has no easting" };
            }

            string time = decleration.DeclaredGoal.easting.ToString();
            int timelenght = time.Length;
            if (timelenght < 4)
            {
                for (int i = 0; i < (4 - timelenght); i++)
                {
                    time = "0" + time;
                }
            }

            Console.WriteLine(time);
            comment += $"Time: {time} | ";
            string minuteString = time.Substring(0, 2);
            comment += $"minuteString: {time} | ";
            int minutes = int.Parse(minuteString);
            string secondString = time.Substring(2, 2);
            int seconds = int.Parse(secondString);

            comment += $"Declared {minutes}:{seconds} | ";
            seconds += minutes * 60;
            comment += $"Declared Secands and Minutes {seconds} | ";

            DateTime addSeconds = startPoint.TimeStamp.AddSeconds(seconds);
            comment += $"time of Decleration: {addSeconds.ToString("HH:mm:ss")}";

            double timeDelta = (addSeconds - endPoint.TimeStamp).TotalSeconds;
            result = NumberHelper.formatDoubleToStringAndRound(Math.Abs(timeDelta));
            Console.WriteLine($"Finish scoring Pilot {track.Pilot.PilotNumber} for task {this.number()}");
            return new[] { result, comment };
        }

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 09, 07, 00, 00);
        }
    }

    public class Task9 : Task
    {
        public Task9(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            throw new NotImplementedException();
        }

        public override string[] score(Track track)
        {
            throw new NotImplementedException();
        }

        public override Coordinate[] goals()
        {
            throw new NotImplementedException();
        }

        public override DateTime getScoringPeriodeUntil()
        {
            throw new NotImplementedException();
        }
    }

    public class Task10 : Task
    {
        public Task10(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            throw new NotImplementedException();
        }

        public override string[] score(Track track)
        {
            throw new NotImplementedException();
        }

        public override Coordinate[] goals()
        {
            throw new NotImplementedException();
        }

        public override DateTime getScoringPeriodeUntil()
        {
            throw new NotImplementedException();
        }
    }

    public class Task11 : Task
    {
        public Task11(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            throw new NotImplementedException();
        }

        public override string[] score(Track track)
        {
            throw new NotImplementedException();
        }

        public override Coordinate[] goals()
        {
            throw new NotImplementedException();
        }

        public override DateTime getScoringPeriodeUntil()
        {
            throw new NotImplementedException();
        }
    }

    public class Task12 : Task
    {
        public Task12(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 12;
        }

        public override string[] score(Track track)
        {
            string comment = "";


            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);

            if (markerDrop == null)
            {
                return new[] { "No Result", "No markerdrop found!" };
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
                comment += $"The distance is less than the MMA, the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})";
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
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 578860, 5221170,
                    CoordinateHelpers.ConvertToMeter(893)),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33T", 578990, 5220560,
                    CoordinateHelpers.ConvertToMeter(889)),
            };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 09, 07, 30, 00);
        }
    }

    public class Task13 : Task
    {
        public Task13(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 13;
        }

        public override string[] score(Track track)
        {
            string comment = "";
            string result = "";

            List<Declaration> declarations = track.Declarations.FindAll(declaration => declaration.GoalNumber == 2);


            if (declarations == null || !declarations.Any())
            {
                return new[] { "No Result", $"No Declerations" };
            }

            int amountDecleration = declarations.Count;
            if (amountDecleration > 2)
            {
                return new[] { "No Result", $"More than 2 decleration ({amountDecleration})" };
            }

            MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
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

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 08, 08, 00, 00);
        }
    }

    //Angle
    public class Task14 : Task
    {
        public Task14(Flight flight) : base(flight)
        {
        }

        public override int number()
        {
            return 14;
        }

        public override string[] score(Track track)
        {
            Console.WriteLine($"Start scoring Pilot {track.Pilot.PilotNumber} for task {this.number()}");

            string result = "";
            string comment = "";

            MarkerDrop markerDrop5 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
            if (markerDrop5 == null)
            {
                return new[] { "No Result", "No Marker 5 found." };
            }

            (string utmZoneM5, double eastingM5, double northingM5) =
                CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerDrop5.MarkerLocation);
            MarkerDrop markerDrop6 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);
            if (markerDrop6 == null)
            {
                return new[] { "No Result", "No Marker 6 found." };
            }

            (string utmZoneM6, double eastingM6, double northingM6) =
                CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerDrop6.MarkerLocation);

            double trieAngleBottom = Math.Abs(eastingM5 - eastingM6);
            double trieAngleWall = Math.Abs(northingM5 - northingM6);
            double alpha = Math.Atan2(trieAngleBottom, trieAngleWall) * 180 / PI;
            double beta = 220 - alpha;
            if (beta > 180)
            {
                beta = (360 - 220) + alpha;
            }

            comment +=
                $"Angle Bottom: {NumberHelper.formatDoubleToStringAndRound(trieAngleBottom)} | Angle Wall {NumberHelper.formatDoubleToStringAndRound(trieAngleWall)} | Alpha: {NumberHelper.formatDoubleToStringAndRound(alpha)} | Beta: {NumberHelper.formatDoubleToStringAndRound(beta)} | ";
            result = NumberHelper.formatDoubleToStringAndRound(Math.Abs(beta));

            Console.WriteLine($"Finish scoring Pilot {track.Pilot.PilotNumber} for task {this.number()}");
            return new[] { result, comment };
        }

        private double PI =
            3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;

        public override Coordinate[] goals()
        {
            return new Coordinate[] { };
        }

        public override DateTime getScoringPeriodeUntil()
        {
            return new DateTime(2022, 09, 09, 08, 30, 00);
        }
    }
}