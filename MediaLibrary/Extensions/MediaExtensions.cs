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
    }
}
