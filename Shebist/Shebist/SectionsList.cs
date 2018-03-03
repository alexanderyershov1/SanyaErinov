using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shebist
{
    public partial class SectionsList : Form
    {   
        public SectionsList()
        {
                InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
            {Directory.GetCurrentDirectory()}\Words.mdf;Integrated Security=True"))
            {
                connection.Open();
                string createtable = $"CREATE TABLE dbo.[{textBox1.Text}]" +
                    "([Id] INT IDENTITY(1, 1) NOT NULL," +
                    "[Russian] NCHAR (50)," +
                    "[Description] NCHAR (50)," +
                    "[English] NCHAR (50)," +
                    "[Path] NCHAR (50))";

                SqlCommand command = new SqlCommand(createtable, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
