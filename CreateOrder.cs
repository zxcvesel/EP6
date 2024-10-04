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
    public partial class CreateOrder : Form
    {
        string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";

        public CreateOrder()
        {
            InitializeComponent();
            LoadUsers();
            LoadBooks();
        }

        private void LoadUsers()
        {
            string query = "SELECT ID, Login FROM Users";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBoxUser.DataSource = dataTable;
                comboBoxUser.DisplayMember = "Login";
                comboBoxUser.ValueMember = "ID";
            }
        }

        private void LoadBooks()
        {
            string query = "SELECT ID, Title FROM Books";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBoxProduct.DataSource = dataTable;
                comboBoxProduct.DisplayMember = "Title";
                comboBoxProduct.ValueMember = "ID";
            }
        }
        


        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(comboBoxUser.Text) || string.IsNullOrWhiteSpace(comboBoxProduct.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int userID = Convert.ToInt32(comboBoxUser.SelectedValue);
            DateTime loanDate = dateTimePicker1.Value;
            int bookID = Convert.ToInt32(comboBoxProduct.SelectedValue); 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("AddLoan", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@LoanDate", loanDate);
                command.Parameters.AddWithValue("@BookID", bookID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Заказ успешно добавлен.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении заказа: " + ex.Message);
                }
            }
        }

        
    }
}

            
