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
        int Year { get; set; }
        Genre Genre { get; set; }
        bool IsAvailable { get; set; }
        string GetInfo();
    }
}
