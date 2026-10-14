using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(int id)
            : base($"Элемент с ID {id} не найден") { }
    }
}
