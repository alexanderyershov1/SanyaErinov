using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shebist
{
    public partial class ChangeSizeForm : Form
    {
        Button button;
        TrackBar trackBar;
        ProgressBar progressBar;
        TextBox textBox;
        public ChangeSizeForm(Button button)
        {
            InitializeComponent();
            this.button = button;
        }

        public ChangeSizeForm(TextBox textBox)
        {
            InitializeComponent();
            this.textBox = textBox;
        }

        public ChangeSizeForm(ProgressBar progressBar)
        {
            InitializeComponent();
            this.progressBar = progressBar;
        }

        public ChangeSizeForm(TrackBar trackBar)
        {
            InitializeComponent();
            this.trackBar = trackBar;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button != null)
                ++button.Height;
            if (progressBar != null)
                ++progressBar.Height;
            if (trackBar != null)
                ++trackBar.Height;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(button != null)
                ++button.Width;
            if (progressBar != null)
                ++progressBar.Width;
            if (textBox != null)
                ++textBox.Width;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button != null)
                --button.Width;
            if (progressBar != null)
                --progressBar.Width;
            if (textBox != null)
                --textBox.Width;
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            this.button3timer.Tick += new System.EventHandler(this.button3timer_Tick);
            button3timer.Start();
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            this.button3timer.Tick -= new System.EventHandler(this.button3timer_Tick);
            button3timer.Stop();
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            this.button2timer.Tick += new System.EventHandler(this.button2timer_Tick);
            button2timer.Start();
        }


        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            this.button2timer.Tick -= new System.EventHandler(this.button2timer_Tick);
            button2timer.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(button != null)
                --button.Height;
            if (progressBar != null)
                --progressBar.Height;

        }

        private void button1timer_Tick(object sender, EventArgs e)
        {
            if (button != null)
                --button.Height;
            if (progressBar != null)
                --progressBar.Height;
        }

        private void button2timer_Tick(object sender, EventArgs e)
        {
            if (button != null)
                --button.Width;
            if (progressBar != null)
                --progressBar.Width;
            if (textBox != null)
                --textBox.Width;
        }

        private void button3timer_Tick(object sender, EventArgs e)
        {
            if (button != null)
                ++button.Width;
            if (progressBar != null)
                ++progressBar.Width;
            if (textBox != null)
                ++textBox.Width;
        }

        private void button4timer_Tick(object sender, EventArgs e)
        {
            if (button != null)
                ++button.Height;
            if (progressBar != null)
                ++progressBar.Height;
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            this.button1timer.Tick += new System.EventHandler(this.button1timer_Tick);
            button1timer.Start();
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            this.button1timer.Tick -= new System.EventHandler(this.button1timer_Tick);
            button1timer.Stop();
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            this.button4timer.Tick += new System.EventHandler(this.button4timer_Tick);
            button4timer.Start();
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            this.button4timer.Tick -= new System.EventHandler(this.button4timer_Tick);
            button4timer.Stop();
        }
    }
}
