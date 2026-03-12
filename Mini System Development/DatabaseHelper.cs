using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ProductManagementSystem
{
    class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public DataTable GetAllProducts()
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT product_id AS 'ID', product_name AS 'Product Name', " +
                                  "price AS 'Price', quantity AS 'Quantity' FROM products ORDER BY product_id";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading products: " + ex.Message);
                }
            }

            return dataTable;
        }

        public bool AddProduct(string name, decimal price, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO products (product_name, price, quantity) " +
                                 "VALUES (@name, @price, @quantity)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding product: " + ex.Message);
                }
            }
        }

        public bool UpdateProduct(int id, string name, decimal price, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE products SET product_name = @name, " +
                                 "price = @price, quantity = @quantity WHERE product_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating product: " + ex.Message);
                }
            }
        }

        public void ResetAutoIncrement()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM products";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count == 0)
                    {
                        string resetQuery = "ALTER TABLE products AUTO_INCREMENT = 1";
                        MySqlCommand resetCmd = new MySqlCommand(resetQuery, conn);
                        resetCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string maxQuery = "SELECT MAX(product_id) FROM products";
                        MySqlCommand maxCmd = new MySqlCommand(maxQuery, conn);
                        int maxId = Convert.ToInt32(maxCmd.ExecuteScalar());

                        string resetQuery = $"ALTER TABLE products AUTO_INCREMENT = {maxId + 1}";
                        MySqlCommand resetCmd = new MySqlCommand(resetQuery, conn);
                        resetCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error resetting auto-increment: " + ex.Message);
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM products WHERE product_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ResetAutoIncrement();
                        }

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting product: " + ex.Message);
                }
            }
        }
    }
}

    