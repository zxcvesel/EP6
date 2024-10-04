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

namespace MDKCurs.Cat
{
    public partial class EditCategoryForm : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";

        public EditCategoryForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            string query = "SELECT ID, Name FROM Categories";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBoxCategories.DataSource = dataTable;
                comboBoxCategories.DisplayMember = "Name";
                comboBoxCategories.ValueMember = "ID";
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (comboBoxCategories.SelectedValue != null)
            {
                int categoryId = Convert.ToInt32(comboBoxCategories.SelectedValue);
                string newCategoryName = textBoxNewName.Text;

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    string query = "UPDATE Categories SET Name = @Name WHERE ID = @ID";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", newCategoryName);
                        command.Parameters.AddWithValue("@ID", categoryId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Категория успешно обновлена.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Введите новое название категории.");
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию для обновления.");
            }
        }

       
    }
}
