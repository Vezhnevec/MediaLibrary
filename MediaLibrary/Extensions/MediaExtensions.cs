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
        private static int _newReleaseThresholdYears = 2;
        public static int NewReleaseThresholdYears
        {
            get => _newReleaseThresholdYears;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Порог не может быть отрицательным");
                _newReleaseThresholdYears = value;
            }
        }
        public static bool IsNewRelease(this IMedia item, int thresholdYears = 2) 
        {
            return (DateTime.Now.Year - item.Year) <= thresholdYears;
        }
    }
}
