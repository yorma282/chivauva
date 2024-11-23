using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTech
{
    public partial class Home : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Home()
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

        private void CloseLabel_MouseEnter(object sender, EventArgs e)
        {
            CloseLabel.BackColor = Color.Red;
        }

        private void CloseLabel_MouseLeave(object sender, EventArgs e)
        {
            CloseLabel.BackColor = Color.Transparent;
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

        private void UchetButton_Click(object sender, EventArgs e)
        {
            Form fHome = new EquipmentAccounting();
            fHome.Show();
            fHome.FormClosed += new FormClosedEventHandler(form_FormClosed);
            this.Hide();
        }
        private void form_FormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void CreateExcelReport()
        {
            string connectionString = "Data Source=SmartTech.db; Version=3;"; // Замените на путь к вашей базе данных
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM equipment"; // Вам может понадобиться указать конкретные поля
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Создаем диалог для выбора пути сохранения файла
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Сохранить отчет";
                    saveFileDialog.FileName = "EquipmentReport.xlsx"; // Имя файла по умолчанию

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (ExcelPackage excel = new ExcelPackage())
                        {
                            var workSheet = excel.Workbook.Worksheets.Add("Equipment Report");
                            workSheet.Cells[1, 1].LoadFromDataTable(dataTable, true);

                            // Задайте стиль, если необходимо
                            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                            // Сохраняем файл по выбранному пути
                            FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                            excel.SaveAs(excelFile);
                            MessageBox.Show($"Отчет успешно создан: {saveFileDialog.FileName}");
                        }
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            CreateExcelReport();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form fHome = new Settings();
            fHome.Show();
            fHome.FormClosed += new FormClosedEventHandler(form_FormClosed);
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Путь к документу
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Helps.docx"); // Полный путь к файлу

                // Проверка существования файла
                if (File.Exists(filePath))
                {
                    // Открытие документа
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    MessageBox.Show("Файл не найден: " + filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть документ: " + ex.Message);
            }
        }

    }
}
