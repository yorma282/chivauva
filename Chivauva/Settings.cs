using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTech
{
    public partial class Settings : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Settings()
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

        private void button2_Click(object sender, EventArgs e)
        {
            Form fSettings = new Home();
            fSettings.Show();
            fSettings.FormClosed += new FormClosedEventHandler(form_FormClosed);
            this.Hide();
        }
        private void form_FormClosed(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string oldPassword = textBox2.Text;
            string newPassword = textBox3.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=SmartTech.db;Version=3;"))
            {
                conn.Open();

                // Проверка старого пароля
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM users WHERE username=@username AND password=@oldPassword", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@oldPassword", oldPassword); // Здесь лучше использовать хеширование
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 0)
                    {
                        MessageBox.Show("Неверный логин или старый пароль.");
                    }
                    else
                    {
                        // Обновление пароля
                        using (SQLiteCommand updateCmd = new SQLiteCommand("UPDATE users SET password=@newPassword WHERE username=@username", conn))
                        {
                            updateCmd.Parameters.AddWithValue("@username", username);
                            updateCmd.Parameters.AddWithValue("@newPassword", newPassword); // Здесь тоже лучше хешировать
                            updateCmd.ExecuteNonQuery();

                            MessageBox.Show("Пароль успешно изменен.");
                        }
                    }
                }
            }
        }

    }
}
