using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Management_System.Entity
{
    internal class Clothing:Products
    {
        public string Size {  get; set; }
        public string Color {  get; set; }
    }
}
