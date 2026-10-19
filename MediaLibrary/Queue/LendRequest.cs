using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary.Queue
{
    public record LendRequest
    {
        public int ItemId { get; set; }
        public string? ReaderName { get; set; }
    }
}
