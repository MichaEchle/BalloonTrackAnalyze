using Competition;
using Coordinates;

namespace TestProgramm;
internal class German_and_British_National_Hot_Air_Balloon_Championships_2025
{
    internal void Flight1()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 1;
        _ = flight.MapPilotNamesToTracks(@".\PilotsMapping.csv");
        //TODO parse tracks
        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToFeet(1800));

        ChecksTask1(flight);

    }

    internal void ChecksTask1(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }
        }
    }
}
