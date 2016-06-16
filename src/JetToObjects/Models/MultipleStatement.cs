using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ekm.oledb.data
{
    public class MultipleStatement
    {
        public string Query { get; set; }
        public Param[] Parameters { get; set; }
    }
}
