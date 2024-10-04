using MDKCurs.Cat;
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

namespace MDKCurs
{
    public partial class Category : Form
    {
        public Category()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private SqlConnection connection;

        private void Category_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            string currentRole = UserManager.CurrentUser.Role;
            if (currentRole != "admin")
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();
            }

            string query = "SELECT ID, Name FROM Categories";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                comboBox1.DataSource = dataTable;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "ID";
            }


            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string categoryName = comboBox1.Text;
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                string query = "INSERT INTO Categories (Name) VALUES (@Name)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", categoryName);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadCategories();
                MessageBox.Show("Категория успешно добавлена.");
            }
            else
            {
                MessageBox.Show("Введите название категории.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (EditCategoryForm editForm = new EditCategoryForm())
            {
                editForm.ShowDialog();
                LoadCategories(); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                int categoryId = Convert.ToInt32(comboBox1.SelectedValue);
                string query = "DELETE FROM Categories WHERE ID = @ID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", categoryId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadCategories();
                MessageBox.Show("Категория успешно удалена.");
            }
            else
            {
                MessageBox.Show("Выберите категорию для удаления.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadCategories();
            comboBox1.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchKeyword = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string query = "SELECT ID, Name FROM Categories WHERE Name LIKE @Name";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@Name", "%" + searchKeyword + "%");
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    comboBox1.DataSource = dataTable;
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "ID";
                }
            }
            else
            {
                MessageBox.Show("Введите ключевое слово для поиска.");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                comboBox1.Text = selectedRow.Cells["Name"].Value.ToString();
            }
        }

        private void Category_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DataRowView selectedRow = comboBox1.SelectedItem as DataRowView;
                comboBox1.Text = selectedRow["Name"].ToString();
            }
        }
    }
}
