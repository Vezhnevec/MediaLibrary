using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Models
{
    public record Screening
    {
        public int MovieId { get; init; }
        public string MovieTitle { get; init; }
        public DateOnly Date { get; init; }
        public TimeOnly StartTime { get; init; }
        public int DurationMinutes { get; init; } 
    }
}
