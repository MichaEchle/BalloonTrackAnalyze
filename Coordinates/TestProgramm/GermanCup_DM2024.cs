using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProgramm;
internal class GermanCup_DM2024
{
    private readonly ILogger<GermanCup_DM2024> Logger = LogConnector.LoggerFactory.CreateLogger<GermanCup_DM2024>();
    private readonly Flight Flight = Flight.GetInstance();

    private readonly double SeparationAltitude = CoordinateHelpers.ConvertToMeter(2000);

    ////////////////////////////////////////////////////////////////////////////////////
    // Flight1
    ////////////////////////////////////////////////////////////////////////////////////
    #region Flight1

    private readonly Coordinate T2_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 483825, 5418581, CoordinateHelpers.ConvertToMeter(790));
    private readonly Coordinate T2_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 486125, 5419764, CoordinateHelpers.ConvertToMeter(1016));
    private readonly Coordinate T2_HWZ_C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485823, 5417826, CoordinateHelpers.ConvertToMeter(1273));
    #endregion Flight1


    ////////////////////////////////////////////////////////////////////////////////////
    // Flight2
    ////////////////////////////////////////////////////////////////////////////////////
    #region Flight2
    private readonly Coordinate T5_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485822, 5417828, 385);
    private readonly Coordinate T5_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485940, 5416179, CoordinateHelpers.ConvertToMeter(1075));

    private readonly Coordinate T6_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 491478, 5421114, CoordinateHelpers.ConvertToMeter(903));
    private readonly Coordinate T6_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 490782, 5416905, CoordinateHelpers.ConvertToMeter(1124));

    #endregion Flight2


    ////////////////////////////////////////////////////////////////////////////////////
    // Flight3
    ////////////////////////////////////////////////////////////////////////////////////
    #region Flight3
    private readonly Coordinate T7_FIN = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485191, 5419222, CoordinateHelpers.ConvertToMeter(766));

    #endregion Flight3



    ////////////////////////////////////////////////////////////////////////////////////
    // Flight 4
    ////////////////////////////////////////////////////////////////////////////////////
    #region Flight 4
    private readonly Coordinate T10_FIN = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485823, 5417827, CoordinateHelpers.ConvertToMeter(1280));

    private readonly Coordinate T12_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 476893, 5418111, CoordinateHelpers.ConvertToMeter(1123));
    private readonly Coordinate T12_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479490, 5418770, 333);
    private readonly Coordinate T12_HWZ_C = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480527, 5419942, CoordinateHelpers.ConvertToMeter(1011));
    #endregion Flight 4

    internal void ChecksFlight1()
    {
        Flight.FlightNumber = 1;
        Flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(2000));
        if (!Flight.MapPilotNamesToTracks(@"C:\TEMP\GermanCup_DM2024\PilotMapping.csv"))
        {
            Logger.LogError("Failed to map pilot names to tracks");
            return;
        }
        if (!Flight.ParseTrackFiles(@"C:\TEMP\GermanCup_DM2024\Flight1_27_09_AM", true))
        {
            Logger.LogError("Failed to parse track files");
            return;
        }
        Flight.Tracks = Flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        foreach (Track track in Flight.Tracks)
        {
            CheckDistancesFlight1(track);
        }
    }

    private void CheckDistancesFlight1(Track track)
    {
        Declaration declaration1 = track.GetLatestDeclaration(1);
        if (declaration1 is null)
        {
            Logger.LogError("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
            return;
        }
        Declaration declaration2 = track.GetLatestDeclaration(2);
        if (declaration2 is null)
        {
            Logger.LogError("No declaration found for goal 2 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
            return;
        }
        double distanceDeclaration1ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.PositionAtDeclaration, declaration1.DeclaredGoal);
        Logger.LogInformation("Distance from declaration 1 to goal 1 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclaration1ToGoal1, (distanceDeclaration1ToGoal1 >= 1000 ? "OK" : "INVALID"));
        double distanceHWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T2_HWZ_A);
        double distanceHWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T2_HWZ_B);
        double distanceHWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T2_HWZ_C);
        Logger.LogInformation("Distance from goal to HWZ A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_A, (distanceHWZ_A >= 1000 ? "OK" : "INVALID"));
        Logger.LogInformation("Distance from goal to HWZ B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_B, (distanceHWZ_B >= 1000 ? "OK" : "INVALID"));
        Logger.LogInformation("Distance from goal to HWZ C for pilot {track.Pilot.PilotNumber}: {distanceHWZ_C}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_C, (distanceHWZ_C >= 1000 ? "OK" : "INVALID"));

        double distanceDeclartion2ToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.PositionAtDeclaration, declaration2.DeclaredGoal);
        Logger.LogInformation("Distance from declaration 2 to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclartion2ToGoal2, (distanceDeclartion2ToGoal2 >= 1000 ? "OK" : "INVALID"));

        double distanceGoal1ToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, declaration2.DeclaredGoal);
        Logger.LogInformation("Distance from goal 1 to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceGoal1ToGoal2}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceGoal1ToGoal2, (distanceGoal1ToGoal2 >= 1000 ? "OK" : "INVALID"));
    }


    internal void ChecksFlight2()
    {
        Flight.FlightNumber = 2;
        Flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(2000));
        if (!Flight.MapPilotNamesToTracks(@"C:\TEMP\GermanCup_DM2024\PilotMapping.csv"))
        {
            Logger.LogError("Failed to map pilot names to tracks");
            return;
        }
        if (!Flight.ParseTrackFiles(@"C:\TEMP\GermanCup_DM2024\Flight2_27_09_PM", true))
        {
            Logger.LogError("Failed to parse track files");
            return;
        }
        Flight.Tracks = Flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        foreach (Track track in Flight.Tracks)
        {
            CheckDistancesFlight2(track);
        }
        foreach (Track track in Flight.Tracks)
        {
            CalcMarkerDistanceTask5(track);
        }
        foreach (Track track in Flight.Tracks)
        {
            CalcMarkerDistanceTask6(track);
        }
    }
    private void CheckDistancesFlight2(Track track)
    {
        Declaration declaration1 = track.GetLatestDeclaration(1);
        if (declaration1 is null)
        {
            Logger.LogError("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
            return;
        }
        double distanceDeclaration1ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.PositionAtDeclaration, declaration1.DeclaredGoal);
        Logger.LogInformation("Distance from declaration 1 to goal 1 for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclaration1ToGoal1, (distanceDeclaration1ToGoal1 >= 1000 ? "OK" : "INVALID"));
        double distanceHWZ_5A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T5_HWZ_A);
        double distanceHWZ_5B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T5_HWZ_B);
        double distanceHWZ_6A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T6_HWZ_A);
        double distanceHWZ_6B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration1.DeclaredGoal, T6_HWZ_B);
        Logger.LogInformation("Distance from goal to HWZ 5A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_5A, (distanceHWZ_5A >= 1000 ? "OK" : "INVALID"));
        Logger.LogInformation("Distance from goal to HWZ 5B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_5B, (distanceHWZ_5B >= 1000 ? "OK" : "INVALID"));
        Logger.LogInformation("Distance from goal to HWZ 6A for pilot {track.Pilot.PilotNumber}: {distanceHWZ_A}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_6A, (distanceHWZ_6A >= 1000 ? "OK" : "INVALID"));
        Logger.LogInformation("Distance from goal to HWZ 6B for pilot {track.Pilot.PilotNumber}: {distanceHWZ_B}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceHWZ_6B, (distanceHWZ_6B >= 1000 ? "OK" : "INVALID"));
    }


    private void CalcMarkerDistanceTask5(Track track)
    {
        MarkerDrop marker2 = track.MarkerDrops.First(x => x.MarkerNumber == 2);
        double distanceMarker2ToHWZ_5A = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T5_HWZ_A, marker2.MarkerLocation, SeparationAltitude, true);
        double distanceMarker2ToHWZ_5B = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T5_HWZ_B, marker2.MarkerLocation, SeparationAltitude, true);
        Logger.LogInformation("Task 5 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(Math.Min(distanceMarker2ToHWZ_5A, distanceMarker2ToHWZ_5B), 0, MidpointRounding.AwayFromZero));
    }

    private void CalcMarkerDistanceTask6(Track track)
    {
        MarkerDrop marker3 = track.MarkerDrops.First(x => x.MarkerNumber == 3);
        double distanceMarker3ToHWZ_6A = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T6_HWZ_A, marker3.MarkerLocation, SeparationAltitude, true);
        double distanceMarker3ToHWZ_6B = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T6_HWZ_B, marker3.MarkerLocation, SeparationAltitude, true);
        Logger.LogInformation("Task 6 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(Math.Min(distanceMarker3ToHWZ_6A, distanceMarker3ToHWZ_6B), 0, MidpointRounding.AwayFromZero));
    }

    internal void Flight2_ManualScoreP18()
    {
        //Flight2 Pilot 18 special handling
        Declaration declaration = new Declaration(1, new Coordinate(48.9022981575426, 8.77144168565184, 487.68, 487.68, DateTime.Parse("9/27/2024  3:38:23 PM")), new Coordinate(48.888615, 8.72398333333333, 322, 322, DateTime.Parse("9/27/2024  3:38:23 PM")), true, 83250, 16620);
        MarkerDrop marker1 = new MarkerDrop(1, new Coordinate(48.902015, 8.77164166666667, 745, 745, DateTime.Parse("9/27/2024  4:18:49 PM")));
        MarkerDrop marker2 = new MarkerDrop(2, new Coordinate(48.9140483333333, 8.80575166666667, 507, 507, DateTime.Parse("9/27/2024  4:23:39 PM")));
        MarkerDrop marker3 = new MarkerDrop(3, new Coordinate(48.94218, 8.884475, 291, 291, DateTime.Parse("9/27/2024  4:36:42 PM")));

        Coordinate T5_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485822, 5417828, 385);
        Coordinate T5_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 485940, 5416179, CoordinateHelpers.ConvertToMeter(1075));

        Coordinate T6_HWZ_A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 491478, 5421114, CoordinateHelpers.ConvertToMeter(903));
        Coordinate T6_HWZ_B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 490782, 5416905, CoordinateHelpers.ConvertToMeter(1124));


        double distanceGoal1Marker1 = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, marker1.MarkerLocation, true);

        ILogger<Program> logger = LogConnector.LoggerFactory.CreateLogger<Program>();
        logger.LogInformation("Distance between goal 1 and marker 1: {distanceGoal1Marker1}[m]", Math.Round(distanceGoal1Marker1, 0, MidpointRounding.AwayFromZero));

        double distanceHWZ_5A = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T5_HWZ_A, marker2.MarkerLocation, CoordinateHelpers.ConvertToMeter(2000), true);
        double distanceHWZ_5B = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T5_HWZ_B, marker2.MarkerLocation, CoordinateHelpers.ConvertToMeter(2000), true);

        logger.LogInformation("Distance between HWZ 5 and marker 2: {distanceHWZ_5}[m]", Math.Round(Math.Min(distanceHWZ_5A, distanceHWZ_5B), 0, MidpointRounding.AwayFromZero));

        double distanceHWZ_6A = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T6_HWZ_A, marker3.MarkerLocation, CoordinateHelpers.ConvertToMeter(2000), true);
        double distanceHWZ_6B = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T6_HWZ_B, marker3.MarkerLocation, CoordinateHelpers.ConvertToMeter(2000), true);

        logger.LogInformation("Distance between HWZ 6 and marker 3: {distanceHWZ_6}[m]", Math.Round(Math.Min(distanceHWZ_6A, distanceHWZ_6B), 0, MidpointRounding.AwayFromZero));

        double distanceGoal1HWZ5A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, T5_HWZ_A);
        double distanceGoal1HWZ5B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, T5_HWZ_B);
        double distanceGoal1HWZ6A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, T6_HWZ_A);
        double distanceGoal1HWZ6B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, T6_HWZ_B);

        logger.LogInformation("Distance between goal 1 and HWZ 5A: {distanceGoal1HWZ5A}[m]", Math.Round(distanceGoal1HWZ5A, 0, MidpointRounding.AwayFromZero));
        logger.LogInformation("Distance between goal 1 and HWZ 5B: {distanceGoal1HWZ5B}[m]", Math.Round(distanceGoal1HWZ5B, 0, MidpointRounding.AwayFromZero));
        logger.LogInformation("Distance between goal 1 and HWZ 6A: {distanceGoal1HWZ6A}[m]", Math.Round(distanceGoal1HWZ6A, 0, MidpointRounding.AwayFromZero));
        logger.LogInformation("Distance between goal 1 and HWZ 6B: {distanceGoal1HWZ6B}[m]", Math.Round(distanceGoal1HWZ6B, 0, MidpointRounding.AwayFromZero));

    }


    //internal void ChecksFlight3()
    //{
    //    Flight.FlightNumber = 3;
    //    Flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(2000));
    //    if (!Flight.MapPilotNamesToTracks(@"C:\TEMP\GermanCup_DM2024\PilotMapping.csv"))
    //    {
    //        Logger.LogError("Failed to map pilot names to tracks");
    //        return;
    //    }
    //    if (!Flight.ParseTrackFiles(@"C:\TEMP\GermanCup_DM2024\Flight3_28_09_AM", true))
    //    {
    //        Logger.LogError("Failed to parse track files");
    //        return;
    //    }
    //    Flight.Tracks = Flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
    //    foreach (Track track in Flight.Tracks)
    //    {
    //        //Check ILP to all Goal > 1000m
    //        if (TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
    //        {
    //            double distanceILPToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T7_FIN);
    //            Logger.LogInformation("Distance from ILP to goal 1 for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal1}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal1, (distanceILPToGoal1 >= 1000 ? "OK" : "INVALID"));
    //            double distanceILPToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T8_JDG);
    //            Logger.LogInformation("Distance from ILP to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal2}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal2, (distanceILPToGoal2 >= 1000 ? "OK" : "INVALID"));
    //            double distanceILPToGoal3_A = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T9_HWZ_A);
    //            Logger.LogInformation("Distance from ILP to goal 3 A for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal3}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal3_A, (distanceILPToGoal3_A >= 1000 ? "OK" : "INVALID"));
    //            double distanceILPToGoal3_B = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T9_HWZ_B);
    //            Logger.LogInformation("Distance from ILP to goal 3 B for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal3}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal3_B, (distanceILPToGoal3_B >= 1000 ? "OK" : "INVALID"));
    //            //Check Launch between 09:45 local time and 10:30 local time
    //            DateTime startPeriodBegin = new DateTime(2024, 9, 28, 9, 45, 0);
    //            DateTime startPeriodEnd = new DateTime(2024, 9, 28, 10, 30, 0);
    //            Logger.LogInformation("Launch at {launchTime} for pilot {track.Pilot.PilotNumber}: {launchValid}",launchPoint.TimeStamp.ToString("HH:mm:ss"), track.Pilot.PilotNumber, (launchPoint.TimeStamp>=startPeriodBegin&&launchPoint.TimeStamp<=startPeriodEnd? "OK" : "INVALID"));
    //        }
    //        else
    //        {
    //            Logger.LogWarning("Failed to estimate launch and landing point for pilot {track.Pilot.PilotNumber}. Skipping distance checks", track.Pilot.PilotNumber);
    //        }

    //        DateTime scoringPeriodEnd= new DateTime(2024, 9, 28, 11, 30, 0);
    //        foreach (MarkerDrop marker in track.MarkerDrops.OrderBy(x=>x.MarkerNumber))
    //        {
    //            Logger.LogInformation("Marker {markerNumber} at {markerTime} for pilot {track.Pilot.PilotNumber}: {markerValid}", marker.MarkerNumber, marker.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), track.Pilot.PilotNumber, (marker.MarkerLocation.TimeStamp <= scoringPeriodEnd ? "OK" : "INVALID"));
    //        }
    //            Logger.LogInformation("Marker order for pilot {track.Pilot.PilotNumber}: {markerOrderValid}", track.Pilot.PilotNumber, (track.MarkerDrops.OrderBy(x => x.MarkerLocation.TimeStamp).Select(x => x.MarkerNumber).SequenceEqual(new int[] { 1, 2, 3 }) ? "OK" : "INVALID"));
    //    }
    //    foreach (Track track in Flight.Tracks)
    //    {
    //        //Calculate Result for Task 7 using Marker 1 and Separation Altitude
    //        double distanceMarker1ToFIN_T7 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T7_FIN, track.MarkerDrops.First(x => x.MarkerNumber == 1).MarkerLocation, SeparationAltitude, true);
    //        Logger.LogInformation("Task 7 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker1ToFIN_T7, 0, MidpointRounding.AwayFromZero));
    //    }
    //    foreach (Track track in Flight.Tracks)
    //    {
    //        //Calculate Result for Task 8 using Marker 2 and Separation Altitude
    //        double distanceMarker2ToJDG_T8 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T8_JDG, track.MarkerDrops.First(x => x.MarkerNumber == 2).MarkerLocation, SeparationAltitude, true);
    //        Logger.LogInformation("Task 8 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker2ToJDG_T8, 0, MidpointRounding.AwayFromZero));
    //    }
    //    foreach (Track track in Flight.Tracks)
    //    {
    //        //Calculate Result for Task 9 using Marker 3 and Separation Altitude
    //        double distanceMarker3ToHWZ_A_T9 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T9_HWZ_A, track.MarkerDrops.First(x => x.MarkerNumber == 3).MarkerLocation, SeparationAltitude, true);
    //        double distanceMarker3ToHWZ_B_T9 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T9_HWZ_B, track.MarkerDrops.First(x => x.MarkerNumber == 3).MarkerLocation, SeparationAltitude, true);
    //        Logger.LogInformation("Task 9 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(Math.Min(distanceMarker3ToHWZ_A_T9, distanceMarker3ToHWZ_B_T9), 0, MidpointRounding.AwayFromZero));
    //    }
    //}

    internal void ChecksFlight3()
    {
        Flight.FlightNumber = 3;
        Flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(2000));
        if (!Flight.MapPilotNamesToTracks(@"C:\TEMP\GermanCup_DM2024\PilotMapping.csv"))
        {
            Logger.LogError("Failed to map pilot names to tracks");
            return;
        }
        if (!Flight.ParseTrackFiles(@"C:\TEMP\GermanCup_DM2024\Flight3_28_09_PM", true))
        {
            Logger.LogError("Failed to parse track files");
            return;
        }
        Flight.Tracks = Flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        foreach (var track in Flight.Tracks)
        {
            Declaration declaration = track.GetLatestDeclaration(1);
            if (TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
            {
                double distanceILPToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T7_FIN);
                Logger.LogInformation("Distance from ILP to goal 1 for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal1}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal1, (distanceILPToGoal1 >= 1000 ? "OK" : "INVALID"));
                double distanceILPToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
                Logger.LogInformation("Distance from ILP to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceILPToGoal2}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceILPToGoal2, (distanceILPToGoal2 >= 1000 ? "OK" : "INVALID"));
                //Check Launch between 09:45 local time and 10:30 local time
                DateTime startPeriodBegin = new DateTime(2024, 9, 28, 15, 45, 0);
                DateTime startPeriodEnd = new DateTime(2024, 9, 28, 16, 30, 0);
                Logger.LogInformation("Launch at {launchTime} for pilot {track.Pilot.PilotNumber}: {launchValid}", launchPoint.TimeStamp.ToString("HH:mm:ss"), track.Pilot.PilotNumber, (launchPoint.TimeStamp >= startPeriodBegin && launchPoint.TimeStamp <= startPeriodEnd ? "OK" : "INVALID"));
            }
            else
            {
                Logger.LogWarning("Failed to estimate launch and landing point for pilot {track.Pilot.PilotNumber}. Skipping distance checks", track.Pilot.PilotNumber);
            }
            DateTime scoringPeriodEnd = new DateTime(2024, 9, 28, 17, 00, 0);
            foreach (MarkerDrop marker in track.MarkerDrops.OrderBy(x => x.MarkerNumber))
            {
                Logger.LogInformation("Marker {markerNumber} at {markerTime} for pilot {track.Pilot.PilotNumber}: {markerValid}", marker.MarkerNumber, marker.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), track.Pilot.PilotNumber, (marker.MarkerLocation.TimeStamp <= scoringPeriodEnd ? "OK" : "INVALID"));
            }
            Logger.LogInformation("Marker order for pilot {track.Pilot.PilotNumber}: {markerOrderValid}", track.Pilot.PilotNumber, (track.MarkerDrops.OrderBy(x => x.MarkerLocation.TimeStamp).Select(x => x.MarkerNumber).SequenceEqual(new int[] { 1, 3 }) ? "OK" : "INVALID"));

            double distanceDeclarationToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, declaration.PositionAtDeclaration);
            Logger.LogInformation("Distance from declaration to goal for pilot {track.Pilot.PilotNumber}: {distanceDeclartionToGoal}\t->\t{declarationValid}", track.Pilot.PilotNumber, distanceDeclarationToGoal, (distanceDeclarationToGoal >= 1000 ? "OK" : "INVALID"));

            double distanceGoal1ToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, T7_FIN);
            Logger.LogInformation("Distance from goal 1 to goal 2 for pilot {track.Pilot.PilotNumber}: {distanceGoal1ToGoal2}\t->\t{distanceValid}", track.Pilot.PilotNumber, distanceGoal1ToGoal2, (distanceGoal1ToGoal2 >= 1000 ? "OK" : "INVALID"));
        }
        foreach (var track in Flight.Tracks)
        {
            //Calculate Result for Task 7 using Marker 1 and Separation Altitude
            if (track.MarkerDrops.Where(x => x.MarkerNumber == 1).Count() == 0)
            {
                Logger.LogWarning("No marker 1 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
                continue;
            }
            double distanceMarker1ToFIN_T7 = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T7_FIN, track.MarkerDrops.First(x => x.MarkerNumber == 1).MarkerLocation, SeparationAltitude, true);
            Logger.LogInformation("Task 7 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker1ToFIN_T7, 0, MidpointRounding.AwayFromZero));
        }
        foreach (var track in Flight.Tracks)
        {
            if (track.MarkerDrops.Where(x => x.MarkerNumber == 3).Count() == 0)
            {
                Logger.LogWarning("No marker 3 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
                continue;
            }
            //Calculate Result for Task 9 using Marker 2 using 3D Distance
            double distanceMarker2ToGoal2 = CoordinateHelpers.Calculate3DDistance(track.GetLatestDeclaration(1).DeclaredGoal, track.MarkerDrops.First(x => x.MarkerNumber == 3).MarkerLocation, true);
            Logger.LogInformation("Task 9 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker2ToGoal2, 0, MidpointRounding.AwayFromZero));
        }

    }


    internal void ChecksFlight4()
    {
        Flight.FlightNumber = 4;
        if (!Flight.MapPilotNamesToTracks(@"C:\TEMP\GermanCup_DM2024\PilotMapping.csv"))
        {
            Logger.LogError("Failed to map pilot names to tracks");
            return;
        }
        if (!Flight.ParseTrackFiles(@"C:\TEMP\GermanCup_DM2024\Flight4_29_09_AM", true))
        {
            Logger.LogError("Failed to parse track files");
            return;
        }
        Flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(2000));
        Flight.Tracks = Flight.Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        HesitationWaltzTask t10_FIN_Task = new HesitationWaltzTask();
        t10_FIN_Task.SetupHWZ(10, [T10_FIN], 1, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.LatestValid);
        t10_FIN_Task.SeparationAltitude = SeparationAltitude;
        DonutTask t11_3DT_Task = new DonutTask();
        t11_3DT_Task.SetupDonut(11, 1, 1, 1500, 3000, double.NaN, double.NaN, true
            , null, Competition.Validation.ValidationStrictnessType.LatestValid);
        HesitationWaltzTask t12_HWZ_Task = new HesitationWaltzTask();
        t12_HWZ_Task.SetupHWZ(12, [T12_HWZ_A, T12_HWZ_B, T12_HWZ_C], 2, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.LatestValid);
        t12_HWZ_Task.SeparationAltitude = SeparationAltitude;
        Flight.Tasks.Add(t10_FIN_Task);
        Flight.Tasks.Add(t11_3DT_Task);
        Flight.Tasks.Add(t12_HWZ_Task);
        ////ILP checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    Declaration declaration1 = track.GetLatestDeclaration(1);
        //    Declaration declaration2 = track.GetLatestDeclaration(2);
        //    Declaration declaration3 = track.GetLatestDeclaration(3);
        //    Declaration declaration4 = track.GetLatestDeclaration(4);
        //    if (TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _))
        //    {
        //        double distanceILPToT10_FIN = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T10_FIN);
        //        bool distanceILPToT10_FIN_Valid = distanceILPToT10_FIN >= 1000;
        //        LogCheckResultsWithColor(distanceILPToT10_FIN_Valid, "Pilot {pilotNumber}: Distance between ILP and T10 FIN: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToT10_FIN, distanceILPToT10_FIN_Valid ? "OK" : "INVALID");
        //        if (declaration1 is not null)
        //        {
        //            double distanceILPToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration1.DeclaredGoal);
        //            bool distanceILPToGoal1_Valid = distanceILPToGoal1 >= 1000;
        //            LogCheckResultsWithColor(distanceILPToGoal1_Valid, "Pilot {pilotNumber}: Distance between ILP and Goal 1: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToGoal1, distanceILPToGoal1_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }

        //        double distanceILPToT12_HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T12_HWZ_A);
        //        bool distanceILPToT12_HWZ_A_Valid = distanceILPToT12_HWZ_A >= 1000;
        //        LogCheckResultsWithColor(distanceILPToT12_HWZ_A_Valid, "Pilot {pilotNumber}: Distance between ILP and T12 HWZ A: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToT12_HWZ_A, distanceILPToT12_HWZ_A_Valid ? "OK" : "INVALID");
        //        double distanceILPToT12_HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T12_HWZ_B);
        //        bool distanceILPToT12_HWZ_B_Valid = distanceILPToT12_HWZ_B >= 1000;
        //        LogCheckResultsWithColor(distanceILPToT12_HWZ_B_Valid, "Pilot {pilotNumber}: Distance between ILP and T12 HWZ B: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToT12_HWZ_B, distanceILPToT12_HWZ_B_Valid ? "OK" : "INVALID");
        //        double distanceILPToT12_HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, T12_HWZ_C);
        //        bool distanceILPToT12_HWZ_C_Valid = distanceILPToT12_HWZ_C >= 1000;
        //        LogCheckResultsWithColor(distanceILPToT12_HWZ_C_Valid, "Pilot {pilotNumber}: Distance between ILP and T12 HWZ C: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToT12_HWZ_C, distanceILPToT12_HWZ_C_Valid ? "OK" : "INVALID");
        //        if (declaration2 is not null)
        //        {
        //            double distanceILPToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration2.DeclaredGoal);
        //            bool distanceILPToGoal2_Valid = distanceILPToGoal2 >= 1000;
        //            LogCheckResultsWithColor(distanceILPToGoal2_Valid, "Pilot {pilotNumber}: Distance between ILP and Goal 2: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToGoal2, distanceILPToGoal2_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 2 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }
        //        if (declaration3 is not null)
        //        {
        //            double distanceILPToGoal3 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration3.DeclaredGoal);
        //            bool distanceILPToGoal3_Valid = distanceILPToGoal3 >= 1000;
        //            LogCheckResultsWithColor(distanceILPToGoal3_Valid, "Pilot {pilotNumber}: Distance between ILP and Goal 3: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToGoal3, distanceILPToGoal3_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 3 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }
        //        if (declaration4 is not null)
        //        {
        //            double distanceILPToGoal4 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration4.DeclaredGoal);
        //            bool distanceILPToGoal4_Valid = distanceILPToGoal4 >= 1000;
        //            LogCheckResultsWithColor(distanceILPToGoal4_Valid, "Pilot {pilotNumber}: Distance between ILP and Goal 4: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceILPToGoal4, distanceILPToGoal4_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 4 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }
        //        DateTime startPerionBegin = new DateTime(2024, 9, 29, 5, 20, 0);
        //        DateTime startPerionEnd = new DateTime(2024, 9, 29, 6, 30, 0);
        //        bool launchTimeValid = launchPoint.TimeStamp >= startPerionBegin && launchPoint.TimeStamp <= startPerionEnd;
        //        LogCheckResultsWithColor(launchTimeValid, "Pilot {pilotNumber}: Launch at {launchTime} \t -> \t {valid}", track.Pilot.PilotNumber, launchPoint.TimeStamp.ToString("HH:mm:ss"), launchTimeValid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("Failed to estimate launch and landing point for pilot {track.Pilot.PilotNumber}. Skipping ILP checks", track.Pilot.PilotNumber);
        //    }

        //    bool markerOrderValid = track.MarkerDrops.OrderBy(x => x.MarkerLocation.TimeStamp).Select(x => x.MarkerNumber).SequenceEqual(new int[] { 1, 2, 3, 4, 5 });
        //    LogCheckResultsWithColor(markerOrderValid, "Pilot {pilotNumber}: Marker order \t -> \t {valid}", track.Pilot.PilotNumber, markerOrderValid ? "OK" : "INVALID");
        //}
        ////Task 10 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    DateTime scoringPeriodEnd = new DateTime(2024, 9, 29, 7, 0, 0);
        //    MarkerDrop marker1 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
        //    if (marker1 is not null)
        //    {
        //        bool marker1Valid = marker1.MarkerLocation.TimeStamp <= scoringPeriodEnd;
        //        LogCheckResultsWithColor(marker1Valid, "Pilot {pilotNumber}: Marker 1 at {markerTime} \t -> \t {valid}", track.Pilot.PilotNumber, marker1.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), marker1Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 1 found for pilot {track.Pilot.PilotNumber}. Skipping timing calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 11 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    Declaration declaration1 = track.GetLatestDeclaration(1);
        //    if (declaration1 is not null)
        //    {
        //        bool declaration1_valid = declaration1.OrignalEastingDeclarationUTM == 8200 || declaration1.OrignalEastingDeclarationUTM == 82000;
        //        LogCheckResultsWithColor(declaration1_valid, "Pilot {pilotNumber}: Declaration 1 declared eating UTM {eastingUTM} \t -> \t {valid}", track.Pilot.PilotNumber, declaration1.OrignalEastingDeclarationUTM, declaration1_valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No declaration found for pilot {track.Pilot.PilotNumber}. Skipping declaration checks", track.Pilot.PilotNumber);
        //    }

        //}

        ////Task 12 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    DateTime scoringPeriodEnd = new DateTime(2024, 9, 29, 8, 0, 0);
        //    MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
        //    if (marker2 is not null)
        //    {
        //        bool marker2Valid = marker2.MarkerLocation.TimeStamp <= scoringPeriodEnd;
        //        LogCheckResultsWithColor(marker2Valid, "Pilot {pilotNumber}: Marker 2 at {markerTime} \t -> \t {valid}", track.Pilot.PilotNumber, marker2.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), marker2Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 2 found for pilot {track.Pilot.PilotNumber}. Skipping timing calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 13 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    Declaration declaration1 = track.GetLatestDeclaration(1);
        //    Declaration declaration2 = track.GetLatestDeclaration(2);
        //    Declaration declaration3 = track.GetLatestDeclaration(3);
        //    Declaration declaration4 = track.GetLatestDeclaration(4);
        //    if (declaration2 is not null)
        //    {
        //        double distanceGoal2ToT10_FIN = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, T10_FIN);
        //        bool distanceGoal2ToT10_FIN_Valid = distanceGoal2ToT10_FIN >= 1000;
        //        LogCheckResultsWithColor(distanceGoal2ToT10_FIN_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and T10 FIN: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToT10_FIN, distanceGoal2ToT10_FIN_Valid ? "OK" : "INVALID");

        //        if (declaration1 is not null)
        //        {
        //            double distanceGoal2ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, declaration1.DeclaredGoal);
        //            bool distanceGoal2ToGoal1_Valid = distanceGoal2ToGoal1 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal2ToGoal1_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and Goal 1: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToGoal1, distanceGoal2ToGoal1_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }

        //        double distanceGoal2ToT12_HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, T12_HWZ_A);
        //        bool distanceGoal2ToT12_HWZ_A_Valid = distanceGoal2ToT12_HWZ_A >= 1000;
        //        LogCheckResultsWithColor(distanceGoal2ToT12_HWZ_A_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and T12 HWZ A: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToT12_HWZ_A, distanceGoal2ToT12_HWZ_A_Valid ? "OK" : "INVALID");
        //        double distanceGoal2ToT12_HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, T12_HWZ_B);
        //        bool distanceGoal2ToT12_HWZ_B_Valid = distanceGoal2ToT12_HWZ_B >= 1000;
        //        LogCheckResultsWithColor(distanceGoal2ToT12_HWZ_B_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and T12 HWZ B: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToT12_HWZ_B, distanceGoal2ToT12_HWZ_B_Valid ? "OK" : "INVALID");
        //        double distanceGoal2ToT12_HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, T12_HWZ_C);
        //        bool distanceGoal2ToT12_HWZ_C_Valid = distanceGoal2ToT12_HWZ_C >= 1000;
        //        LogCheckResultsWithColor(distanceGoal2ToT12_HWZ_C_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and T12 HWZ C: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToT12_HWZ_C, distanceGoal2ToT12_HWZ_C_Valid ? "OK" : "INVALID");

        //        if (declaration3 is not null)
        //        {
        //            double distanceGoal2ToGoal3 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, declaration3.DeclaredGoal);
        //            bool distanceGoal2ToGoal3_Valid = distanceGoal2ToGoal3 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal2ToGoal3_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and Goal 3: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToGoal3, distanceGoal2ToGoal3_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 3 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }

        //        if (declaration4 is not null)
        //        {
        //            double distanceGoal2ToGoal4 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.DeclaredGoal, declaration4.DeclaredGoal);
        //            bool distanceGoal2ToGoal4_Valid = distanceGoal2ToGoal4 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal2ToGoal4_Valid, "Pilot {pilotNumber}: Distance between Goal 2 and Goal 4: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal2ToGoal4, distanceGoal2ToGoal4_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 4 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }



        //        double distanceDeclaration2ToGoal2 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration2.PositionAtDeclaration, declaration2.DeclaredGoal);
        //        bool distanceDeclaration2ToGoal2_Valid = distanceDeclaration2ToGoal2 >= 1000 && distanceDeclaration2ToGoal2 <= 3000;
        //        LogCheckResultsWithColor(distanceDeclaration2ToGoal2_Valid, "Pilot {pilotNumber}: Distance between declaration 2 and goal 2: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceDeclaration2ToGoal2, distanceDeclaration2ToGoal2_Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No declaration found for goal 2 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //    }

        //    DateTime scoringPeriodEnd = new DateTime(2024, 9, 29, 8, 30, 0);
        //    MarkerDrop marker3 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
        //    if (marker3 is not null)
        //    {
        //        bool marker3Valid = marker3.MarkerLocation.TimeStamp <= scoringPeriodEnd;
        //        LogCheckResultsWithColor(marker3Valid, "Pilot {pilotNumber}: Marker 3 at {markerTime} \t -> \t {valid}", track.Pilot.PilotNumber, marker3.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), marker3Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 3 found for pilot {track.Pilot.PilotNumber}. Skipping timing calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 14 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    Declaration declaration1 = track.GetLatestDeclaration(1);
        //    Declaration declaration2 = track.GetLatestDeclaration(2);
        //    Declaration declaration3 = track.GetLatestDeclaration(3);
        //    Declaration declaration4 = track.GetLatestDeclaration(4);
        //    if (declaration3 is not null)
        //    {
        //        double distanceGoal3ToT10_FIN = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, T10_FIN);
        //        bool distanceGoal3ToT10_FIN_Valid = distanceGoal3ToT10_FIN >= 1000;
        //        LogCheckResultsWithColor(distanceGoal3ToT10_FIN_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and T10 FIN: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToT10_FIN, distanceGoal3ToT10_FIN_Valid ? "OK" : "INVALID");

        //        if (declaration1 is not null)
        //        {
        //            double distanceGoal3ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, declaration1.DeclaredGoal);
        //            bool distanceGoal3ToGoal1_Valid = distanceGoal3ToGoal1 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal3ToGoal1_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and Goal 1: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToGoal1, distanceGoal3ToGoal1_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }

        //        double distanceGoal3ToT12_HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, T12_HWZ_A);
        //        bool distanceGoal3ToT12_HWZ_A_Valid = distanceGoal3ToT12_HWZ_A >= 1000;
        //        LogCheckResultsWithColor(distanceGoal3ToT12_HWZ_A_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and T12 HWZ A: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToT12_HWZ_A, distanceGoal3ToT12_HWZ_A_Valid ? "OK" : "INVALID");
        //        double distanceGoal3ToT12_HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, T12_HWZ_B);
        //        bool distanceGoal3ToT12_HWZ_B_Valid = distanceGoal3ToT12_HWZ_B >= 1000;
        //        LogCheckResultsWithColor(distanceGoal3ToT12_HWZ_B_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and T12 HWZ B: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToT12_HWZ_B, distanceGoal3ToT12_HWZ_B_Valid ? "OK" : "INVALID");
        //        double distanceGoal3ToT12_HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, T12_HWZ_C);
        //        bool distanceGoal3ToT12_HWZ_C_Valid = distanceGoal3ToT12_HWZ_C >= 1000;
        //        LogCheckResultsWithColor(distanceGoal3ToT12_HWZ_C_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and T12 HWZ C: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToT12_HWZ_C, distanceGoal3ToT12_HWZ_C_Valid ? "OK" : "INVALID");


        //        if (declaration4 is not null)
        //        {
        //            double distanceGoal3ToGoal4 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.DeclaredGoal, declaration4.DeclaredGoal);
        //            bool distanceGoal3ToGoal4_Valid = distanceGoal3ToGoal4 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal3ToGoal4_Valid, "Pilot {pilotNumber}: Distance between Goal 3 and Goal 4: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal3ToGoal4, distanceGoal3ToGoal4_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 4 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }



        //        double distanceDeclaration3ToGoal3 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration3.PositionAtDeclaration, declaration3.DeclaredGoal);
        //        bool distanceDeclaration3ToGoal3_Valid = distanceDeclaration3ToGoal3 >= 1000 && distanceDeclaration3ToGoal3 <= 3000;
        //        LogCheckResultsWithColor(distanceDeclaration3ToGoal3_Valid, "Pilot {pilotNumber}: Distance between declaration 3 and goal 3: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceDeclaration3ToGoal3, distanceDeclaration3ToGoal3_Valid ? "OK" : "INVALID");

        //        double heightDifferenceDeclaration3ToGoal3 = Math.Abs(declaration3.DeclaredGoal.AltitudeGPS - declaration3.PositionAtDeclaration.AltitudeGPS);
        //        bool heightDifferenceDeclaration3ToGoal3_Valid = heightDifferenceDeclaration3ToGoal3 >= CoordinateHelpers.ConvertToMeter(500);
        //        LogCheckResultsWithColor(heightDifferenceDeclaration3ToGoal3_Valid, "Pilot {pilotNumber}: Height difference between declaration 3 and goal 3: {height} \t -> \t {valid}", track.Pilot.PilotNumber, heightDifferenceDeclaration3ToGoal3, heightDifferenceDeclaration3ToGoal3_Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No declaration found for goal 3 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //    }
        //    DateTime scoringPeriodEnd = new DateTime(2024, 9, 29, 8, 30, 0);
        //    MarkerDrop marker4 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 4);
        //    if (marker4 is not null)
        //    {
        //        bool marker4Valid = marker4.MarkerLocation.TimeStamp <= scoringPeriodEnd;
        //        LogCheckResultsWithColor(marker4Valid, "Pilot {pilotNumber}: Marker 4 at {markerTime} \t -> \t {valid}", track.Pilot.PilotNumber, marker4.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), marker4Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 4 found for pilot {track.Pilot.PilotNumber}. Skipping timing calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 15 Checks
        //foreach (Track track in Flight.Tracks)
        //{
        //    Declaration declaration1 = track.GetLatestDeclaration(1);
        //    Declaration declaration2 = track.GetLatestDeclaration(2);
        //    Declaration declaration3 = track.GetLatestDeclaration(3);
        //    Declaration declaration4 = track.GetLatestDeclaration(4);
        //    if (declaration4 is not null)
        //    {
        //        double distanceGoal4ToT10_FIN = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.DeclaredGoal, T10_FIN);
        //        bool distanceGoal4ToT10_FIN_Valid = distanceGoal4ToT10_FIN >= 1000;
        //        LogCheckResultsWithColor(distanceGoal4ToT10_FIN_Valid, "Pilot {pilotNumber}: Distance between  Goal 4 and T10 FIN: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal4ToT10_FIN, distanceGoal4ToT10_FIN_Valid ? "OK" : "INVALID");

        //        if (declaration1 is not null)
        //        {
        //            double distanceGoal4ToGoal1 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.DeclaredGoal, declaration1.DeclaredGoal);
        //            bool distanceGoal4ToGoal1_Valid = distanceGoal4ToGoal1 >= 1000;
        //            LogCheckResultsWithColor(distanceGoal4ToGoal1_Valid, "Pilot {pilotNumber}: Distance between  Goal 4 and Goal 1: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal4ToGoal1, distanceGoal4ToGoal1_Valid ? "OK" : "INVALID");
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No declaration found for goal 1 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //        }

        //        double distanceGoal4ToT12_HWZ_A = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.DeclaredGoal, T12_HWZ_A);
        //        bool distanceGoal4ToT12_HWZ_A_Valid = distanceGoal4ToT12_HWZ_A >= 1000;
        //        LogCheckResultsWithColor(distanceGoal4ToT12_HWZ_A_Valid, "Pilot {pilotNumber}: Distance between  Goal 4 and T12 HWZ A: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal4ToT12_HWZ_A, distanceGoal4ToT12_HWZ_A_Valid ? "OK" : "INVALID");
        //        double distanceGoal4ToT12_HWZ_B = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.DeclaredGoal, T12_HWZ_B);
        //        bool distanceGoal4ToT12_HWZ_B_Valid = distanceGoal4ToT12_HWZ_B >= 1000;
        //        LogCheckResultsWithColor(distanceGoal4ToT12_HWZ_B_Valid, "Pilot {pilotNumber}: Distance between  Goal 4 and T12 HWZ B: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal4ToT12_HWZ_B, distanceGoal4ToT12_HWZ_B_Valid ? "OK" : "INVALID");
        //        double distanceGoal4ToT12_HWZ_C = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.DeclaredGoal, T12_HWZ_C);
        //        bool distanceGoal4ToT12_HWZ_C_Valid = distanceGoal4ToT12_HWZ_C >= 1000;
        //        LogCheckResultsWithColor(distanceGoal4ToT12_HWZ_C_Valid, "Pilot {pilotNumber}: Distance between  Goal 4 and T12 HWZ C: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceGoal4ToT12_HWZ_C, distanceGoal4ToT12_HWZ_C_Valid ? "OK" : "INVALID");

        //        double distanceDeclaration4ToGoal4 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration4.PositionAtDeclaration, declaration4.DeclaredGoal);
        //        bool distanceDeclaration4ToGoal4_Valid = distanceDeclaration4ToGoal4 >= 1000 && distanceDeclaration4ToGoal4 <= 3000;
        //        LogCheckResultsWithColor(distanceDeclaration4ToGoal4_Valid, "Pilot {pilotNumber}: Distance between declaration 4 and goal 4: {distance} \t -> \t {valid}", track.Pilot.PilotNumber, distanceDeclaration4ToGoal4, distanceDeclaration4ToGoal4_Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No declaration found for goal 4 for pilot {track.Pilot.PilotNumber}", track.Pilot.PilotNumber);
        //    }
        //    DateTime scoringPeriodEnd = new DateTime(2024, 9, 29, 9, 0, 0);
        //    MarkerDrop marker5 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 5);
        //    if (marker5 is not null)
        //    {
        //        bool marker5Valid = marker5.MarkerLocation.TimeStamp <= scoringPeriodEnd;
        //        LogCheckResultsWithColor(marker5Valid, "Pilot {pilotNumber}: Marker 5 at {markerTime} \t -> \t {valid}", track.Pilot.PilotNumber, marker5.MarkerLocation.TimeStamp.ToString("HH:mm:ss"), marker5Valid ? "OK" : "INVALID");
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 5 found for pilot {track.Pilot.PilotNumber}. Skipping timing calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 10 results
        //foreach (Track track in Flight.Tracks)
        //{
        //    MarkerDrop marker1 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
        //    if (marker1 is not null)
        //    {
        //        //Calculate Result for Task 10 using Marker 1 and Separation Altitude
        //        double distanceMarker1ToT10_FIN = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T10_FIN, marker1.MarkerLocation, SeparationAltitude, true);
        //        Logger.LogInformation("Task 10 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker1ToT10_FIN, 0, MidpointRounding.AwayFromZero));
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 1 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
        //    }
        //}

        //Task 12 results
        foreach (Track track in Flight.Tracks)
        {
            MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            if (marker2 is not null)
            {
                //Calculate Result for Task 12 using Marker 2 and Separation Altitude
                double distanceMarker2ToT12_HWZ_A = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T12_HWZ_A, marker2.MarkerLocation, SeparationAltitude, true);
                double distanceMarker2ToT12_HWZ_B = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T12_HWZ_B, marker2.MarkerLocation, SeparationAltitude, true);
                double distanceMarker2ToT12_HWZ_C = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(T12_HWZ_C, marker2.MarkerLocation, SeparationAltitude, true);
                Logger.LogInformation("Task 12 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(Math.Min(Math.Min(distanceMarker2ToT12_HWZ_A, distanceMarker2ToT12_HWZ_B), distanceMarker2ToT12_HWZ_C), 0, MidpointRounding.AwayFromZero));
            }
            else
            {
                Logger.LogWarning("No marker 2 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
            }
        }

        ////Task 13 results
        //foreach (Track track in Flight.Tracks)
        //{
        //    MarkerDrop marker3 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
        //    Declaration declaration2 = track.GetLatestDeclaration(2);
        //    if (marker3 is not null && declaration2 is not null)
        //    {
        //        //Calculate Result for Task 13 using Marker 3 and Separation Altitude
        //        double distanceMarker3ToGoal2 = CoordinateHelpers.Calculate3DDistance(declaration2.DeclaredGoal, marker3.MarkerLocation, true);
        //        Logger.LogInformation("Task 13 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker3ToGoal2, 0, MidpointRounding.AwayFromZero));
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 3 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 14 results
        //foreach (Track track in Flight.Tracks)
        //{
        //    MarkerDrop marker4 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 4);
        //    Declaration declaration3 = track.GetLatestDeclaration(3);
        //    if (marker4 is not null && declaration3 is not null)
        //    {
        //        //Calculate Result for Task 14 using Marker 4 and Separation Altitude
        //        double distanceMarker4ToGoal3 = CoordinateHelpers.Calculate3DDistance(declaration3.DeclaredGoal, marker4.MarkerLocation, true);
        //        Logger.LogInformation("Task 14 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker4ToGoal3, 0, MidpointRounding.AwayFromZero));
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 4 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
        //    }
        //}

        ////Task 15 results
        //foreach (Track track in Flight.Tracks)
        //{
        //    MarkerDrop marker5 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 5);
        //    Declaration declaration4 = track.GetLatestDeclaration(4);
        //    if (marker5 is not null && declaration4 is not null)
        //    {
        //        //Calculate Result for Task 15 using Marker 5 and Separation Altitude
        //        double distanceMarker5ToGoal4 = CoordinateHelpers.Calculate3DDistance(declaration4.DeclaredGoal, marker5.MarkerLocation, true);
        //        Logger.LogInformation("Task 15 {PilotNumber}: {result}[m]", track.Pilot.PilotNumber, Math.Round(distanceMarker5ToGoal4, 0, MidpointRounding.AwayFromZero));
        //    }
        //    else
        //    {
        //        Logger.LogWarning("No marker 5 found for pilot {track.Pilot.PilotNumber}. Skipping distance calculation", track.Pilot.PilotNumber);
        //    }
        //}

        //Flight.CalculateResults(true, @"C:\TEMP\GermanCup_DM2024\Flight4_29_09_AM");
    }

    private void LogCheckResultsWithColor(bool valid, string messageTemplate, params object?[] args)
    {
        ConsoleColor temp = Console.ForegroundColor;
        Console.ForegroundColor = valid ? ConsoleColor.Green : ConsoleColor.Red;
        if (valid)
        {
            Logger.LogInformation(messageTemplate, args);
        }
        else
        {
            Logger.LogWarning(messageTemplate, args);
        }
        Console.ForegroundColor = temp;
    }
}
