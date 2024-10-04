using MDKCurs.Order;
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
    public partial class Orders : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private SqlConnection connection;
        public Orders()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            LoadLoans();
            LoadUsers();
            // Устанавливаем формат даты для txtOrderDate
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
        }

        private void LoadUsers()
        {
            string query = "SELECT ID, Login FROM Users";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            comboBoxUsers.DataSource = dataTable;
            comboBoxUsers.DisplayMember = "Login";
            comboBoxUsers.ValueMember = "ID";
        }

        private void LoadLoans()
        {
            string currentRole = UserManager.CurrentUser.Role;
            if (currentRole != "admin")
            {
                button2.Hide();
                button3.Hide();
            }

            string query = "SELECT ID, UserID, LoanDate, BookID FROM Loans";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dtg.DataSource = dataTable;
        }

        private int GetSelectedUserID()
        {
            if (dtg.SelectedRows.Count > 0)
            {
                int loanID = Convert.ToInt32(dtg.SelectedRows[0].Cells["ID"].Value);

                // Запрос для получения UserID по выбранному заказу
                string query = "SELECT UserID FROM Loans WHERE ID = @LoanID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LoanID", loanID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Если результат не равен null, преобразуем его в int и возвращаем
                            return Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при выполнении SQL-запроса: " + ex.Message);
                    }
                }
            }

            return -1;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedValue != null)
            {
                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int userId = Convert.ToInt32(comboBoxUsers.SelectedValue);
                DateTime loanID = dateTimePicker1.Value;

                // Проверка корректности суммы заказа
                if (!decimal.TryParse(textBox3.Text, out decimal bookID))
                {
                    MessageBox.Show("Пожалуйста, введите корректную сумму заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "INSERT INTO Orders (UserID, LoanDate, BookID) VALUES (@UserID, @LoanDate, @BookID)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@LoanDate", loanID);
                command.Parameters.AddWithValue("@BookID", bookID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Заказ успешно добавлен.");
                    LoadLoans();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении заказа: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для добавления заказа.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dtg.SelectedRows.Count > 0)
            {
                int loanID = Convert.ToInt32(dtg.SelectedRows[0].Cells["ID"].Value);
                EditOrderForm editForm = new EditOrderForm(loanID);
                editForm.ShowDialog();
                LoadLoans();
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dtg.SelectedRows.Count > 0)
            {
                int loanID = Convert.ToInt32(dtg.SelectedRows[0].Cells["ID"].Value);

                string query = "DELETE FROM Loans WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", loanID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Заказ успешно удален.");
                    LoadLoans();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении заказа: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadLoans();
            comboBoxUsers.SelectedIndex = -1;
            textBox3.Text = "";
        }

        private void dtg_SelectionChanged(object sender, EventArgs e)
        {
            if (dtg.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dtg.SelectedRows[0];
                comboBoxUsers.SelectedValue = selectedRow.Cells["UserID"].Value;
                dateTimePicker1.Value = Convert.ToDateTime(selectedRow.Cells["LoanDate"].Value);
                textBox3.Text = selectedRow.Cells["BookID"].Value.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchKeyword = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                // Ваш SQL-запрос для поиска заказов по ключевому слову
                string query = "SELECT ID, UserID, LoanDate, BookID FROM Loans WHERE UserID LIKE @SearchKeyword";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dtg.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Введите ключевое слово для поиска.");
            }
        }

        private void Orders_ForeColorChanged(object sender, EventArgs e)
        {

        }

        private void Orders_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }
    }
}
