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
            connectionstring = DbConnUtil.GetConnectionString();
            cmd = new SqlCommand();
        }
        public void cancelOrder(int userId, int orderId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();


                    if (isOrderExists(conn, userId, orderId))
                    {

                        using (SqlCommand cmd = new SqlCommand("delete from Orders where userId = @uid and orderId = @oid", conn))
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool isUserExists(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("select 1 from Users where userId = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }

        public bool isOrderExists(SqlConnection conn, int userId, int orderId)
        {
            using (SqlCommand cmd = new SqlCommand("select 1 from Orders where userId = @uid and orderId = @oid", conn))
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
                        using (SqlCommand cmd = new SqlCommand("insert into Products (productName, description, price, quantityInStock, type) values (@pname, @desc, @price, @quantity, @type)", conn))
                        {
                            cmd.Parameters.AddWithValue("@pname", product.ProductName);
                            cmd.Parameters.AddWithValue("@desc", product.Description);
                            cmd.Parameters.AddWithValue("@price", product.Price);
                            cmd.Parameters.AddWithValue("@quantity", product.QuantityInStock);
                            cmd.Parameters.AddWithValue("@type", product.Type);
                            int productId = Convert.ToInt32(cmd.ExecuteScalar());

                            Console.WriteLine($"Product added successfully with productId");
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

        public bool isAdmin(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("select 1 from Users where userId = @uid and role = 'Admin'", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }


        public int createUser(Users user)
        {
            int createstatus = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("insert into Users (username, password, role) values (@username, @password, @role)", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        createstatus = cmd.ExecuteNonQuery();
                        if (createstatus == 1) { Console.WriteLine("User Added Successfully"); }
                    }

                }

            }
            catch (Exception e) { Console.WriteLine(e.Message); }
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


                    using (SqlCommand cmd = new SqlCommand("select p.* from Products p join Orders o on p.productId = o.productId where o.userId = @uid", conn))
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


                    using (SqlCommand cmd = new SqlCommand("select userId from Users where userId = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", user.UserID);
                        object uId = cmd.ExecuteScalar();

                        if (uId == null)
                        {
                            createUser(user);
                        }
                    }



                    using (SqlCommand cmd = new SqlCommand("insert into Orders (userId, productId) values (@userid, @productid)", conn))
                    {
                        foreach (Products product in products)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@userid", user.UserID);
                            cmd.Parameters.AddWithValue("@productid", product.ProductID);
                            cmd.ExecuteNonQuery();
                        }

                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }



    }
}
