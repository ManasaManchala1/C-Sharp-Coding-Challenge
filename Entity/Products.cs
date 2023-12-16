using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Order_Management_System.Entity
{
    internal class Products
    {
        public int ProductID {  get; set; }
        public string ProductName { get; set; }
        public string Description {  get; set; }
        public decimal Price {  get; set; }
        public int QuantityInStock {  get; set; }
        public string Type {  get; set; }
        public override string ToString()
        {
            return $"ProductName:{ProductName},Description:{Description}";
        }
    }

}
