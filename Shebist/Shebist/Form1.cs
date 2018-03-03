using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace Shebist
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public int id, count = 0;
        public string connectionString, russian, description, english, path, Section = "";
        public bool isSoundEnabled = true;
        MediaPlayer player = new MediaPlayer();

        private void QueryRussianDescriptionEnglishPath()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Russian, Description, English, Path FROM {Section} where Id = {id}", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        russian = (string)reader.GetValue(0);
                        russian = russian.Trim();
                        label1.Text = russian;
                        description = (string)reader.GetValue(1);
                        description = description.Trim();
                        description_label.Text = description;
                        english = (string)reader.GetValue(2);
                        english.Trim();
                        path = (string)reader.GetValue(3);
                    }
                }
            }
        }

        //Нажатие Enter в поле выбора темы
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                    switch (textBox2.Text.ToLower().Trim())
                    {
                        case "первая":
                            Section = "First";
                            break;
                        default:
                            textBox2.Clear();
                            return;
                    }

                    label4.Enabled = label4.Visible = textBox2.Enabled = textBox2.Visible = false;
                    button2.Enabled = button2.Visible = button3.Enabled = button3.Visible = true;
                    textBox2.Clear();
            }
        }

        //при изменении текста в textBox1
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == english.Trim())
            {
                if (isSoundEnabled)
                {
                    if(File.Exists(Directory.GetCurrentDirectory() + path))
                    {
                        player.Open(new Uri(Directory.GetCurrentDirectory() + path, UriKind.Absolute));
                        player.Play();
                    }
                }
                textBox1.Clear();
                if (id < count)
                {
                    id++;
                    QueryRussianDescriptionEnglishPath();

                    progressBar1.Value = id;
                    label5.Text = id + "/" + count;
                }
                else
                {
                    label1.Text = "Выполнено";
                    description_label.Text = "";
                    progressBar1.Value = id;
                    label5.Text = id + "/" + count;
                }
            }
        }

        //Клик по разделу настройки -> прозрачность в меню
        private void OpacityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar1.Enabled = trackBar1.Visible = button1.Enabled = button1.Visible = true;
        }

        //изменение значения прозрачности 
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            switch (trackBar1.Value)
            {
                case 0:
                    MessageBox.Show("Прозрачность не может быть равна 0 ради вашей же безопасности!");
                    Opacity = 1;
                    break;
                case 5:
                    Opacity = 0.05;
                    break;
                case 10:
                    Opacity = 0.1;
                    break;
                case 15:
                    Opacity = 0.15;
                    break;
                case 20:
                    Opacity = 0.2;
                    break;
                case 25:
                    Opacity = 0.25;
                    break;
                case 30:
                    Opacity = 0.3;
                    break;
                case 35:
                    Opacity = 0.35;
                    break;
                case 40:
                    Opacity = 0.40;
                    break;
                case 45:
                    Opacity = 0.45;
                    break;
                case 50:
                    Opacity = 0.5;
                    break;
                case 55:
                    Opacity = 0.55;
                    break;
                case 60:
                    Opacity = 0.6;
                    break;
                case 65:
                    Opacity = 0.65;
                    break;
                case 70:
                    Opacity = 0.7;
                    break;
                case 75:
                    Opacity = 0.75;
                    break;
                case 80:
                    Opacity = 0.8;
                    break;
                case 85:
                    Opacity = 0.85;
                    break;
                case 90:
                    Opacity = 0.9;
                    break;
                case 100:
                    Opacity = 1;
                    break;
            }
        }

        //клик по кнопке ок для закрытия настройки прозрачности
        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Enabled = trackBar1.Visible = button1.Enabled = button1.Visible = false;
        }

        //клик по кнопке начать/снова
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Enabled = textBox1.Visible = description_label.Enabled = description_label.Visible = true;
            
            if(isSearchEnabled)
            {
                textBox3.Enabled = textBox3.Visible = true;
            }
            
            label1.Enabled = label1.Visible = true;

            if (isCounterEnabled)
            {
                label5.Enabled = label5.Visible = true;
            }

            if (isForwardBackEnabled)
            {
                button4.Enabled = button4.Visible = button5.Enabled = button5.Visible = true;
            }

            if(isProgressBarEnabled)
            {
                progressBar1.Enabled = progressBar1.Visible = true;
            }

            id = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                id = 1;
                SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM {Section}", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        count = (int)reader.GetValue(0);                   
                    }
                }
            }

            QueryRussianDescriptionEnglishPath();

            progressBar1.Value = 1;
            progressBar1.Maximum = count;
            label5.Text =  "1/" + count;
        }

        //нажатие кнопки Enter в поле ввода слова
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                label1.Text = english.Trim();
                description_label.Text = "";
                textBox1.Clear();
                if (isSoundEnabled)
                {
                    if(File.Exists(Directory.GetCurrentDirectory() + path))
                    {
                        player.Open(new Uri(Directory.GetCurrentDirectory() + path, UriKind.Absolute));
                        player.Play();
                    }
                }    
            }
        }

        //изменение текта в поле вывода слова
        private void label1_TextChanged(object sender, EventArgs e)
        {
            button2.Text = "Снова";
        }

        //клик по разделу список тем в меню
        private void SectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SectionsList sl = new SectionsList();
            sl.Show();
        }

        //клик но кнопке к выбору темы
        private void button3_Click(object sender, EventArgs e)
        {

            label4.Enabled = label4.Visible = textBox2.Enabled = textBox2.Visible = true;

            label5.Enabled = label5.Visible = textBox3.Enabled = textBox3.Visible =
            button2.Enabled = button2.Visible = button3.Enabled = button3.Visible =
            button4.Visible = button4.Enabled = button5.Visible = button5.Enabled =
            textBox1.Enabled = textBox1.Visible = label1.Enabled = label1.Visible =
            progressBar1.Enabled = progressBar1.Visible = description_label.Enabled
            = description_label.Visible = false;

            button2.Text = "Начать";
            label4.Text = "Выберите тему";
        }

        //при загрузке
        private void Form1_Load(object sender, EventArgs e)
        {
            DeserForm1 dsf1 = new DeserForm1(this);
            //dsf1.deserForm1();
            
            if (isPCMEnabled == true)
                AddEvents();
            else RemoveEvents();
            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\Words.mdf;Integrated Security=True";
        }

        //при закрытии формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SerForm1 sf1 = new SerForm1(this);
            sf1.serForm1();
        }

        public bool isBackgroundImageExists = false;
        //клик по разделу настройки -> фон -> цвет в меню
        private void ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            BackColor = colorDialog1.Color;
            BackgroundImage = null;
            isBackgroundImageExists = false;
        }

        //клик по разделу настройки -> фон -> картинка в меню
        private void ImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            BackgroundImage = Image.FromFile(openFileDialog1.FileName);
            isBackgroundImageExists = true;
        }

        //клик по кнопке вперёд
        private void button5_Click(object sender, EventArgs e)
        {
            if(id < count)
            {
                id++;
                QueryRussianDescriptionEnglishPath();

                progressBar1.Value = id;
                label5.Text = id + "/" + count;
            }
            else if (id == count)
            {
                id = 1;
                QueryRussianDescriptionEnglishPath();
                
                progressBar1.Value = id;
                label5.Text = id + "/" + count;
            }
        }

        //клик по кнопке назад
        private void button4_Click(object sender, EventArgs e)
        {
            if(id == 1)
            {
                id = count;
                QueryRussianDescriptionEnglishPath();
                progressBar1.Value = id;
                label5.Text = "1/" + count;
            }
            else if(label1.Text == "Выполнено")
            {
                QueryRussianDescriptionEnglishPath();

                textBox1.Enabled = true;
                description_label.Enabled = description_label.Visible = true;
                progressBar1.Value = id;
                label5.Text = id + "/" + count;
            }
            else
            {
                id--;
                QueryRussianDescriptionEnglishPath();
                progressBar1.Value = id;
                label5.Text = id + "/" + count;
            }
            
        }

        //переход к слову по номеру
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    id = Int32.Parse(textBox3.Text);
                    if (id >= 1 && id <= count)
                    {
                        QueryRussianDescriptionEnglishPath();

                        textBox3.Clear();
                        label5.Text = id + "/" + count;
                        progressBar1.Value = id;
                        textBox1.Enabled = true;
                    }
                    else
                    {
                        textBox3.Clear();
                    }
                }
                catch (FormatException)
                {
                    textBox3.Clear();
                }
            }
        }

        private void цветToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            
            button1.BackColor = button2.BackColor
                = button3.BackColor = button4.BackColor = button5.BackColor = colorDialog2.Color;
        }

        private void прозрачныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.BackColor = button2.BackColor
                = button3.BackColor = button4.BackColor = button5.BackColor = System.Drawing.Color.Transparent;
        }
       

        private void цветШрифтаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog3.ShowDialog() == DialogResult.Cancel)
                return;
                textBox1.ForeColor = label1.ForeColor = textBox2.ForeColor =
                textBox3.ForeColor =  label4.ForeColor =
                label5.ForeColor = button1.ForeColor = button2.ForeColor =
                button3.ForeColor = button4.ForeColor =
                button5.ForeColor = description_label.ForeColor = colorDialog3.Color;
        }

        public bool isForwardBackEnabled = true;

        private void цветПолейВводаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog4.ShowDialog() == DialogResult.Cancel)
                return;
            textBox1.BackColor = textBox2.BackColor
                = textBox3.BackColor = colorDialog4.Color;
        }

        public bool isCounterEnabled = true;

        public bool isSearchEnabled = true;

        public bool isProgressBarEnabled = true;

        Point moveStart;
        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void label5_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                label5.Location = new Point(label5.Location.X + deltaPos.X,
                label5.Location.Y + deltaPos.Y);
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                label1.Location = new Point(label1.Location.X + deltaPos.X,
                label1.Location.Y + deltaPos.Y);
            }
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                label4.Location = new Point(label4.Location.X + deltaPos.X,
                label4.Location.Y + deltaPos.Y);
            }
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                button5.Location = new Point(button5.Location.X + deltaPos.X,
                button5.Location.Y + deltaPos.Y);
            }
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                button1.Location = new Point(button1.Location.X + deltaPos.X,
                button1.Location.Y + deltaPos.Y);
            }
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                button2.Location = new Point(button2.Location.X + deltaPos.X,
                button2.Location.Y + deltaPos.Y);
            }
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                button3.Location = new Point(button3.Location.X + deltaPos.X,
                button3.Location.Y + deltaPos.Y);
            }
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                button4.Location = new Point(button4.Location.X + deltaPos.X,
                button4.Location.Y + deltaPos.Y);
            }
        }

        private void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void progressBar1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                progressBar1.Location = new Point(progressBar1.Location.X + deltaPos.X,
                progressBar1.Location.Y + deltaPos.Y);
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void textBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                textBox1.Location = new Point(textBox1.Location.X + deltaPos.X,
                textBox1.Location.Y + deltaPos.Y);
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void textBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                textBox2.Location = new Point(textBox2.Location.X + deltaPos.X,
                textBox2.Location.Y + deltaPos.Y);
            }
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void textBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                textBox3.Location = new Point(textBox3.Location.X + deltaPos.X,
                textBox3.Location.Y + deltaPos.Y);
            }
        }

        private void description_label_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveStart = new Point(e.X, e.Y);
            }
        }

        private void description_label_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - moveStart.X, e.Y - moveStart.Y);
                description_label.Location = new Point(description_label.Location.X + deltaPos.X,
                description_label.Location.Y + deltaPos.Y);
            }
        }

        public bool isPCMEnabled = false;

        private void label1forecolorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (label1forecolorDialog.ShowDialog() == DialogResult.Cancel)
                return;
            label1.ForeColor = label1forecolorDialog.Color;
        }

        private void кнопкиВперёдназадToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
                if (кнопкиВперёдназадToolStripMenuItem.CheckState == CheckState.Checked)
                {
                if (!textBox2.Enabled)
                {
                    if (id != 1)
                        button4.Enabled = true;

                    if (id != count)
                        button5.Enabled = true;
                    button5.Visible = button4.Visible = isForwardBackEnabled = true;
                }
                else if (textBox2.Enabled)
                    isForwardBackEnabled = true;
                }
            else
            {
                if (!textBox2.Enabled)
                {
                    button4.Enabled = button4.Visible = button5.Enabled =
                    button5.Visible = isForwardBackEnabled = false;
                }
                else if (textBox2.Enabled)
                    isForwardBackEnabled = false;
            }
        }

        private void счётчикСловToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (счётчикСловToolStripMenuItem.CheckState == CheckState.Checked)
            {
                if (!textBox2.Enabled)
                    label5.Visible = label5.Enabled = isCounterEnabled = true;
                else if (textBox2.Enabled)
                    isCounterEnabled = true;
            }
            else
            {
                if (!textBox2.Enabled)
                    label5.Visible = label5.Enabled = isCounterEnabled = false;
                else if (textBox2.Enabled)
                    isCounterEnabled = false;
            }
        }

        private void поискСловаПоНомеруToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if(поискСловаПоНомеруToolStripMenuItem.CheckState == CheckState.Checked)
            {
                if (!textBox2.Enabled)
                    textBox3.Enabled = textBox3.Visible = isSearchEnabled = true;
                else if (textBox2.Enabled)
                    isSearchEnabled = true;
            }

            else
            {
                if (!textBox2.Enabled)
                    textBox3.Enabled = textBox3.Visible = isSearchEnabled = false;
                else if (textBox2.Enabled)
                    isSearchEnabled = false;
            }
        }

        private void полосаПрогрессаToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if(полосаПрогрессаToolStripMenuItem.CheckState == CheckState.Checked)
            {
                if (!textBox2.Enabled)
                    progressBar1.Enabled = progressBar1.Visible = isProgressBarEnabled = true;
                else if (textBox2.Enabled)
                    isProgressBarEnabled = true;
            }
            else
            {
                if (!textBox2.Enabled)
                    progressBar1.Enabled = progressBar1.Visible = isProgressBarEnabled = false;
                else if (textBox2.Enabled)
                    isProgressBarEnabled = false;
            }
             
        }

        private void AddEvents()
        {
            description_label.MouseDown += new MouseEventHandler(this.description_label_MouseDown);
            description_label.MouseMove += new MouseEventHandler(this.description_label_MouseMove);
            textBox3.MouseDown += new MouseEventHandler(this.textBox3_MouseDown);
            textBox3.MouseMove += new MouseEventHandler(this.textBox3_MouseMove);
            textBox2.MouseDown += new MouseEventHandler(this.textBox2_MouseDown);
            textBox2.MouseMove += new MouseEventHandler(this.textBox2_MouseMove);
            textBox1.MouseDown += new MouseEventHandler(this.textBox1_MouseDown);
            textBox1.MouseMove += new MouseEventHandler(this.textBox1_MouseMove);
            progressBar1.MouseDown += new MouseEventHandler(this.progressBar1_MouseDown);
            progressBar1.MouseMove += new MouseEventHandler(this.progressBar1_MouseMove);
            label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
            label1.MouseMove += new MouseEventHandler(this.label1_MouseMove);
            label4.MouseDown += new MouseEventHandler(this.label4_MouseDown);
            label4.MouseMove += new MouseEventHandler(this.label4_MouseMove);
            label5.MouseDown += new MouseEventHandler(this.label5_MouseDown);
            label5.MouseMove += new MouseEventHandler(this.label5_MouseMove);
            button1.MouseDown += new MouseEventHandler(this.button1_MouseDown);
            button1.MouseMove += new MouseEventHandler(this.button1_MouseMove);
            button1.Click -= new EventHandler(this.button1_Click);
            button2.MouseDown += new MouseEventHandler(this.button2_MouseDown);
            button2.MouseMove += new MouseEventHandler(this.button2_MouseMove);
            button2.Click -= new EventHandler(this.button2_Click);
            button3.MouseDown += new MouseEventHandler(this.button3_MouseDown);
            button3.MouseMove += new MouseEventHandler(this.button3_MouseMove);
            button3.Click -= new EventHandler(this.button3_Click);
            button4.MouseDown += new MouseEventHandler(this.button4_MouseDown);
            button4.MouseMove += new MouseEventHandler(this.button4_MouseMove);
            button4.Click -= new EventHandler(this.button4_Click);
            button5.MouseDown += new MouseEventHandler(this.button5_MouseDown);
            button5.MouseMove += new MouseEventHandler(this.button5_MouseMove);
            button5.Click -= new EventHandler(this.button5_Click);
        }

        private void режимСменыПоложенияToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (режимСменыПоложенияToolStripMenuItem.CheckState == CheckState.Checked)
            {
                isPCMEnabled = true;
                AddEvents();
            }

            else
            {
                isPCMEnabled = false;
                RemoveEvents();
            }
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImageLayout = ImageLayout.Center;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImageLayout = ImageLayout.None;
        }

        private void stretchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void button5изображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button5openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            button5.BackgroundImage = Image.FromFile(button5openFileDialog.FileName);
        }

        private void button5centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.BackgroundImageLayout = ImageLayout.Center;
        }

        private void button5noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.BackgroundImageLayout = ImageLayout.None;
        }

        private void button5stretchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void button5tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.BackgroundImageLayout = ImageLayout.Tile;
        }

        private void button4изображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button4openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            button4.BackgroundImage = Image.FromFile(button4openFileDialog.FileName);
        }

        private void button4centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.BackgroundImageLayout = ImageLayout.Center;
        }

        private void button4noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.BackgroundImageLayout = ImageLayout.None;
        }

        private void button4stretchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void button4tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.BackgroundImageLayout = ImageLayout.Tile;
        }

        private void button4zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void button5шрифтToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button5fontDialog.ShowDialog() == DialogResult.Cancel)
                return;
            button5.Font = button5fontDialog.Font;
        }

        private void textBox1ForeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1ForeColorDialog.ShowDialog() == DialogResult.Cancel)
                return;
            textBox1.ForeColor = textBox1ForeColorDialog.Color;
            
        }

        private void button5zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImageLayout = ImageLayout.Tile;
        }

        private void textBox1ChangeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSizeForm csf = new ChangeSizeForm(textBox1);
            csf.Show();
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void озвучкаСловToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (озвучкаСловToolStripMenuItem.CheckState == CheckState.Checked)
                isSoundEnabled = true;
            else isSoundEnabled = false;
        }

        private void RemoveEvents()
        {
            description_label.MouseDown -= new MouseEventHandler(this.description_label_MouseDown);
            description_label.MouseMove -= new MouseEventHandler(this.description_label_MouseMove);
            textBox3.MouseDown -= new MouseEventHandler(this.textBox3_MouseDown);
            textBox3.MouseMove -= new MouseEventHandler(this.textBox3_MouseMove);
            textBox2.MouseDown -= new MouseEventHandler(this.textBox2_MouseDown);
            textBox2.MouseMove -= new MouseEventHandler(this.textBox2_MouseMove);
            textBox1.MouseDown -= new MouseEventHandler(this.textBox1_MouseDown);
            textBox1.MouseMove -= new MouseEventHandler(this.textBox1_MouseMove);
            progressBar1.MouseDown -= new MouseEventHandler(this.progressBar1_MouseDown);
            progressBar1.MouseMove -= new MouseEventHandler(this.progressBar1_MouseMove);
            label1.MouseDown -= new MouseEventHandler(this.label1_MouseDown);
            label1.MouseMove -= new MouseEventHandler(this.label1_MouseMove);
            label4.MouseDown -= new MouseEventHandler(this.label4_MouseDown);
            label4.MouseMove -= new MouseEventHandler(this.label4_MouseMove);
            label5.MouseDown -= new MouseEventHandler(this.label5_MouseDown);
            label5.MouseMove -= new MouseEventHandler(this.label5_MouseMove);
            button1.MouseDown -= new MouseEventHandler(this.button1_MouseDown);
            button1.MouseMove -= new MouseEventHandler(this.button1_MouseMove);
            button1.Click += new EventHandler(this.button1_Click);
            button2.MouseDown -= new MouseEventHandler(this.button2_MouseDown);
            button2.MouseMove -= new MouseEventHandler(this.button2_MouseMove);
            button2.Click += new EventHandler(this.button2_Click);
            button3.MouseDown -= new MouseEventHandler(this.button3_MouseDown);
            button3.MouseMove -= new MouseEventHandler(this.button3_MouseMove);
            button3.Click += new EventHandler(this.button3_Click);
            button4.MouseDown -= new MouseEventHandler(this.button4_MouseDown);
            button4.MouseMove -= new MouseEventHandler(this.button4_MouseMove);
            button4.Click += new EventHandler(this.button4_Click);
            button5.MouseDown -= new MouseEventHandler(this.button5_MouseDown);
            button5.MouseMove -= new MouseEventHandler(this.button5_MouseMove);
            button5.Click += new EventHandler(this.button5_Click);
        }

        private void label1fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (label1fontDialog.ShowDialog() == DialogResult.Cancel)
                return;
            label1.Font = label1fontDialog.Font;
            label1.ForeColor = label1fontDialog.Color;
        }

        private void textBox1fonttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1fontDialog.ShowDialog() == DialogResult.Cancel)
                return;
            textBox1.Font = textBox1fontDialog.Font;
            textBox1.ForeColor = textBox1fontDialog.Color;
        }

        private void textBox1backcolorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (textBox1backcolorDialog.ShowDialog() == DialogResult.Cancel)
                return;
            textBox1.BackColor = textBox1backcolorDialog.Color;
        }
    }
}