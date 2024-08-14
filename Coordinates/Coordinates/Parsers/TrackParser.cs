using System;
using System.IO;

namespace Coordinates.Parsers;

public interface TrackParser
{
    /// <summary>
    /// Parses a  file and pass back a track object
    /// </summary>
    /// <param name="fileNameAndPath">the file path and name of the file</param>
    /// <param name="track">output parameter. the parsed track from the file</param>
    /// <param name="referenceCoordinate">provide a reference coordinate to be used complete the missing information of a goal declaration
    /// <para>a goal declaration not in the zone 6/7 format is ambiguous</para>
    /// <para>this is an optional parameter, the parse will use either marker drop 1 or the position at declaration if not reference point has been provided</para></param>
    /// <returns>true:success; false:error</returns>
    bool ParseFile(FileInfo fileInfo, out Track track, Coordinate referenceCoordinate = null);

}