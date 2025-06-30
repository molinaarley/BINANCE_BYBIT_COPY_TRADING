using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Converters
{
    public class DatetimeConvert
    {

        public static DateTime GetDateParisTimeZone(DateTime utcDateTime)
        {
      
            TimeZoneInfo parisTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
            DateTime dateTimeParis = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, parisTimeZone);
            return dateTimeParis;
        }
    }
}
