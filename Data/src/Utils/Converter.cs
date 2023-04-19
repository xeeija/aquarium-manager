namespace Utils
{
    public static class Converter
    {
        public static long ConvertDateToUnixTimeStamp(DateTime d)
        {
            //var epoch = (d.Date - new DateTime(1970, 1, 1)).TotalSeconds;
            // return (long)epoch;

            TimeSpan span = d - new DateTime(1970, 1, 1);
            long returnval = (long)span.TotalSeconds;

            return returnval;
        }


        public static DateTime ConvertUnixTimeStampToDate(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1);
            return epoch.AddSeconds(timestamp);
        }

    }
}
