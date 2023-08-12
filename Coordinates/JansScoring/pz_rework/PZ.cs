using Coordinates;
using JansScoring.flights;
using System;
using System.IO;

namespace JansScoring.pz_rework;

public abstract class PZ
{
    /// <summary>
    /// The ID of the PZ
    /// </summary>
    public int ID
    {
        get;
    }

    protected PZ(int ID)
    {
        this.ID = ID;
    }

    public abstract bool IsInsidePz(Flight flight, Track track, Coordinate coordinate, out String comment);
}