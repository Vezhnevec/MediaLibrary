using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Exceptions
{
    public class ItemNotAvailableException : Exception
    {
        public ItemNotAvailableException(int id)
            : base($"Элемент с ID {id} уже выдан") { }
    }
}
