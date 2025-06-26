namespace PL.Helpers
{
    /// <summary>
    /// Configuration for map services and API keys.
    /// </summary>
    public static class MapConfig
    {
        /// <summary>
        /// Azure Maps subscription key.
        /// Replace with your actual Azure Maps key.
        /// </summary>
        public const string AZURE_MAPS_KEY = "***";

        /// <summary>
        /// Default map center coordinates (Jerusalem, Israel).
        /// </summary>
        public const double DEFAULT_LATITUDE = 31.7683;
        public const double DEFAULT_LONGITUDE = 35.2137;

        /// <summary>
        /// Default zoom level for maps.
        /// </summary>
        public const int DEFAULT_ZOOM_LEVEL = 12;

        /// <summary>
        /// Map tile server URL for OpenStreetMap (free alternative).
        /// </summary>
        public const string OSM_TILE_SERVER = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
    }
}