using Coordinates;

namespace JansScoring.flights.tasks;

public abstract class TaskHWZ : Task
{
    protected TaskHWZ(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        throw new System.NotImplementedException();
    }
}