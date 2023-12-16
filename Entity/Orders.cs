using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Management_System.Entity
{
    internal class Orders
    {
        public int OrderID {  get; set; }
        public int UserID {  get; set; }
        public int ProductID {  get; set; }
    }
}
