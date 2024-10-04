using MDKCurs.Prod;
using MDKCurs.UserFormsss;
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
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
            LoadUsers();
            FillRolesComboBox();
        }
        private string connectionString = @"Data Source=Desktop-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private SqlConnection connection;


        private void FillRolesComboBox()
        {
            List<string> roles = new List<string> { "admin", "customer" }; // Замените этот список на ваши реальные роли
            comboBoxRole.DataSource = roles;
        }


        private void button5_Click(object sender, EventArgs e)
        {
            string searchKeyword = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string query = "SELECT ID, Login, Password, Role FROM Users WHERE Login LIKE @SearchKeyword OR Role LIKE @SearchKeyword";
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
        private void LoadUsers()
        {
            string currentRole = UserManager.CurrentUser.Role;
            if (currentRole != "admin")
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();
            }

            string query = "SELECT ID, Login, Password, Role FROM Users";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox3.Text;
            string role = comboBoxRole.Text;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO Users (Login, Password, Role) VALUES (@Login, @Password, @Role)";
            ExecuteNonQuery(query,
                            ("@Login", login),
                            ("@Password", password),
                            ("@Role", role));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int detailID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                EditUserForm editForm = new EditUserForm(detailID);
                editForm.ShowDialog();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                string query = "DELETE FROM Users WHERE ID = @UserID";
                ExecuteNonQuery(query, ("@UserID", userID));
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadUsers();
            textBox1.Text = "";
            textBox3.Text = "";
            comboBoxRole.Text = "";
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Login"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["Password"].Value.ToString();
                comboBoxRole.Text = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
            }
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
                    LoadUsers(); // Перезагружаем данные после выполнения операции
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }

        }

        private void Users_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }
    }
}
