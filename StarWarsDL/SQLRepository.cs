using System.Data.SqlClient;
using StarWarsModel;

namespace StarWarsDL
{
    public class SQLRepository : IRepository{
        private readonly string _connectionStrings;
        public SQLRepository(string p_connectionStrings){
            _connectionStrings = p_connectionStrings;
        }

        public Customer AddCustomer(Customer n_customer){
            string sqlQuery = @"insert into Customer 
                            values(@CustomerName, @CustomerAddress, @CustomerNumber, @CustomerEmail)";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                command.Parameters.AddWithValue("@CustomerName", n_customer.Name);
                command.Parameters.AddWithValue("@CustomerAddress", n_customer.Address);
                command.Parameters.AddWithValue("@CustomerNumber", n_customer.Number);
                command.Parameters.AddWithValue("@CustomerEmail", n_customer.Email);

                command.ExecuteNonQuery();
            }
        return n_customer;
        }

        public Storefront AddStoreFront(Storefront n_storeFront){
            string sqlQuery = @"insert into StoreFront
                            values(@StoreName, @StoreAddress)";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                command.Parameters.AddWithValue("@StoreName", n_storeFront.Name);
                command.Parameters.AddWithValue("@StoreAddress", n_storeFront.Address);

                command.ExecuteNonQuery();
            };
        return n_storeFront;
        }

        public List<Order> CustomerOrder(int p_CustomerID)
        {
            List<Order> listOfCustomerOrder = new List<Order>();
            string sqlQuery = @$"SELECT co.OrderID, sf.StoreName, p.ProductName, lico.Quantity, co.Total from StoreFront sf
                                INNER JOIN CustomerOrder co on sf.StoreID = co.StoreID 
                                INNER JOIN LineItems_CustomerOrders lico on co.OrderID = lico.OrderID 
                                INNER JOIN LineItem li on lico.LineItemID = li.LineItemID 
                                INNER JOIN Product p on li.LineItemID = p.LineItemID 
                                WHERE co.CustomerID = {p_CustomerID}";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read()){
                    listOfCustomerOrder.Add(new Order(){
                        _orderID = reader.GetInt32(0),
                        _storeName = reader.GetString(1),
                        _productName = reader.GetString(2),
                        _quantity = reader.GetInt32(3),
                        _totalPrice = reader.GetInt32(4),
                    });
                }
            }
        return listOfCustomerOrder;

        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> listOfCustomer = new List<Customer>();
            string sqlQuery = @"select * from Customer";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    listOfCustomer.Add(new Customer(){
                        _customerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                        Number = reader.GetString(3),
                        Email = reader.GetString(4),
                    });
                }
            }
        return listOfCustomer;
        }

        public List<LineItems> GetAllLineItems(int p_ID)
        {
            List<LineItems> listOfStoreProducts = new List<LineItems>();
            string sqlQuery = @"select li.LineItemID, ProductName, ProductPrice, Quantity from Product p 
                                join LineItem li
                                on p.LineItemID = li.LineItemID
                                where li.StoreID = "+p_ID;
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    listOfStoreProducts.Add(new LineItems(){
                        _lineItemID = reader.GetInt32(0),
                        _productName = reader.GetString(1),
                        _productPrice = reader.GetInt32(2),
                        _quantity = reader.GetInt32(3),
                    });
                }
            }
        return listOfStoreProducts;
        }

        public List<LineItems> GetAllStoreItems(int p_ID)
        {
            List<LineItems> listOfProducts = new List<LineItems>();
            string sqlQuery = @"select li.LineItemID, ProductName, Quantity from Product p
                                inner join LineItem li on p.LineItemID = li.LineItemID 
                                where li.StoreID = "+p_ID;
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    listOfProducts.Add(new LineItems(){
                        _lineItemID = reader.GetInt32(0),
                        _productName = reader.GetString(1),
                        _quantity = reader.GetInt32(2),
                    });
                }
            }
        return listOfProducts;
        }

        public List<Storefront> GetAllStores()
        {
            List<Storefront> listOfStores = new List<Storefront>();
            string sqlQuery = @"select * from StoreFront";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    listOfStores.Add(new Storefront(){
                        _storeID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                    });
                }
            }
        return listOfStores;
        }

        public List<ShoppingCart> PlaceOrder(int p_ID, float p_Price, int p_Quantity, int p_StoreID, int p_CustomerID)
        {
            string sqlQuery = @$"INSERT CustomerOrder  VALUES(@StoreID, @CustomerID, @Total)
                                INSERT LineItems_CustomerOrders  VALUES(@LineItemID, @Quantity)
                                UPDATE LineItem  set Quantity = Quantity - {p_Quantity} WHERE LineItem.LineItemID = {p_ID}
                                SELECT co.OrderID, c.CustomerName, lico.Quantity, p.ProductID, p.ProductName From Customer c
                                INNER JOIN CustomerOrder co on c.CustomerID = co.CustomerID 
                                INNER JOIN LineItems_CustomerOrders lico on co.OrderID = lico.OrderID 
                                INNER JOIN LineItem li on lico.LineItemID = li.LineItemID 
                                INNER JOIN Product p on li.ProductID = p.ProductID 
                                WHERE c.CustomerID = {p_CustomerID}";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                command.Parameters.AddWithValue("@StoreID", p_StoreID);
                command.Parameters.AddWithValue("@CustomerID", p_CustomerID);
                command.Parameters.AddWithValue("@Total", p_Price);
                command.Parameters.AddWithValue("@LineItemID", p_ID);
                command.Parameters.AddWithValue("@Quantity", p_Quantity);

                command.ExecuteNonQuery();
            }
            return null;
        }

        public List<LineItems> ReplenishInventory(int p_ID, int p_Quantity, int p_ItemID)
        {
            List<LineItems> listOfStoreProducts = new List<LineItems>();
            string sqlQuery = @$"update LineItem set Quantity = Quantity + {p_Quantity} where LineItemID = {p_ItemID}
                                select li.LineItemID, ProductName, Quantity from Product p
                                inner join LineItem li on p.LineItemID = li.LineItemID 
                                where li.StoreID = {p_ID}";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    listOfStoreProducts.Add(new LineItems(){
                        _lineItemID = reader.GetInt32(0),
                        _productName = reader.GetString(1),
                        _quantity = reader.GetInt32(2),
                    });
                }
            }
        return listOfStoreProducts;
        }

        public List<Order> StoreOrder(int p_StoreID)
        {
            List<Order> listOfCustomerOrder = new List<Order>();
            string sqlQuery = @$"SELECT co.OrderID, c.CustomerName, p.ProductName, li.LineItemID, lico.Quantity, co.Total from Customer c
                                INNER JOIN CustomerOrder co on c.CustomerID = co.CustomerID 
                                INNER JOIN LineItems_CustomerOrders lico on co.OrderID = lico.OrderID 
                                INNER JOIN LineItem li on lico.LineItemID = li.LineItemID 
                                INNER JOIN Product p on li.ProductID = p.ProductID 
                                WHERE co.StoreID = {p_StoreID}";
            using (SqlConnection con = new SqlConnection(_connectionStrings)){
                con.Open();
                SqlCommand command = new SqlCommand(sqlQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read()){
                    listOfCustomerOrder.Add(new Order(){
                        _orderID = reader.GetInt32(0),
                        _customerName = reader.GetString(1),
                        _productName = reader.GetString(2),
                        _lineItemID = reader.GetInt32(3),
                        _quantity = reader.GetInt32(4),
                        _totalPrice = reader.GetInt32(5),
                    });
                }
            }
        return listOfCustomerOrder;
        }
    }
}