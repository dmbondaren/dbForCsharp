using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace dbForCsharp
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private MySqlDataAdapter adapter1;
        private MySqlDataAdapter adapter2;
        private DataSet dataSet;
        private DataRow selectedRow;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;database=library;uid=root;pwd=root;";
            connection = new MySqlConnection(connectionString);
            dataSet = new DataSet();

            try
            {
                connection.Open();

                // Load Authors table
                adapter1 = new MySqlDataAdapter("SELECT * FROM authors", connection);
                adapter1.Fill(dataSet, "authors");
                dataGridView1.DataSource = dataSet.Tables["authors"];

                // Load Books table
                adapter2 = new MySqlDataAdapter("SELECT * FROM books", connection);
                adapter2.Fill(dataSet, "books");
                dataGridView2.DataSource = dataSet.Tables["books"];

                // Update functionality
                MySqlCommandBuilder commandBuilder1 = new MySqlCommandBuilder(adapter1);
                MySqlCommandBuilder commandBuilder2 = new MySqlCommandBuilder(adapter2);

                adapter1.UpdateCommand = commandBuilder1.GetUpdateCommand();
                adapter1.InsertCommand = commandBuilder1.GetInsertCommand();
                adapter1.DeleteCommand = commandBuilder1.GetDeleteCommand();

                adapter2.UpdateCommand = commandBuilder2.GetUpdateCommand();
                adapter2.InsertCommand = commandBuilder2.GetInsertCommand();
                adapter2.DeleteCommand = commandBuilder2.GetDeleteCommand();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveChanges(adapter1, "authors");
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveChanges(adapter2, "books");
        }

        private void SaveChanges(MySqlDataAdapter adapter, string tableName)
        {
            try
            {
                adapter.Update(dataSet, tableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Load selected row data into the TextBox for editing
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["a_name"].Value.ToString();
                selectedRow = ((DataRowView)row.DataBoundItem).Row;
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Load selected row data into the TextBox for editing
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.Rows.Count)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                textBox1.Text = row.Cells["b_name"].Value.ToString();
                selectedRow = ((DataRowView)row.DataBoundItem).Row;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (dataGridView2.SelectedCells.Count > 0)
                {
                    int selectedRowIndex = dataGridView2.SelectedCells[0].RowIndex;
                    DataGridViewRow selectedRow = dataGridView2.Rows[selectedRowIndex];
                    int bookId = Convert.ToInt32(selectedRow.Cells["b_id"].Value);
                    string newValue = textBox1.Text;

                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection("server=localhost;database=library;uid=root;pwd=root;"))
                        {
                            conn.Open();

                            string query = "UPDATE books SET b_name = @newValue WHERE b_id = @bookId";

                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@newValue", newValue);
                                cmd.Parameters.AddWithValue("@bookId", bookId);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    selectedRow.Cells["b_name"].Value = newValue;
                                    MessageBox.Show("Значение успешно обновлено.");
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка при обновлении значения.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите строку для редактирования в таблице.");
                }
            }
            else
            {
                MessageBox.Show("Введите новое значение в текстовое поле.");
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Optional: handle text changes if needed
        }
    }
}
