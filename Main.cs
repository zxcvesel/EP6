using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDKCurs
{
    public partial class Main : Form
    {
        public static Main Instance { get; private set; }
        public Main()
        {
            InitializeComponent();
            Instance = this;
        }

        private void категорииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Category category = new Category();
            this.Hide();
            category.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SearchForm form = new SearchForm();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateOrder form = new CreateOrder();
            form.ShowDialog();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Orders loans = new Orders();
            this.Hide();
            loans.ShowDialog();
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Books books = new Books();
            this.Hide();
            books.ShowDialog();
        }

        private void отзывыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reviews reviews = new Reviews();
            this.Hide();
            reviews.ShowDialog();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            this.Hide();
            users.ShowDialog();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
            string currentRole = UserManager.CurrentUser.Role;
            if (currentRole != "admin")
            {
                пользователиToolStripMenuItem.Enabled = false;
                заказыToolStripMenuItem.Enabled = false;
                

            }
        }

        
        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
