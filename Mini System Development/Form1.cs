using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ProductManagementSystem
{
    public partial class Form1 : Form
    {
        private DatabaseHelper dbHelper;
        private int selectedProductId = -1;

        public Form1()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts();
            ClearFields();
        }

        private void LoadProducts()
        {
            try
            {
                DataTable dt = dbHelper.GetAllProducts();
                dgvProducts.DataSource = dt;

                dgvProducts.Columns["ID"].Width = 50;
                dgvProducts.Columns["Product Name"].Width = 200;
                dgvProducts.Columns["Price"].Width = 100;
                dgvProducts.Columns["Quantity"].Width = 100;

                dgvProducts.Columns["Price"].DefaultCellStyle.Format = "₱#,##0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtProductName.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
            selectedProductId = -1;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            txtProductName.Focus();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Product name is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Price is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid positive price!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Text = "";
                txtPrice.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Quantity is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid quantity (0 or more)!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Text = "";
                txtQuantity.Focus();
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                string name = txtProductName.Text.Trim();
                decimal price = decimal.Parse(txtPrice.Text);
                int quantity = int.Parse(txtQuantity.Text);

                if (dbHelper.AddProduct(name, price, quantity))
                {
                    MessageBox.Show("Product added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Please select a product to update!", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInputs())
                return;

            try
            {
                string name = txtProductName.Text.Trim();
                decimal price = decimal.Parse(txtPrice.Text);
                int quantity = int.Parse(txtQuantity.Text);

                if (dbHelper.UpdateProduct(selectedProductId, name, price, quantity))
                {
                    MessageBox.Show("Product updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Please select a product to delete!", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (dbHelper.DeleteProduct(selectedProductId))
                    {
                        MessageBox.Show("Product deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadProducts();
            ClearFields();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                selectedProductId = Convert.ToInt32(row.Cells["ID"].Value);
                txtProductName.Text = row.Cells["Product Name"].Value.ToString();

                string priceValue = row.Cells["Price"].Value.ToString();
                if (priceValue.StartsWith("₱"))
                {
                    priceValue = priceValue.Replace("₱)", "");
                }
                txtPrice.Text = priceValue;

                txtQuantity.Text = row.Cells["Quantity"].Value.ToString();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void panelInput_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelButtons_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}