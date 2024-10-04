using MDKCurs.Prod;
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
using System.Xml.Linq;

namespace MDKCurs
{
    public partial class Books : Form
    {
        public Books()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
            LoadBooks();
            LoadCategory();
            LoadAuthors();
        }
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private SqlConnection connection;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                textBoxName.Text = selectedRow.Cells["Title"].Value.ToString();
                comboBoxBrands.Text = selectedRow.Cells["Author"].Value.ToString();
                textBoxDescription.Text = selectedRow.Cells["Description"].Value.ToString();
                comboBoxCategory.SelectedValue = selectedRow.Cells["CategoryID"].Value;
            }

        }

        private void LoadCategory()
        {
            string query = "SELECT ID, Name FROM Categories";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            comboBoxCategory.DataSource = dataTable;
            comboBoxCategory.DisplayMember = "Name";
            comboBoxCategory.ValueMember = "ID";
        }

        private void LoadAuthors()
        {
            string query = "SELECT DISTINCT Author FROM Books";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            comboBoxBrands.DataSource = dataTable;
            comboBoxBrands.DisplayMember = "Author";
        }

        private void LoadBooks()
        {
            string currentRole = UserManager.CurrentUser.Role;
            if (currentRole != "admin")
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();
            }

            string query = "SELECT ID, Title, Author, Description, CategoryID FROM Books";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchKeyword = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string query = "SELECT ID, Title, Author, Description, CategoryID FROM Books WHERE Title LIKE @SearchKeyword OR Author LIKE @SearchKeyword";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Введите ключевое слово для поиска.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBoxName.Text;
            string author = comboBoxBrands.Text;
            string description = textBoxDescription.Text;
            int categoryID = Convert.ToInt32(comboBoxCategory.SelectedValue);

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            // Если все поля заполнены и цена корректна, добавляем продукт
            AddProduct(title, author, description, categoryID);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int detailID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                EditProductForm editForm = new EditProductForm(detailID);
                editForm.ShowDialog();
                LoadBooks();
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int BookID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
            DeleteProduct(BookID);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadBooks();
            textBoxName.Text = "";
            comboBoxBrands.SelectedIndex = -1;
            textBoxDescription.Text = "";
            comboBoxCategory.SelectedIndex = -1;
        }

        private void AddProduct(string title, string author, string description, int categoryID)
        {
            string query = "INSERT INTO Books (Title, Author, Description, CategoryID) VALUES (@Title, @Author, @Description, @CategoryID)";
            ExecuteNonQuery(query,
                            ("@Title", title),
                            ("@Aythor", author),
                            ("@Description", description),
                            ("@CategoryID", categoryID));
        }

        private void UpdateProduct(int bookID, string title, string author, string description, int categoryID)
        {
            string query = "UPDATE Books SET Title = @Title, Author = @Author,  Description = @Description, CategoryID = @CategoryID WHERE ID = @BookID";
            ExecuteNonQuery(query,
                            ("@Title", title),
                            ("@Author", author),
                            ("@Description", description),
                            ("@CategoryID", categoryID),
                            ("@BookID", bookID));
        }

        private void DeleteProduct(int bookID)
        {
            string query = "DELETE FROM Books WHERE ID = @BookID";
            ExecuteNonQuery(query, ("@BookID", bookID));
        }

        private void ExecuteNonQuery(string query, params (string, object)[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Операция успешно выполнена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }
        }

        private void Products_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.Instance.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
