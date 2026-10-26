using MediaLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Interface
{
    public interface IMedia : ICloneable
    {
        int Id { get; }
        string Title { get; set; }
        int Year { get; }
        Genre Genre { get; set; }
        bool IsAvailable { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public double Rating { get; set; }
        string GetInfo();
        void Deconstruct(out int year, out Genre genre, out bool isAvailable);

    }
}
