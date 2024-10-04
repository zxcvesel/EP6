using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDKCurs.Prod
{
    public partial class EditProductForm : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        public int ProductId { get; set; }

        public EditProductForm(int productId)
        {
            InitializeComponent();
            ProductId = productId;
            LoadCategories();
            LoadProductDetails();
        }

        private void LoadCategories()
        {
            string query = "SELECT ID, Name FROM Categories";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBoxCategory.DataSource = dataTable;
                comboBoxCategory.DisplayMember = "Name";
                comboBoxCategory.ValueMember = "ID";
            }
        }

        private void LoadProductDetails()
        {
            string query = "SELECT Title, Author, Description, CategoryID FROM Books WHERE ID = @BookID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BookID", ProductId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    textBoxName.Text = reader["Title"].ToString();
                    textBoxBrand.Text = reader["Author"].ToString();
                    textBoxDescription.Text = reader["Description"].ToString();
                    comboBoxCategory.SelectedValue = reader["CategoryID"];
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string title = textBoxName.Text;
            string author = textBoxBrand.Text;
            string description = textBoxDescription.Text;
            int categoryID = Convert.ToInt32(comboBoxCategory.SelectedValue);

            UpdateProduct(title, author, description, categoryID);
        }

        private void UpdateProduct(string title, string author, string description, int categoryID)
        {
            string query = "UPDATE Books SET Title = @Title, Author = @Author, Description = @Description, CategoryID = @CategoryID WHERE ID = @BookID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@CategoryID", categoryID);
                command.Parameters.AddWithValue("@BookID", ProductId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            MessageBox.Show("Продукт успешно обновлен.");
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
