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

namespace MDKCurs.UserFormsss
{
    public partial class EditUserForm : Form
    {
        private string connectionString = @"Data Source=Desktop-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        public int UserId { get; set; }

        public EditUserForm(int userId)
        {
            InitializeComponent();
            UserId = userId;
            LoadUserDetails();
            FillRolesComboBox();
        }

        private void FillRolesComboBox()
        {
            List<string> roles = new List<string> { "admin", "customer" }; // Замените этот список на ваши реальные роли
            comboBoxRole.DataSource = roles;
        }

        private void LoadUserDetails()
        {
            string query = "SELECT Login, Password, Role FROM Users WHERE ID = @UserID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", UserId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    textBoxLogin.Text = reader["Login"].ToString();
                    textBoxPassword.Text = reader["Password"].ToString();
                    comboBoxRole.SelectedItem = reader["Role"].ToString();
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;
            string role = comboBoxRole.SelectedItem.ToString();

            UpdateUser(login, password, role);
        }

        private void UpdateUser(string login, string password, string role)
        {
            string query = "UPDATE Users SET Login = @Login, Password = @Password, Role = @Role WHERE ID = @UserID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Role", role);
                command.Parameters.AddWithValue("@UserID", UserId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            MessageBox.Show("Данные пользователя успешно обновлены.");
            this.Close();
        }
    }
}
