using Order_Management_System.Entity;
using Order_Management_System.Repository;
using System.Transactions;

IOrderManagementRepository orderManagementRepository=new OrderProcessorRepository();
Console.WriteLine("Create User");
Users user = new Users() { UserID=5,UserName = "Manasa", Password = "ytdsgf", Role = "Admin" };
int status = orderManagementRepository.createUser(user);
if (status == 0) Console.WriteLine("User not Added");

Console.WriteLine("Create Order");
Console.WriteLine("Enter User details");
Users user1 = new Users();
Console.WriteLine("Username");
user1.UserName = Console.ReadLine();
Console.WriteLine("Password");
user1.Password = Console.ReadLine();
Console.WriteLine("Role");
user1.Role = Console.ReadLine();
Console.WriteLine("Enter number of products");
int count = int.Parse(Console.ReadLine());
List<Products> products = new List<Products>();
while (count > 0)
{
    Products product1 = new Products();
    Console.WriteLine("Product Name");
    product1.ProductName = Console.ReadLine();
    Console.WriteLine("Description");
    product1.Description = Console.ReadLine();
    Console.WriteLine("Price");
    product1.Price = decimal.Parse(Console.ReadLine());
    Console.WriteLine("Quantity in stock");
    product1.QuantityInStock = int.Parse(Console.ReadLine());
    Console.WriteLine("Type");
    product1.Type = Console.ReadLine();
    products.Add(product1);
    count--;
}
orderManagementRepository.createOrder(user, products);

Console.WriteLine("Create Product");
Products product = new Products();
product.ProductName = Console.ReadLine();
Console.WriteLine("Description");
product.Description = Console.ReadLine();
Console.WriteLine("Price");
product.Price = decimal.Parse(Console.ReadLine());
Console.WriteLine("Quantity in stock");
product.QuantityInStock = int.Parse(Console.ReadLine());
Console.WriteLine("Type");
product.Type = Console.ReadLine();
orderManagementRepository.createProduct(user, product);

Console.WriteLine("Getallproducts");
List<Products> allproducts = orderManagementRepository.getAllProducts();
foreach (Products product2 in allproducts) { Console.WriteLine(product); }

List<Products> productsbyuser = orderManagementRepository.getOrderByUser(user);
foreach (Products product3 in productsbyuser) Console.WriteLine(product);


