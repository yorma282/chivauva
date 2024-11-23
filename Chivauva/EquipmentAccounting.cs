using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTech
{
    public partial class EquipmentAccounting : Form
    {
        private SQLiteConnection con;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public EquipmentAccounting()
        {
            InitializeComponent();
            CloseLabel.MouseEnter += CloseLabel_MouseEnter;
            CloseLabel.MouseLeave += CloseLabel_MouseLeave;
            panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            panel1.MouseMove += new MouseEventHandler(panel1_MouseMove);
            panel1.MouseUp += new MouseEventHandler(panel1_MouseUp);
        }

        private void CloseLabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Form fEquipmentAccounting = new Home();
            fEquipmentAccounting.Show();
            fEquipmentAccounting.FormClosed += new FormClosedEventHandler(form_FormClosed);
            this.Hide();
        }

        private void form_FormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CloseLabel_MouseLeave(object sender, EventArgs e)
        {
            CloseLabel.BackColor = Color.Transparent;
        }

        private void CloseLabel_MouseEnter(object sender, EventArgs e)
        {
            CloseLabel.BackColor = Color.Red;
        }

        private async void EquipmentAccounting_Load(object sender, EventArgs e)
        {
            con = new SQLiteConnection(database.connectionString);
            try
            {
                await con.OpenAsync();
                if (con.State != ConnectionState.Open)
                {
                    MessageBox.Show("Не удалось открыть соединение с базой данных.");
                    return;
                }
                await LoadingTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}", "Ошибка");
            }
        }

        private async Task LoadingTable()
        {
            dataGridView1.Rows.Clear();
            SQLiteDataReader sqlReader = null;
            SQLiteCommand command = new SQLiteCommand($"SELECT * FROM [{table_equipment.main}]", con);
            List<string[]> data = new List<string[]>();

            try
            {
                sqlReader = (SQLiteDataReader)await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    var row = new string[7];
                    row[0] = sqlReader[$"{table_equipment.id}"].ToString();
                    row[1] = sqlReader[$"{table_equipment.name}"].ToString();
                    row[2] = sqlReader[$"{table_equipment.type}"].ToString();
                    row[3] = sqlReader[$"{table_equipment.purchase_date}"].ToString();
                    row[4] = sqlReader[$"{table_equipment.responsible}"].ToString();
                    row[5] = sqlReader[$"{table_equipment.classroom}"].ToString();
                    row[6] = sqlReader[$"{table_equipment.status}"].ToString();

                    data.Add(row);
                }

                // Добавление данных в DataGridView
                foreach (var s in data)
                {
                    dataGridView1.Rows.Add(s);
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
            }
            finally
            {
                sqlReader?.Close();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Ошибка");
                return;
            }

            var selectedRow = dataGridView1.SelectedRows[0];
            var idToDelete = selectedRow.Cells[0].Value.ToString(); // Предполагается, что ID в первом столбце

            var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SQLiteCommand command = new SQLiteCommand($"DELETE FROM [{table_equipment.main}] WHERE {table_equipment.id} = @id", con);
                command.Parameters.AddWithValue("@id", idToDelete);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Запись успешно удалена.", "Успех");
                    await LoadingTable(); // Обновление таблицы
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка");
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, что все поля заполнены
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                comboBox4.SelectedItem == null ||
                dateTimePicker1.Value == null ||
                comboBox3.SelectedItem == null ||
                comboBox2.SelectedItem == null ||
                comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                return;
            }

            // Сохраняем значения из полей
            var name = textBox2.Text;
            var type = comboBox4.SelectedItem.ToString();
            var purchaseDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var responsible = comboBox3.SelectedItem.ToString();
            var classroom = comboBox2.SelectedItem.ToString();
            var status = comboBox1.SelectedItem.ToString();

            // Формируем SQL-запрос для добавления записи
            SQLiteCommand command = new SQLiteCommand($"INSERT INTO [{table_equipment.main}] " +
                $"({table_equipment.name}, {table_equipment.type}, {table_equipment.purchase_date}, " +
                $"{table_equipment.responsible}, {table_equipment.classroom}, {table_equipment.status}) " +
                "VALUES (@name, @type, @purchase_date, @responsible, @classroom, @status)", con);

            // Добавляем параметры
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@purchase_date", purchaseDate);
            command.Parameters.AddWithValue("@responsible", responsible);
            command.Parameters.AddWithValue("@classroom", classroom);
            command.Parameters.AddWithValue("@status", status);

            try
            {
                // Выполняем запрос
                await command.ExecuteNonQueryAsync();
                MessageBox.Show("Запись успешно добавлена.", "Успех");
                await LoadingTable(); // Обновляем таблицу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
