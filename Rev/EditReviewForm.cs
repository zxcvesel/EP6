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

namespace MDKCurs.Rev
{
    public partial class EditReviewForm : Form
    {
        private string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        public int ReviewId { get; set; }

        public EditReviewForm(int reviewId)
        {
            InitializeComponent();
            ReviewId = reviewId;
            LoadUsers();
            LoadProducts();
            LoadReviewDetails();
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

        private void LoadProducts()
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

        private void LoadReviewDetails()
        {
            string query = "SELECT UserID, BookID, Rating, Comment FROM Reviews WHERE ID = @ReviewID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ReviewID", ReviewId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    comboBoxUser.SelectedValue = reader["UserID"];
                    comboBoxProduct.SelectedValue = reader["BookID"];
                    numericUpDownRating.Value = Convert.ToInt32(reader["Rating"]);
                    textBoxComment.Text = reader["Comment"].ToString();
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(comboBoxUser.SelectedValue);
            int bookID = Convert.ToInt32(comboBoxProduct.SelectedValue);
            int rating = Convert.ToInt32(numericUpDownRating.Value);
            string comment = textBoxComment.Text;

            UpdateReview(userID, bookID, rating, comment);
        }

        private void UpdateReview(int userID, int bookID, int rating, string comment)
        {
            string query = "UPDATE Reviews SET UserID = @UserID, BookID = @BookID, Rating = @Rating, Comment = @Comment WHERE ID = @ReviewID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@BookID", bookID);
                command.Parameters.AddWithValue("@Rating", rating);
                command.Parameters.AddWithValue("@Comment", comment);
                command.Parameters.AddWithValue("@ReviewID", ReviewId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            MessageBox.Show("Отзыв успешно обновлен.");
            this.Close();
        }
    }
}
