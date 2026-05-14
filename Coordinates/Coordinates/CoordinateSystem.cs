namespace Coordinates
{
    /// <summary>
    /// Identifies the geodetic reference frame and map projection a pair of
    /// numeric coordinates is expressed in. Used as metadata on
    /// <see cref="Coordinate"/> and <see cref="Declaration"/> and as the
    /// discriminator for the parsing / conversion helpers in
    /// <see cref="CoordinateHelpers"/>.
    /// </summary>
    public enum CoordinateSystem
    {
        /// <summary>
        /// Geographic coordinates on the WGS84 ellipsoid, expressed as
        /// decimal degrees latitude / longitude.
        /// </summary>
        WGS84_LatLon,

        /// <summary>
        /// Universal Transverse Mercator on the WGS84 ellipsoid.
        /// Zone is given as "&lt;number&gt;&lt;letter&gt;" e.g. "32U".
        /// </summary>
        UTM_WGS84,

        /// <summary>
        /// Universal Transverse Mercator on the GRS80 ellipsoid (ETRS89 datum).
        /// Within Europe ETRS89 is currently offset from WGS84 by a few decimetres;
        /// the projection mathematics are identical to UTM_WGS84 because the
        /// GRS80 and WGS84 ellipsoids are equal within ~0.1 mm.
        /// </summary>
        UTM_ETRS89,

        /// <summary>
        /// Swiss national grid LV95 (CH1903+ / EPSG:2056).
        /// False origin at Bern observatory; easting around 2'600'000,
        /// northing around 1'200'000.
        /// </summary>
        SwissGrid_LV95,

        /// <summary>
        /// Swiss national grid LV03 (CH1903 / EPSG:21781).
        /// Same projection as LV95 but with a 2'000'000 / 1'000'000 m offset removed,
        /// so easting around 600'000 and northing around 200'000.
        /// </summary>
        SwissGrid_LV03
    }
}
