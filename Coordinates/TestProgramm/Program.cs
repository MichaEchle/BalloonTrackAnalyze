using System;
using Coordinates;
namespace TestProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            Track track;
            if(!IGCParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\E94BC98E-001-20611105838 - Original.igc",out track))
            {
                Console.WriteLine("Error parsing track");
            }

            Console.ReadLine();
        }
    }
}
