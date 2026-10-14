using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Exceptions
{
    public class InvalidMediaDataException : Exception
    {
        public InvalidMediaDataException(string message)
            : base($"Некорректные данные: {message}") { }
    }
}
