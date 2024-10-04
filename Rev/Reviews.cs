using MDKCurs.Prod;
using MDKCurs.Rev;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MDKCurs
{
    public partial class Reviews : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private SqlConnection connection;
        public Reviews()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
            LoadReviews();
            LoadUsersIntoComboBox();
            LoadProductsIntoComboBox();
        }

        private void LoadUsersIntoComboBox()
        {
            string query = "SELECT ID, Login FROM Users";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            comboBoxUser.DataSource = dataTable;
            comboBoxUser.DisplayMember = "Login";
            comboBoxUser.ValueMember = "ID";
        }

        private void LoadProductsIntoComboBox()
        {
            string query = "SELECT ID, Title FROM Books";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            comboBoxProduct.DataSource = dataTable;
            comboBoxProduct.DisplayMember = "Title";
            comboBoxProduct.ValueMember = "ID";
        }

        private void LoadReviews()
        {
            

            string query = "SELECT ID, UserID, BookID, Rating, Comment FROM Reviews";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
        }

        private void LoadReviewDetails()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Подгружаем дополнительные данные отзыва
                    int reviewID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                    string query = "SELECT UserID, BookID FROM Reviews WHERE ID = @ReviewID";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReviewID", reviewID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            comboBoxUser.SelectedValue = reader["UserID"];
                            comboBoxProduct.SelectedValue = reader["BookID"];
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке деталей отзыва: " + ex.Message);
            }
            finally
            {
                connection.Close(); // Закрытие подключения
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            LoadReviewDetails();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchKeyword = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string query = "SELECT ID, UserID, ProductID, Rating, Comment FROM Reviews WHERE UserID LIKE @SearchKeyword OR BookID LIKE @SearchKeyword";
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
            try
            {
                int userID = Convert.ToInt32(comboBoxUser.SelectedValue);
                int bookID = Convert.ToInt32(comboBoxProduct.SelectedValue);
                int rating = Convert.ToInt32(numericUpDownRating.Text);
                string comment = textBoxComment.Text;

                // Проверяем, существует ли уже отзыв от этого пользователя на этот продукт
                string checkQuery = "SELECT COUNT(*) FROM Reviews WHERE UserID = @UserID AND BookID = @BookID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserID", userID);
                    checkCommand.Parameters.AddWithValue("@BookID", bookID);

                    connection.Open();
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Вы уже оставили отзыв на этот товар.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string query = "INSERT INTO Reviews (UserID, BookID, Rating, Comment) VALUES (@UserID, @BookID, @Rating, @Comment)";
                ExecuteNonQuery(query,
                                ("@UserID", userID),
                                ("@BookID", bookID),
                                ("@Rating", rating),
                                ("@Comment", comment));

                LoadReviews();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении отзыва: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int detailID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                EditReviewForm editForm = new EditReviewForm(detailID);
                editForm.ShowDialog();
                LoadReviews();
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int reviewID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                    string query = "DELETE FROM Reviews WHERE ID = @ReviewID";
                    ExecuteNonQuery(query, ("@ReviewID", reviewID));

                    LoadReviews(); // Обновляем отображение отзывов
                }
                else
                {
                    MessageBox.Show("Выберите отзыв для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении отзыва: " + ex.Message);
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
                    LoadReviews(); // Перезагружаем данные после выполнения операции
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadReviews();
            comboBoxUser.Text = "";
            comboBoxProduct.Text = "";
            textBoxComment.Text = "";
            numericUpDownRating.Value = 0;
        }

        private void Reviews_Load(object sender, EventArgs e)
        {

        }

        private void Reviews_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
