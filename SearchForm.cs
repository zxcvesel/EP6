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
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
        }
        string connectionString = @"Data Source=DESKTOP-IEFJ9UO\BXSTXRD;Initial catalog=!!!Deadpool;Integrated Security=True";
        private void button4_Click(object sender, EventArgs e)
        {
           string bookTitle = textBox1.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("GetReviewsByBookTitle", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@BookTitle", bookTitle);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    adapter.Fill(dataTable);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                    return;
                }

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Товар с указанным названием не найден.");
                }
                else
                {
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
