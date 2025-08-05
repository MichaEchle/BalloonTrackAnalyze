using System;

namespace JansScoring.pz_rework;

public class PZInfrigement
{
    public PZInfrigement(DateTime infrigementBegin, DateTime infrigementEnd, double distance)
    {
        this.infrigementBegin = infrigementBegin;
        this.infrigementEnd = infrigementEnd;
        this.distance = distance;
    }

    public DateTime infrigementBegin
    {
        get;
        set;
    }

    public DateTime infrigementEnd
    {
        get;
        set;
    }

    public double distance
    {
        get;
        set;
    }
}