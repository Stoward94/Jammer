using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, string timeZoneId)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return time.ToTimeZoneTime(tzi);
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, TimeZoneInfo tzi)
        {
            if (time.Kind == DateTimeKind.Local) return time;

            return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
        }

        public static string ToFullDateString(this DateTime date, bool showYear = false)
        {
            string dayOfWeek = date.DayOfWeek.ToString();
            
            //Date
            int dayNumber = date.Day;
            string daySuffix;
            string month = date.ToString("MMMM");
            string year = date.ToString("yyyy");

            //Add day suffix
            switch (dayNumber)
            {
                case 1:
                case 21:
                case 31:
                    daySuffix = "st";
                    break;

                case 2:
                case 22:
                    daySuffix = "nd";
                    break;

                case 3:
                case 23:
                    daySuffix = "rd";
                    break;

                default:
                    daySuffix = "th";
                    break;

            }

            string dateString = $"{dayOfWeek} {dayNumber}{daySuffix} {month}";

            if (showYear)
                dateString += " " + year;

            return dateString;
        }

        public static string ToFullDateTimeString(this DateTime date)
        {
            string dateString = date.ToFullDateString();
            string timeString = date.ToShortTimeString();

            return dateString + " at " + timeString;
        }

        public static string ToMinsAgoTime(this DateTime time, DateTime now)
        {
            TimeSpan diff = now.Subtract(time);

            int mins = (int) diff.TotalMinutes;

            //Seconds ago
            if (mins < 1)
                return (int) diff.TotalSeconds + "s ago";

            //Mins ago
            if (mins < 60)
                return mins + "m ago";

            int hours = (int) diff.TotalHours;

            //Hours ago
            if (hours < 24)
                return hours + "h ago";

            int days = (int) diff.TotalDays;

            //Yesterday
            if(days < 2)
                return time.ToString("HH:mm") + " yesterday";

            return time.ToString("HH:mm | dd/MM/yy");
        }
    }
}