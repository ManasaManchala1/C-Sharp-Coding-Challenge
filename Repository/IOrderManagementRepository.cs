using Order_Management_System.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Management_System.Repository
{
    internal interface IOrderManagementRepository
    {
        void createOrder(Users user, List<Products> products);
        void cancelOrder(int userId, int orderId);
        void createProduct(Users user, Products product);
        int createUser(Users user);
        List<Products> getAllProducts();
        List<Products> getOrderByUser(Users user);
    }
}
