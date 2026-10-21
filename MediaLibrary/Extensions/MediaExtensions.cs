using MediaLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Extensions
{
    public static class MediaExtensions
    {     
        public static bool IsNewRelease(this IMedia item, int thresholdYears = 2) 
        {
            return (DateTime.Now.Year - item.Year) <= thresholdYears;
        }
        public static string GetEraByYear(this IMedia item)
        {
            return item.Year switch
            {
                < 1900 => "Дореволюционное",
                >= 1900 and < 1950 => "Середина века",
                >= 1950 and < 2000 => "Современная классика",
                >= 2000 and < 2020 => "Новое время",
                >= 2020 => "Современность"
            };
        }
        public static (int Years, int Months, int Days) GetDetailedAge(this IMedia media)
        {
            DateOnly releaseDate = media.ReleaseDate;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            if (releaseDate > today) return (0, 0, 0);

            int years = today.Year - releaseDate.Year;
            if (releaseDate.AddYears(years) > today)
            {
                years--;
            }

            DateOnly dateAfterYears = releaseDate.AddYears(years);


            int months = (today.Year - dateAfterYears.Year) * 12 + (today.Month - dateAfterYears.Month);
            if (dateAfterYears.AddMonths(months) > today)
            {
                months--;
            }

            DateOnly dateAfterMonths = dateAfterYears.AddMonths(months);


            int days = today.DayNumber - dateAfterMonths.DayNumber;

            return (years, months, days);
        }
    }
}
