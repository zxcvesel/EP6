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

namespace MDKCurs.Order
{
    public partial class EditOrderForm : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        public int LoanID { get; set; }

        public EditOrderForm(int loanID)
        {
            InitializeComponent();
            LoanID = loanID;
            LoadUsers();
            LoadOrderDetails();
        }

        private void LoadUsers()
        {
            string query = "SELECT ID, Login FROM Users";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBoxUsers.DataSource = dataTable;
                comboBoxUsers.DisplayMember = "Login";
                comboBoxUsers.ValueMember = "ID";
            }
        }

        private void LoadOrderDetails()
        {
            string query = "SELECT UserID, LoanDate, BookID FROM Loans WHERE ID = @LoanID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LoanID", LoanID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    comboBoxUsers.SelectedValue = reader["UserID"];
                    dateTimePickerOrderDate.Value = Convert.ToDateTime(reader["LoanDate"]);
                    textBoxTotalAmount.Text = reader["BookID"].ToString();
                }
                reader.Close();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedValue != null)
            {
                int userId = Convert.ToInt32(comboBoxUsers.SelectedValue);
                DateTime loanDate = dateTimePickerOrderDate.Value;
                decimal bookID = Convert.ToDecimal(textBoxTotalAmount.Text);

                string query = "UPDATE Loads SET UserID = @UserID, LoanDate = @LoanDate, BookId = @BookID WHERE ID = @ID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@LoanDate", loanDate);
                    command.Parameters.AddWithValue("@BookID", bookID);
                    command.Parameters.AddWithValue("@ID", LoanID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Заказ успешно обновлен.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для обновления.");
            }
        }
    }
}
