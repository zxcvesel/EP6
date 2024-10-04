using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MDKCurs
{
    public partial class Form1 : Form
    {
        private bool isRegistering = false;
        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            textBox3.PasswordChar = '*';
            label1.Visible = false;
            textBox3.Visible = false;
            buttonRegister.Visible = false;
        }

        string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Role FROM users WHERE Login = @Login and Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                object role = command.ExecuteScalar();
                if (role != null)
                {
                    string userRole = (string)role;
                    UserManager.SetCurrentUser(login, userRole);

                    Main form2 = new Main();
                    form2.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ToggleRegistrationMode(!isRegistering);
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string confirmPassword = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO users (Login, Password, Role) VALUES (@Login, @Password, 'customer')";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Регистрация успешна. Теперь вы можете войти.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ToggleRegistrationMode(false);
                    }
                    else
                    {
                        MessageBox.Show("Регистрация не удалась.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при регистрации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToggleRegistrationMode(bool isRegistering)
        {
            this.isRegistering = isRegistering;

            textBox3.Visible = isRegistering;
            buttonRegister.Visible = isRegistering;
            button1.Visible = !isRegistering;
            label1.Visible = isRegistering;
            textBox2.PasswordChar = isRegistering ? '\0' : '*';
            textBox3.PasswordChar = isRegistering ? '\0' : '*';

            if (!isRegistering)
            {
                // Reset form fields
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }
    }
}
