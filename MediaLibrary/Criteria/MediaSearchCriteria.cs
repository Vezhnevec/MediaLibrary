using MediaLibrary.Enums;
using MediaLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Criteria
{
    public record MediaSearchCriteria
    {
        public string? Title { get; init; }
        public Genre? Genre { get; init; }
        public int? MinYear { get; init; }
        public bool? IsAvailableOnly { get; init; }
               
        public bool Matches(IMedia item)
        {
            string[] userWords = Title.Split(' ');
            foreach (var word in userWords)
            {
                if (string.IsNullOrEmpty(word))
                    continue;

                if (!item.Title.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (Genre.HasValue && Genre.Value != item.Genre)
                return false;

            if (MinYear.HasValue && MinYear.Value > item.Year)
                return false;

            if (IsAvailableOnly.HasValue && IsAvailableOnly.Value != item.IsAvailable)
                return false;

            return true;
        }

    }
}
