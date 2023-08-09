using ABI.Windows.ApplicationModel;
using System;
using System.Collections.Generic;

namespace JansScoring;

public class StringHelper
{
    public static string IntArrayToString(int[] array)
    {
        return String.Join("", new List<int>(array).ConvertAll(i => i.ToString()).ToArray());
    }

    public static string IntListToString(List<int> list)
    {
        return String.Join("", list.ConvertAll(i => i.ToString()).ToArray());
    }
}