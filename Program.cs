using Order_Management_System.Entity;
using Order_Management_System.Repository;
using System.Transactions;

IOrderManagementRepository orderManagementRepository=new OrderProcessorRepository();
while (true)
{
    Console.WriteLine("Main Menu");
    Console.WriteLine("1.Create User");
    Console.WriteLine("2.Create Product");
    Console.WriteLine("3.Cancel Order");
    Console.WriteLine("4.Get all products");
    Console.WriteLine("5.Get Products by user");
    Console.WriteLine("6.Exit");
    Console.WriteLine("Enter your choice");
    int choice=int.Parse(Console.ReadLine());
    switch (choice)
    {
        case 1:
            Users user = new Users() {UserName = "Manasa", Password = "ytdsgf", Role = "Admin" };
            int status = orderManagementRepository.createUser(user);
            if (status == 0) Console.WriteLine("User not Added");
            break;
        case 2:
            Users productuser = new Users { UserID = 10, UserName = "Manasa", Password = "ytdsgf", Role = "Admin" };
            Products productt = new Products();
            Console.WriteLine("ProductName");
            productt.ProductName = Console.ReadLine();
            Console.WriteLine("Description");
            productt.Description = Console.ReadLine();
            Console.WriteLine("Price");
            productt.Price = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Quantity in stock");
            productt.QuantityInStock = int.Parse(Console.ReadLine());
            Console.WriteLine("Type");
            productt.Type = Console.ReadLine();
            orderManagementRepository.createProduct(productuser, productt);
            break;
        case 3:
            Console.WriteLine("userid");
            int userid=int.Parse(Console.ReadLine());
            Console.WriteLine("Orderid");
            int orderid=int.Parse(Console.ReadLine());
            orderManagementRepository.cancelOrder(userid, orderid);
            break;
        case 4:
            List<Products> allproducts = orderManagementRepository.getAllProducts();
            foreach (Products product2 in allproducts) { Console.WriteLine(product2); }
            break;
        case 5:
            Console.WriteLine("Enter user details");
            Users userproducts = new Users();
            Console.WriteLine("userid");
            userproducts.UserID=int.Parse(Console.ReadLine());
            Console.WriteLine("username");
            userproducts.UserName= Console.ReadLine();
            Console.WriteLine("password");
            userproducts.Password= Console.ReadLine();
            Console.WriteLine("Role");
            userproducts.Role=Console.ReadLine();
            List<Products> productsbyuser = orderManagementRepository.getOrderByUser(userproducts);
            foreach (Products product3 in productsbyuser) Console.WriteLine(product3);
            break;
        case 6:
            Environment.Exit(0);
            break;
        default: 
            Console.WriteLine("Invalid Choice");
            break;

    }

}

