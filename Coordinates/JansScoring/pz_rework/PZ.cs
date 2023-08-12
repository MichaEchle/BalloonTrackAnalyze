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
    private int id
    {
        get;
    }

    protected PZ(int id)
    {
        this.id = id;
    }

    public abstract bool IsInsidePz(Flight flight, Track track, Coordinate coordinate, out String comment);
}