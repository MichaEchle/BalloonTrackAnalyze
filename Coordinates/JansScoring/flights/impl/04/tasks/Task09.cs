using Coordinates;
using JansScoring.check;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._04.tasks;

public class Task09 : TaskFON_Dec_from_GoalList
{
    public Task09(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 9;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        List<Declaration> declarations =
            track.Declarations.FindAll(declaration => declaration.GoalNumber == DeclarationNumber());

        if (declarations.Count == 0)
        {
            comment += $"Pilot has no declaration in goal number {DeclarationNumber()} | ";
            return true;
        }

        Declaration declaration = track.GetLatestDeclaration(DeclarationNumber());
        if (declaration == null)
        {
            comment += $"No declaration found in goal number {DeclarationNumber()}. | ";
            return true;
        }


        declaration.DeclaredGoal.AltitudeBarometric = CoordinateHelpers.ConvertToMeter(3000);

        DeclarationChecks.CheckDistanceFromDeclarationPointToDelcaredGoal(Flight, declaration, 4000, ref comment);

        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchpoint, out _);
        int declarationsAfterEstimated = 0;
        Declaration fistDeclarationInAir = null;
        declarations.ForEach(declaration =>
        {
            if (declaration.PositionAtDeclaration.TimeStamp > launchpoint.TimeStamp)
            {
                if (fistDeclarationInAir == null)
                    fistDeclarationInAir = declaration;
                declarationsAfterEstimated++;
            }
        });
        if (declarationsAfterEstimated > 1)
        {
            comment += "Pilot has maybe more than one declaration in the air | ";
            track.Declarations.RemoveAll(declaration1 =>
                declaration1.PositionAtDeclaration.TimeStamp > launchpoint.TimeStamp &&
                declaration1 != fistDeclarationInAir);
        }

        DeclarationChecks.CheckDistanceFromDelcaredGoalToAllGoals(Flight, declaration, 500, ref comment);

        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 16, 18, 59, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 4;
    }

    public override Coordinate Goal(Track track, Declaration declaration, int pilot)
    {
        if (declaration.OrignalEastingDeclarationUTM == 0 && declaration.OrignalNorhtingDeclarationUTM == 0)
        {
            return null;
        }

        if (declaration.OrignalEastingDeclarationUTM == 0 && declaration.OrignalNorhtingDeclarationUTM != 0)
        {
            return goals[declaration.OrignalNorhtingDeclarationUTM.ToString()];
        }

        if (declaration.OrignalEastingDeclarationUTM != 0 && declaration.OrignalNorhtingDeclarationUTM == 0)
        {
            return goals[declaration.OrignalEastingDeclarationUTM.ToString()];
        }

        return null;
    }

    private static Dictionary<string, Coordinate> goals = new()
    {
        {
            "200", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625471, 230084,
                CoordinateHelpers.ConvertToMeter(1535))
        },
        {
            "201", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623914, 232245,
                CoordinateHelpers.ConvertToMeter(1421))
        },
        {
            "202", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627751, 230416,
                CoordinateHelpers.ConvertToMeter(1526))
        },
        {
            "203", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624620, 236228,
                CoordinateHelpers.ConvertToMeter(1634))
        },
        {
            "204", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624346, 237750,
                CoordinateHelpers.ConvertToMeter(1437))
        },
        {
            "205", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627327, 238853,
                CoordinateHelpers.ConvertToMeter(1440))
        },
        {
            "206", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621700, 234549,
                CoordinateHelpers.ConvertToMeter(1483))
        },
        {
            "207", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623011, 230196,
                CoordinateHelpers.ConvertToMeter(1667))
        },
        {
            "208", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622038, 230114,
                CoordinateHelpers.ConvertToMeter(1509))
        },
        {
            "209", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623233, 231286,
                CoordinateHelpers.ConvertToMeter(1453))
        },
        {
            "210", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624919, 231515,
                CoordinateHelpers.ConvertToMeter(1552))
        },
        {
            "211", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625772, 230029,
                CoordinateHelpers.ConvertToMeter(1552))
        },
        {
            "300", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634364, 230506,
                CoordinateHelpers.ConvertToMeter(1729))
        },
        {
            "301", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634907, 230109,
                CoordinateHelpers.ConvertToMeter(1742))
        },
        {
            "302", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 637290, 233126,
                CoordinateHelpers.ConvertToMeter(1617))
        },
        {
            "303", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634701, 235834,
                CoordinateHelpers.ConvertToMeter(1417))
        },
        {
            "304", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634266, 233830,
                CoordinateHelpers.ConvertToMeter(1486))
        },
        {
            "305", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634257, 232896,
                CoordinateHelpers.ConvertToMeter(1578))
        },
        {
            "306", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 632430, 232208,
                CoordinateHelpers.ConvertToMeter(1598))
        },
        {
            "307", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 632258, 232218,
                CoordinateHelpers.ConvertToMeter(1598))
        },
        {
            "308", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 630353, 230418,
                CoordinateHelpers.ConvertToMeter(1480))
        },
        {
            "309", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631319, 233780,
                CoordinateHelpers.ConvertToMeter(1585))
        },
        {
            "310", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 630966, 231077,
                CoordinateHelpers.ConvertToMeter(1503))
        },
        {
            "400", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 619620, 229461,
                CoordinateHelpers.ConvertToMeter(1493))
        },
        {
            "401", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 617227, 228498,
                CoordinateHelpers.ConvertToMeter(1545))
        },
        {
            "402", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 615883, 226748,
                CoordinateHelpers.ConvertToMeter(1522))
        },
        {
            "403", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 613737, 226943,
                CoordinateHelpers.ConvertToMeter(1493))
        },
        {
            "404", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 613727, 225187,
                CoordinateHelpers.ConvertToMeter(1542))
        },
        {
            "405", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 616037, 222724,
                CoordinateHelpers.ConvertToMeter(1555))
        },
        {
            "406", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 613312, 224296,
                CoordinateHelpers.ConvertToMeter(1549))
        },
        {
            "500", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626242, 229898,
                CoordinateHelpers.ConvertToMeter(1555))
        },
        {
            "501", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626771, 228322,
                CoordinateHelpers.ConvertToMeter(1604))
        },
        {
            "503", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626211, 228931,
                CoordinateHelpers.ConvertToMeter(1572))
        },
        {
            "504", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626728, 228655,
                CoordinateHelpers.ConvertToMeter(1572))
        },
        {
            "505", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627152, 228023,
                CoordinateHelpers.ConvertToMeter(1598))
        },
        {
            "506", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627567, 227024,
                CoordinateHelpers.ConvertToMeter(1765))
        },
        {
            "507", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626876, 224709,
                CoordinateHelpers.ConvertToMeter(1706))
        },
        {
            "508", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627053, 221174,
                CoordinateHelpers.ConvertToMeter(1854))
        },
        {
            "509", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 627832, 220655,
                CoordinateHelpers.ConvertToMeter(1886))
        },
        {
            "510", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626022, 220525,
                CoordinateHelpers.ConvertToMeter(2228))
        },
        {
            "511", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625123, 220421,
                CoordinateHelpers.ConvertToMeter(1975))
        },
        {
            "512", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625270, 220488,
                CoordinateHelpers.ConvertToMeter(1946))
        },
        {
            "513", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623368, 219702,
                CoordinateHelpers.ConvertToMeter(2451))
        },
        {
            "514", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621699, 220163,
                CoordinateHelpers.ConvertToMeter(2352))
        },
        {
            "515", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623442, 222504,
                CoordinateHelpers.ConvertToMeter(2001))
        },
        {
            "516", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623593, 222683,
                CoordinateHelpers.ConvertToMeter(2014))
        },
        {
            "517", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621261, 226198,
                CoordinateHelpers.ConvertToMeter(1634))
        },
        {
            "518", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621633, 226122,
                CoordinateHelpers.ConvertToMeter(1650))
        },
        {
            "519", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621639, 226244,
                CoordinateHelpers.ConvertToMeter(1660))
        },
        {
            "520", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622449, 227025,
                CoordinateHelpers.ConvertToMeter(1703))
        },
        {
            "521", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622981, 227750,
                CoordinateHelpers.ConvertToMeter(1644))
        },
        {
            "522", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623176, 228295,
                CoordinateHelpers.ConvertToMeter(1581))
        },
        {
            "523", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623403, 228941,
                CoordinateHelpers.ConvertToMeter(1552))
        },
        {
            "524", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623400, 228874,
                CoordinateHelpers.ConvertToMeter(1549))
        },
        {
            "525", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623133, 229818,
                CoordinateHelpers.ConvertToMeter(1640))
        },
        {
            "526", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625034, 228614,
                CoordinateHelpers.ConvertToMeter(1631))
        },
        {
            "527", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625267, 228482,
                CoordinateHelpers.ConvertToMeter(1680))
        },
        {
            "528", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625752, 227606,
                CoordinateHelpers.ConvertToMeter(1611))
        },
        {
            "529", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624546, 227234,
                CoordinateHelpers.ConvertToMeter(1578))
        },
        {
            "530", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624749, 225633,
                CoordinateHelpers.ConvertToMeter(1745))
        },
        {
            "531", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625080, 225646,
                CoordinateHelpers.ConvertToMeter(1745))
        },
        {
            "532", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 625003, 224245,
                CoordinateHelpers.ConvertToMeter(1932))
        },
        {
            "533", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623837, 223651,
                CoordinateHelpers.ConvertToMeter(2073))
        },
        {
            "534", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626762, 228299,
                CoordinateHelpers.ConvertToMeter(1598))
        },
        {
            "535", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626399, 227097,
                CoordinateHelpers.ConvertToMeter(1624))
        },
        {
            "536", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626425, 227041,
                CoordinateHelpers.ConvertToMeter(1640))
        },
        {
            "537", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623918, 218737,
                CoordinateHelpers.ConvertToMeter(2041))
        },
        {
            "538", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624420, 228430,
                CoordinateHelpers.ConvertToMeter(1619))
        },
        {
            "600", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 635618, 218440,
                CoordinateHelpers.ConvertToMeter(2080))
        },
        {
            "602", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 635037, 230021,
                CoordinateHelpers.ConvertToMeter(1765))
        },
        {
            "603", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 633389, 229622,
                CoordinateHelpers.ConvertToMeter(1834))
        },
        {
            "604", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 636596, 227584,
                CoordinateHelpers.ConvertToMeter(2119))
        },
        {
            "605", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634780, 227474,
                CoordinateHelpers.ConvertToMeter(2336))
        },
        {
            "606", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 635449, 227022,
                CoordinateHelpers.ConvertToMeter(2349))
        },
        {
            "607", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631139, 225163,
                CoordinateHelpers.ConvertToMeter(1854))
        },
        {
            "608", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631193, 225241,
                CoordinateHelpers.ConvertToMeter(1854))
        },
        {
            "609", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634114, 225835,
                CoordinateHelpers.ConvertToMeter(1939))
        },
        {
            "610", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 634773, 225028,
                CoordinateHelpers.ConvertToMeter(2306))
        },
        {
            "611", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 630254, 220400,
                CoordinateHelpers.ConvertToMeter(2267))
        },
        {
            "612", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 636780, 225584,
                CoordinateHelpers.ConvertToMeter(2119))
        },
        {
            "614", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 630523, 220735,
                CoordinateHelpers.ConvertToMeter(2205))
        },
        {
            "615", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 635122, 223362,
                CoordinateHelpers.ConvertToMeter(1995))
        },
        {
            "616", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 636906, 222394,
                CoordinateHelpers.ConvertToMeter(2441))
        },
        {
            "617", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631688, 222298,
                CoordinateHelpers.ConvertToMeter(2152))
        },
        {
            "618", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 633791, 221053,
                CoordinateHelpers.ConvertToMeter(2359))
        },
        {
            "700", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 615800, 206481,
                CoordinateHelpers.ConvertToMeter(1952))
        },
        {
            "701", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 619543, 206136,
                CoordinateHelpers.ConvertToMeter(2005))
        },
        {
            "702", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 619543, 206136,
                CoordinateHelpers.ConvertToMeter(2005))
        },
        {
            "703", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 616843, 212976,
                CoordinateHelpers.ConvertToMeter(2100))
        },
        {
            "704", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 616087, 213219,
                CoordinateHelpers.ConvertToMeter(2251))
        },
        {
            "705", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 615747, 213185,
                CoordinateHelpers.ConvertToMeter(2198))
        },
        {
            "800", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 628011, 216965,
                CoordinateHelpers.ConvertToMeter(2375))
        },
        {
            "801", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626180, 215189,
                CoordinateHelpers.ConvertToMeter(2287))
        },
        {
            "802", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 628323, 212764,
                CoordinateHelpers.ConvertToMeter(2700))
        },
        {
            "803", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624410, 213236,
                CoordinateHelpers.ConvertToMeter(2566))
        },
        {
            "804", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621522, 206788,
                CoordinateHelpers.ConvertToMeter(2142))
        },
        {
            "805", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624521, 211502,
                CoordinateHelpers.ConvertToMeter(2523))
        },
        {
            "806", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622909, 210729,
                CoordinateHelpers.ConvertToMeter(2457))
        },
        {
            "807", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622509, 211150,
                CoordinateHelpers.ConvertToMeter(2680))
        },
        {
            "808", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622591, 211105,
                CoordinateHelpers.ConvertToMeter(2657))
        },
        {
            "809", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622140, 213316,
                CoordinateHelpers.ConvertToMeter(2628))
        },
        {
            "810", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622246, 213339,
                CoordinateHelpers.ConvertToMeter(2582))
        },
        {
            "811", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622019, 209436,
                CoordinateHelpers.ConvertToMeter(2602))
        },
        {
            "812", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621743, 216917,
                CoordinateHelpers.ConvertToMeter(2641))
        },
        {
            "813", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 623569, 214611,
                CoordinateHelpers.ConvertToMeter(2306))
        },
        {
            "814", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 622782, 212496,
                CoordinateHelpers.ConvertToMeter(2507))
        },
        {
            "815", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621243, 213502,
                CoordinateHelpers.ConvertToMeter(2608))
        },
        {
            "816", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 621084, 211778,
                CoordinateHelpers.ConvertToMeter(2411))
        },
        {
            "817", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 626203, 210609,
                CoordinateHelpers.ConvertToMeter(2405))
        },
        {
            "818", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 624874, 210703,
                CoordinateHelpers.ConvertToMeter(2818))
        },
        {
            "900", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 636211, 216387,
                CoordinateHelpers.ConvertToMeter(2260))
        },
        {
            "901", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 636509, 214098,
                CoordinateHelpers.ConvertToMeter(2362))
        },
        {
            "902", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631352, 215837,
                CoordinateHelpers.ConvertToMeter(2316))
        },
        {
            "903", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 631391, 217393,
                CoordinateHelpers.ConvertToMeter(2359))
        },
        {
            "905", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 629726, 217474,
                CoordinateHelpers.ConvertToMeter(2113))
        },
        {
            "906", CoordinateHelpers.ConvertToWgs84Coordinate(CoordinateSystem.SwissGrid_LV03, 629896, 213550,
                CoordinateHelpers.ConvertToMeter(2415))
        },
    };
}