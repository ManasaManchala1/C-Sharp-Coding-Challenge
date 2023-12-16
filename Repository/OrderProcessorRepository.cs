using Order_Management_System.Entity;
using Order_Management_System.Exceptions;
using Order_Management_System.Utility;
using System.Data.SqlClient;

namespace Order_Management_System.Repository
{
    internal class OrderProcessorRepository : IOrderManagementRepository
    {
        string connectionstring;
        SqlCommand cmd;
        public OrderProcessorRepository()
        {
            connectionstring=DbConnUtil.GetConnectionString();
            cmd = new SqlCommand();
        }
        public void cancelOrder(int userId, int orderId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Check if the userId and orderId exist in the Orders table
                    if (isOrderExists(conn, userId, orderId))
                    {
                        // If the order exists, proceed to cancel it
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE userId = @uid AND orderId = @oid", conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@oid", orderId);
                            cmd.ExecuteNonQuery();

                            Console.WriteLine("Order canceled successfully.");
                        }
                    }
                    else
                    {
                        if (!isUserExists(conn, userId))
                        {
                            throw new UserNotFound($"User with userId {userId} not found.");
                        }

                        throw new OrderNotFound($"Order with orderId {orderId} not found for userId {userId}.");
                    }
                }
            }catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Helper method to check if the userId exists in the Users table
        private bool isUserExists(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM Users WHERE userId = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }

        // Helper method to check if the orderId and userId exist in the Orders table
        private bool isOrderExists(SqlConnection conn, int userId, int orderId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM Orders WHERE userId = @uid AND orderId = @oid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@oid", orderId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }






        public void createProduct(Users user, Products product)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    if (isAdmin(conn, user.UserID))
                    {
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Products (productName, description, price, quantityInStock, type) VALUES (@pname, @desc, @price, @quantity, @type); SELECT SCOPE_IDENTITY()", conn))
                        {
                            cmd.Parameters.AddWithValue("@pname", product.ProductName);
                            cmd.Parameters.AddWithValue("@desc", product.Description);
                            cmd.Parameters.AddWithValue("@price", product.Price);
                            cmd.Parameters.AddWithValue("@quantity", product.QuantityInStock);
                            cmd.Parameters.AddWithValue("@type", product.Type);
                            int productId = Convert.ToInt32(cmd.ExecuteScalar());

                            Console.WriteLine($"Product added successfully with productId: {productId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User is not authorized to create products. Admin access required.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Helper method to check if the user is an admin
        private bool isAdmin(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM Users WHERE userId = @uid AND role = 'Admin'", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }


        public int createUser(Users user)
        {
            int createstatus=0;
            try
            {
                using(SqlConnection conn=new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("insert into Users (username, password, role) VALUES (@username, @password, @role)", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        createstatus=cmd.ExecuteNonQuery();
                        if (createstatus == 1) { Console.WriteLine("User Added Successfully"); }
                    }

                }

            }
            catch(Exception e) { Console.WriteLine(e.Message); }
            return createstatus;
        }

        public List<Products> getAllProducts()
        {
            List<Products> products = new List<Products>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Retrieve all products from the Products table
                    using (SqlCommand cmd = new SqlCommand("select * from Products", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products product = new Products
                                {
                                    ProductID = Convert.ToInt32(reader["productId"]),
                                    ProductName = Convert.ToString(reader["productName"]),
                                    Description = Convert.ToString(reader["description"]),
                                    Price = Convert.ToDecimal(reader["price"]),
                                    QuantityInStock = Convert.ToInt32(reader["quantityInStock"]),
                                    Type = Convert.ToString(reader["type"])
                                };

                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return products;
        }


        public List<Products> getOrderByUser(Users user)
        {
            List<Products> orderedProducts = new List<Products>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Retrieve products ordered by the specific user from the Orders table
                    using (SqlCommand cmd = new SqlCommand("SELECT p.* FROM Products p JOIN Orders o ON p.productId = o.productId WHERE o.userId = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", user.UserID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products product = new Products
                                {
                                    ProductID = Convert.ToInt32(reader["productId"]),
                                    ProductName = Convert.ToString(reader["productName"]),
                                    Description = Convert.ToString(reader["description"]),
                                    Price = Convert.ToDecimal(reader["price"]),
                                    QuantityInStock = Convert.ToInt32(reader["quantityInStock"]),
                                    Type = Convert.ToString(reader["type"])
                                };

                                orderedProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return orderedProducts;
        }


        public void createOrder(Users user, List<Products> products)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Check if the user already exists in the Users table
                    using (SqlCommand cmd = new SqlCommand("SELECT userId FROM Users WHERE userId = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", user.UserID);
                        object uId = cmd.ExecuteScalar();

                        if (uId == null)
                        {
                            createUser(user); // Create the user if not found
                        }
                    }

                    // Now, get the userId (whether it already existed or was just added)
                    int userId = getUserId(conn, user.UserID);

                    // Check if the userId is valid before proceeding
                    if (userId != -1)
                    {
                        // Insert records into the Orders table
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Orders (userId, productId) VALUES (@userid, @productid)", conn))
                        {
                            foreach (Products product in products)
                            {
                                cmd.Parameters.Clear(); // Clear previous parameters
                                cmd.Parameters.AddWithValue("@userid", userId); // Use the obtained userId
                                cmd.Parameters.AddWithValue("@productid", product.ProductID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to get a valid userId. Order creation aborted.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private int getUserId(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT userId FROM Users WHERE userId = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null) ? (int)result : -1; // Return -1 if user not found
            }
        }

    }
}
