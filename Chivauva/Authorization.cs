using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SmartTech
{
    public partial class Authorization : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private readonly string connectionString = "Data Source=SmartTech.db;Version=3;";
        public Authorization()
        {
            InitializeComponent();
            Close_Button.MouseEnter += Close_Button_MouseEnter;
            Close_Button.MouseLeave += Close_Button_MouseLeave;
            panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            panel1.MouseMove += new MouseEventHandler(panel1_MouseMove);
            panel1.MouseUp += new MouseEventHandler(panel1_MouseUp);
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Close_Button_MouseEnter(object sender, EventArgs e)
        {
            Close_Button.BackColor = Color.Red;
        }

        private void Close_Button_MouseLeave(object sender, EventArgs e)
        {
            Close_Button.BackColor = Color.Transparent;
        }

        private void form_FormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void EntryButton_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Users WHERE username=@username AND password=@password";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", textBox1.Text);
                    command.Parameters.AddWithValue("@password", textBox2.Text);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            MessageBox.Show("Авторизация прошла успешно");
                            // код для входа обычного пользователя
                            Form fAuthorization = new Home();
                            fAuthorization.Show();
                            fAuthorization.FormClosed += new FormClosedEventHandler(form_FormClosed);
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Логин или пароль были неверны.");
                        }
                    }
                }
            }
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
    }
}
