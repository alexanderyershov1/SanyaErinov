using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Shebist
{
    class DeserForm1
    {
        Form1 form1;
        public DeserForm1(Form1 form1)
        {
            this.form1 = form1;
        }

        //десериализация всех элементов формы
        public void deserForm1()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            //десериализация button4BackgroundImage
            if (File.Exists("\\Data\\button4BackgroundImage.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4BackgroundImage.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.BackgroundImage = (Image)formatter.Deserialize(fs);
                }
            }

            //десериализация button4BackgroundImageLayout
            if (File.Exists("\\Data\\button4BackgroundImageLayout.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4BackgroundImageLayout.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.BackgroundImageLayout = (ImageLayout)formatter.Deserialize(fs);
                }
            }

            //десериализация button5BackgroundImage
            if (File.Exists("\\Data\\button5BackgroundImage.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5BackgroundImage.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.BackgroundImage = (Image)formatter.Deserialize(fs);
                }
            }
            //десериализация button5BackgroundImageLayout
            if (File.Exists("\\Data\\button5BackgroundImageLayout.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5BackgroundImageLayout.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.BackgroundImageLayout = (ImageLayout)formatter.Deserialize(fs);
                }
            }

            //десериализация isBackgroundImageExists
            if (File.Exists("\\Data\\isBackgroundImageExists.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isBackgroundImageExists.dat", FileMode.OpenOrCreate))
                {
                    form1.isBackgroundImageExists = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация Form1.Location
            if (File.Exists("\\Data\\Form1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\Form1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация Form1.Width
            if (File.Exists("\\Data\\Form1Width.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\Form1Width.dat", FileMode.OpenOrCreate))
                {
                    form1.Width = (int)formatter.Deserialize(fs);
                }
            }

            //десериализация Form1.Height
            if (File.Exists("\\Data\\Form1Height.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\Form1Height.dat", FileMode.OpenOrCreate))
                {
                    form1.Height = (int)formatter.Deserialize(fs);
                }
            }

            //десериализация озвучкаСловToolStripMenuItemCheckState
            if (File.Exists("\\Data\\озвучкаСловToolStripMenuItemCheckState.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\озвучкаСловToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.озвучкаСловToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }
            }


            //десериализация режимСменыПоложенияToolStripMenuItemCheckState
            if (File.Exists("\\Data\\режимСменыПоложенияToolStripMenuItemCheckState.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\режимСменыПоложенияToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.режимСменыПоложенияToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }
            }


            //десериализация полосаПрогрессаToolStripMenuItemCheckState
            if (File.Exists("\\Data\\полосаПрогрессаToolStripMenuItemCheckState.dat"))
                using (FileStream fs = new FileStream("\\Data\\полосаПрогрессаToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.полосаПрогрессаToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }


            //десериализация поискСловаПоНомеруToolStripMenuItemCheckState
            if (File.Exists("\\Data\\поискСловаПоНомеруToolStripMenuItemCheckState.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\поискСловаПоНомеруToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.поискСловаПоНомеруToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }
            }

            //десериализация кнопкиВперёдНазадToolStripMenuItemCheckState
            if (File.Exists("\\Data\\кнопкиВперёдНазадToolStripMenuItemCheckState.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\кнопкиВперёдНазадToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.кнопкиВперёдназадToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }
            }

            //десериализация счётчикСловToolStripMenuItemCheckState
            if (File.Exists("\\Data\\счётчикСловToolStripMenuItemCheckState.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\счётчикСловToolStripMenuItemCheckState.dat", FileMode.OpenOrCreate))
                {
                    form1.счётчикСловToolStripMenuItem.CheckState = (CheckState)formatter.Deserialize(fs);
                }
            }


            //десериализация isSoundEnabled
            if (File.Exists("\\Data\\isSoundEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isSoundEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isSoundEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация description_label
            if (File.Exists("\\Data\\description_labelEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\description_labelVisible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelVisible.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\description_labelLocation.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelLocation.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.Location = (Point)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\description_labelForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\description_labelFont.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelFont.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.Font = (Font)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\description_labelText.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\description_labelText.dat", FileMode.OpenOrCreate))
                {
                    form1.description_label.Text = (string)formatter.Deserialize(fs);
                }
            }

            //десериализация count
            if (File.Exists("\\Data\\count.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\count.dat", FileMode.OpenOrCreate))
                {
                    form1.count = (int)formatter.Deserialize(fs);
                }
            }

            //десериализация isPCMEnabled
            if (File.Exists("\\Data\\isPCMEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isPCMEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isPCMEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация isForwardBackEnabled
            if (File.Exists("\\Data\\isForwardBackEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isForwardBackEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isForwardBackEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация isSearchEnabled
            if (File.Exists("\\Data\\isSearchEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isSearchEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isSearchEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация isProgressBarEnabled
            if (File.Exists("\\Data\\isProgressBarEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isProgressBarEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isProgressBarEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация isCounterEnabled
            if (File.Exists("\\Data\\isCounterEnabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\isCounterEnabled.dat", FileMode.OpenOrCreate))
                {
                    form1.isCounterEnabled = (bool)formatter.Deserialize(fs);
                }
            }

            //десериализация Section
            if (File.Exists("\\Data\\Section.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\Section.dat", FileMode.OpenOrCreate))
                {
                    form1.Section = (string)formatter.Deserialize(fs);
                }
            }

            //десериализация id
            if (File.Exists("\\Data\\id.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\id.dat", FileMode.OpenOrCreate))
                {
                    form1.id = (int)formatter.Deserialize(fs);
                }
            }

            //десериализация english
            if (File.Exists("\\Data\\english.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\english.dat", FileMode.OpenOrCreate))
                {
                    form1.english = (string)formatter.Deserialize(fs);
                }
            }

            //десериализация path
            if (File.Exists("\\Data\\path.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\path.dat", FileMode.OpenOrCreate))
                {
                    form1.path = (string)formatter.Deserialize(fs);
                }
            }

            //десериализация backcolor
            if (File.Exists("\\Data\\backcolor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\backcolor.dat", FileMode.OpenOrCreate))
                {
                    form1.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }

            //десериализация opacity
            if (File.Exists("\\Data\\opacity.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\opacity.dat", FileMode.OpenOrCreate))
                {
                    form1.Opacity = (double)formatter.Deserialize(fs);
                }
            }

            //десериализация textBox2
            if (File.Exists("\\Data\\textBox2Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox2Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox2.Enabled = (bool)formatter.Deserialize(fs);
                }
            }

            if (File.Exists("\\Data\\textBox2Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox2Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox2.Visible = (bool)formatter.Deserialize(fs);
                }
            }

            if (File.Exists("\\Data\\textBox2BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox2BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox2.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }

            if (File.Exists("\\Data\\textBox2ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox2ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox2.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }

            if (File.Exists("\\Data\\textBox2Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox2Location.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox2.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация textBox1
            if (File.Exists("\\Data\\textBox1Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox1Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox1BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox1ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.Location = (Point)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox1Font.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox1Font.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox1.Font = (Font)formatter.Deserialize(fs);
                }
            }

            //десериализация textBox3
            if (File.Exists("\\Data\\textBox3Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox3Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox3.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox3Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox3Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox3.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox3BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox3BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox3.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox3ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox3ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox3.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\textBox3Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\textBox3Location.dat", FileMode.OpenOrCreate))
                {
                    form1.textBox3.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация label1
            if (File.Exists("\\Data\\label1Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label1Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label1Text.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1Text.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.Text = (string)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label1ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.Location = (Point)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label1Font.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label1Font.dat", FileMode.OpenOrCreate))
                {
                    form1.label1.Font = (Font)formatter.Deserialize(fs);
                }
            }

            //десериализация label4
            if (File.Exists("\\Data\\label4Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label4Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.label4.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label4Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label4Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.label4.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label4ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label4ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.label4.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label4Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label4Location.dat", FileMode.OpenOrCreate))
                {
                    form1.label4.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация label5
            if (File.Exists("\\Data\\label5Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label5Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.label5.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label5Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label5Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.label5.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label5Text.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label5Text.dat", FileMode.OpenOrCreate))
                {
                    form1.label5.Text = (string)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label5ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label5ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.label5.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\label5Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\label5Location.dat", FileMode.OpenOrCreate))
                {
                    form1.label5.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация trackBar1
            if (File.Exists("\\Data\\trackBar1Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\trackBar1Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.trackBar1.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\trackBar1Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\trackBar1Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.trackBar1.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\trackBar1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\trackBar1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.trackBar1.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация button1
            if (File.Exists("\\Data\\button1Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button1Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.button1.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button1Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button1Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.button1.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button1BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button1BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button1.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button1ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button1ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button1.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.button1.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация button2
            if (File.Exists("\\Data\\button2Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button2Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button2Text.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2Text.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.Text = (string)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button2BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button2ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button2Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button2Location.dat", FileMode.OpenOrCreate))
                {
                    form1.button2.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация button3
            if (File.Exists("\\Data\\button3Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button3Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.button3.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button3Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button3Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.button3.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button3BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button3BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button3.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button3ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button3ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button3.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button3Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button3Location.dat", FileMode.OpenOrCreate))
                {
                    form1.button3.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация button4
            if (File.Exists("\\Data\\button4Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button4Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button4BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button4ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button4Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button4Location.dat", FileMode.OpenOrCreate))
                {
                    form1.button4.Location = (Point)formatter.Deserialize(fs);
                }
            }

            //десериализация button5
            if (File.Exists("\\Data\\button5Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button5Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button5BackColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5BackColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.BackColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button5ForeColor.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5ForeColor.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.ForeColor = (System.Drawing.Color)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\button5Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\button5Location.dat", FileMode.OpenOrCreate))
                {
                    form1.button5.Location = (Point)formatter.Deserialize(fs);
                }
            }


            //десериализация BackgroundImage
            if (File.Exists("\\Data\\backgroundimage.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\backgroundimage.dat", FileMode.OpenOrCreate))
                {
                    form1.BackgroundImage = (Image)formatter.Deserialize(fs);
                }
            }

            //десериализация BackgroundImageLayout
            if (File.Exists("\\Data\\backgroundimagelayout.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\backgroundimagelayout.dat", FileMode.OpenOrCreate))
                {
                    form1.BackgroundImageLayout = (ImageLayout)formatter.Deserialize(fs);
                }
            }

            //десериализация progressBar1
            if (File.Exists("\\Data\\progressBar1Enabled.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\progressBar1Enabled.dat", FileMode.OpenOrCreate))
                {
                    form1.progressBar1.Enabled = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\progressBar1Visible.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\progressBar1Visible.dat", FileMode.OpenOrCreate))
                {
                    form1.progressBar1.Visible = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\progressBar1Maximum.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\progressBar1Maximum.dat", FileMode.OpenOrCreate))
                {
                    form1.progressBar1.Maximum = (int)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\progressBar1Value.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\progressBar1Value.dat", FileMode.OpenOrCreate))
                {
                    form1.progressBar1.Value = (int)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("\\Data\\progressBar1Location.dat"))
            {
                using (FileStream fs = new FileStream("\\Data\\progressBar1Location.dat", FileMode.OpenOrCreate))
                {
                    form1.progressBar1.Location = (Point)formatter.Deserialize(fs);
                }
            }
        }
    }
}
