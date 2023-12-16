using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Management_System.Exceptions
{
    internal class OrderNotFound:ApplicationException
    {
        public string Mssg { get; set; }
        public OrderNotFound(string mssg) : base(mssg)
        {
            Mssg = mssg;
        }
    }
}
