using Competition;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace HNBC2026;

internal class Flight2
{
    private ILogger<Flight2> _logger = LogConnector.LoggerFactory.CreateLogger<Flight2>();
    private double _separationAltitude = CoordinateHelpers.ConvertToMeter(2400);
    private Flight _flight = Flight.GetInstance();

    private Coordinate _goalTask7 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476843, 5360188, 509);
    private Coordinate _goalTask8A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 475831, 5361475, CoordinateHelpers.ConvertToMeter(1808));
    private Coordinate _goalTask8B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476011, 5362280, CoordinateHelpers.ConvertToMeter(1756));
    private Coordinate _goalTask8C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 477301, 5363263, 552);


    internal void ScoreFlight()
    {
        _logger.LogInformation("Scoring Flight 2");
        _flight.FlightNumber = 2;
        _flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(3500));
        if (!_flight.ParseTrackFiles(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026 Horb Tracks\Flight 2\F2 IGC", true))
        {
            _logger.LogError("Error parsing track files");
            return;
        }
        if (!_flight.MapPilotNamesToTracks(@"C:\Users\micechle\Dropbox\2026-08-05 Horb Scoring\2026-Horb Documents\HNBC_26_Pilots_Mapping.csv"))
        {
            _logger.LogError("Error mapping pilot names to tracks");
            return;
        }
        _flight.Tracks = [.. _flight.Tracks.OrderBy(x => x.Pilot.PilotNumber)];

        Task7_HNH();
    }

    private void Task7_HNH()
    {
        foreach (var track in _flight.Tracks)
        {
            MarkerDrop? marker1 = track.GetFirstMarkerDrop(1);
            if (marker1 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 1", track.Pilot.PilotNumber);
                continue;
            }

            if(marker1.MarkerLocation.TimeStamp>new DateTime(2026,08,06,18,50,00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 1 after the deadline", track.Pilot.PilotNumber);
                continue;
            }

            double distanceToGoal = CoordinateHelpers.CalculateDistanceWithSeparationAltitude( _goalTask7,marker1.MarkerLocation, _separationAltitude, true);
            double result = Math.Round(Math.Max(distanceToGoal, 30), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task7: {result}m ({distanceToGoal})", track.Pilot.PilotNumber, result, distanceToGoal);
        }
    }

    private void Task8_HWZ()
    {
        foreach (var track in _flight.Tracks)
        {
            MarkerDrop? marker2 = track.GetFirstMarkerDrop(2);
            if (marker2 is null)
            {
                _logger.LogWarning("Pilot {pilotNumber} did not drop marker 2", track.Pilot.PilotNumber);
                continue;
            }
            if (marker2.MarkerLocation.TimeStamp > new DateTime(2026, 08, 06, 18, 50, 00))
            {
                _logger.LogWarning("Pilot {pilotNumber} dropped marker 2 after the deadline", track.Pilot.PilotNumber);
                continue;
            }
            HesitationWaltzTask hesitationWaltzTask = new HesitationWaltzTask();
            hesitationWaltzTask.SetupHWZ(8, [_goalTask8A, _goalTask8B, _goalTask8C], 2, DistanceCalculationType.WithSeparationAlitude, null, ValidationStrictnessType.First);
            hesitationWaltzTask.SeparationAltitude = _separationAltitude;

            if (!hesitationWaltzTask.CalculateResults(track, true, out double distance))
            {
                _logger.LogWarning("Failed to calculate distance for Pilot {pilotNumber} at Task8", track.Pilot.PilotNumber);
            }
            double result = Math.Round(Math.Max(distance, 50), 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task8: {result}m ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }

    private void Task9_3D()
    {
        foreach (var track in _flight.Tracks)
        {
            List<Declaration> declarations = [.. track.Declarations.Where(d => d.GoalNumber == 1).OrderBy(x => x.PositionAtDeclaration.TimeStamp)];
            Track? tempTrack = null;
            for (int index = 0; index < declarations.Count; index++)
            {
                if (declarations.Count == 0)
                {
                    break;
                }


                double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declarations[index].PositionAtDeclaration, declarations[index].DeclaredGoal);
                if (distanceDeclarationToGoal < 2000)
                {
                    continue;
                }
                else
                {
                    if (declarations.Count == 1|| index==declarations.Count-1)
                    {
                        tempTrack = new Track();
                        tempTrack.Pilot = track.Pilot;
                        tempTrack.TrackPoints.AddRange(track.TrackPoints.Where(x => x.TimeStamp < new DateTime(2026, 08, 06, 18, 50, 00)));
                        tempTrack.Declarations.Add(declarations[index]);
                        break;
                    }
                    if (index < declarations.Count - 1)
                    {
                        Coordinate? firstTrackPoint = track.TrackPoints.FirstOrDefault(x => CoordinateHelpers.Calculate2DDistanceHavercos(x, declarations[index].DeclaredGoal) < 2000);
                        if (firstTrackPoint is null)
                        {
                            //_logger.LogWarning("No track points within 2000m of declared goal for Pilot {pilotNumber} at Task9", track.Pilot.PilotNumber);
                            continue;
                        }
                        if (firstTrackPoint.TimeStamp > declarations[index + 1].PositionAtDeclaration.TimeStamp)
                        {
                            continue;
                        }
                        else
                        {
                            
                            tempTrack = new Track();
                            tempTrack.Pilot = track.Pilot;
                            tempTrack.TrackPoints.AddRange(track.TrackPoints.Where(x=>x.TimeStamp<new DateTime(2026,08,06,18,50,00)));
                            tempTrack.Declarations.Add(declarations[index]);
                            break;
                        }
                    }
                }


            }
            if (tempTrack is null)
            {
                _logger.LogWarning("No valid declarations for Pilot {pilotNumber} at Task9", track.Pilot.PilotNumber);
                continue;
            }

            DonutTask donutTask = new DonutTask();
            donutTask.SetupDonut(9, 1, 1, 1000, 2000, double.NaN, double.NaN, true, null, ValidationStrictnessType.First);

            if (!donutTask.CalculateResults(tempTrack, true, out double distance))
            {
                _logger.LogWarning("Failed to calculate distance for Pilot {pilotNumber} at Task9", track.Pilot.PilotNumber);
            }
            double result = Math.Round(distance, 0, MidpointRounding.AwayFromZero);
            _logger.LogInformation("Pilot {pilotNumber} result for Task9: {result}m ({distance})", track.Pilot.PilotNumber, result, distance);
        }
    }
}
