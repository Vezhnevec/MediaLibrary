using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Exceptions
{
    public class ItemAlreadyReturnedException : Exception
    {
        public ItemAlreadyReturnedException(int id)
            : base($"Элемент с ID {id} уже возвращён и доступен") { }
    }
}
