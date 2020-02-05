namespace app
{
    public static class Utils
    {
        public static string FormatTrackName(string trackName)
        {
            return trackName.Replace("+ ", "-").Replace(" ", "-").ToLower();
        }
    }
}