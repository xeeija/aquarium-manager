using System.Collections.Concurrent;

namespace DAL.Influx
{
    public static class InfluxExtension
    {
        public static long ToUnixTimeStamp(this DateTime date)
        {
            TimeSpan span = date - new DateTime(1970, 1, 1);
            long returnval = (long)span.TotalSeconds;

            return returnval;
        }


        public static DateTime FromUnixTimeStamp(this long unixTimeStamp)
        {
            var epoch = new DateTime(1970, 1, 1);
            return epoch.AddSeconds(unixTimeStamp);
        }

        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            toAdd.AsParallel().ForAll(t => @this.Add(t));
        }
    }
}
