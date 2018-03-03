using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Shebist
{
    class SerForm1
    {
        Form1 form1;
        public SerForm1(Form1 form1)
        {
            this.form1 = form1;
        }

        //сериализация всех элементов формы
        public void serForm1()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            //Сериализация переменных
            string directory = Directory.GetCurrentDirectory();
            QuickSer($"{directory}\\Data\\Section.dat", form1.Section);
            QuickSer($"{directory}\\Data\\count.dat", form1.count);
            QuickSer($"{directory}\\Data\\id.dat", form1.id);
            QuickSer($"{directory}\\Data\\english.dat", form1.english);
            QuickSer($"{directory}\\Data\\path.dat", form1.path);
            QuickSer($"{directory}\\Data\\isBackgroundImageExists.dat", form1.isBackgroundImageExists);
            QuickSer($"{directory}\\Data\\isSoundEnabled.dat", form1.isSoundEnabled);
            QuickSer($"{directory}\\Data\\isForwardBackEnabled.dat", form1.isForwardBackEnabled);
            QuickSer($"{directory}\\Data\\isCounterEnabled.dat", form1.isCounterEnabled);
            QuickSer($"{directory}\\Data\\isSearchEnabled.dat", form1.isSearchEnabled);
            QuickSer($"{directory}\\Data\\isProgressBarEnabled.dat", form1.isProgressBarEnabled); 
            QuickSer($"{directory}\\Data\\isPCMEnabled.dat", form1.isPCMEnabled);

            //Сериализация Form1
            QuickSer($"{directory}\\Data\\Form1Width.dat", form1.Width);
            QuickSer($"{directory}\\Data\\Form1Height.dat", form1.Height);
            QuickSer($"{directory}\\Data\\Form1Location.dat", form1.Location);
            QuickSer($"{directory}\\Data\\backcolor.dat", form1.BackColor);
            QuickSer($"{directory}\\Data\\opacity.dat", form1.Opacity);
            if (form1.isBackgroundImageExists)
                QuickSer($"{directory}\\Data\\backgroundimage.dat", form1.BackgroundImage);
            QuickSer($"{directory}\\Data\\backgroundimagelayout.dat", form1.BackgroundImageLayout);

            //Сериализация настроек элементов
            QuickSer($"{directory}\\Data\\кнопкиВперёдНазадToolStripMenuItemCheckState.dat", form1.кнопкиВперёдназадToolStripMenuItem.CheckState);
            QuickSer($"{directory}\\Data\\режимСменыПоложенияToolStripMenuItemCheckState.dat", form1.режимСменыПоложенияToolStripMenuItem.CheckState);
            QuickSer($"{directory}\\Data\\полосаПрогрессаToolStripMenuItemCheckState.dat", form1.полосаПрогрессаToolStripMenuItem.CheckState);
            QuickSer($"{directory}\\Data\\счётчикСловToolStripMenuItemCheckState.dat", form1.счётчикСловToolStripMenuItem.CheckState);
            QuickSer($"{directory}\\Data\\поискСловаПоНомеруToolStripMenuItemCheckState.dat", form1.поискСловаПоНомеруToolStripMenuItem.CheckState);
            QuickSer($"{directory}\\Data\\озвучкаСловToolStripMenuItemCheckState.dat", form1.озвучкаСловToolStripMenuItem.CheckState);

            //сериализация trackBar1
            QuickSer($"{directory}\\Data\\trackBar1Enabled.dat", form1.trackBar1.Enabled);
            QuickSer($"{directory}\\Data\\trackBar1Visible.dat", form1.trackBar1.Visible);
            QuickSer($"{directory}\\Data\\trackBar1Location.dat", form1.trackBar1.Location);

            //сериализация description_label
            QuickSer($"{directory}\\Data\\description_labelEnabled.dat", form1.description_label.Enabled);
            QuickSer($"{directory}\\Data\\description_labelVisible.dat", form1.description_label.Visible);
            QuickSer($"{directory}\\Data\\description_labelLocation.dat", form1.description_label.Location);
            QuickSer($"{directory}\\Data\\description_labelText.dat", form1.description_label.Text);
            QuickSer($"{directory}\\Data\\description_labelForeColor.dat", form1.description_label.ForeColor);
            QuickSer($"{directory}\\Data\\description_labelFont.dat", form1.description_label.Font);
            
            //сериализация button1
            QuickSer($"{directory}\\Data\\button1BackColor.dat", form1.button1.BackColor);
            QuickSer($"{directory}\\Data\\button1ForeColor.dat", form1.button1.ForeColor);
            QuickSer($"{directory}\\Data\\button1Enabled.dat", form1.button1.Enabled);
            QuickSer($"{directory}\\Data\\button1Visible.dat", form1.button1.Visible);
            QuickSer($"{directory}\\Data\\button1Location.dat", form1.button1.Location);

            //сериализация button2
            QuickSer($"{directory}\\Data\\button2Enabled.dat", form1.button2.Enabled);
            QuickSer($"{directory}\\Data\\button2Visible.dat", form1.button2.Visible);
            QuickSer($"{directory}\\Data\\button2Text.dat", form1.button2.Text);
            QuickSer($"{directory}\\Data\\button2BackColor.dat", form1.button2.BackColor);
            QuickSer($"{directory}\\Data\\button2ForeColor.dat", form1.button2.ForeColor);
            QuickSer($"{directory}\\Data\\button2Location.dat", form1.button2.Location);

            //сериализация button3
            QuickSer($"{directory}\\Data\\button3Enabled.dat", form1.button3.Enabled);
            QuickSer($"{directory}\\Data\\button3Visible.dat", form1.button3.Visible);
            QuickSer($"{directory}\\Data\\button3BackColor.dat", form1.button3.BackColor);
            QuickSer($"{directory}\\Data\\button3ForeColor.dat", form1.button3.ForeColor);
            QuickSer($"{directory}\\Data\\button3Location.dat", form1.button3.Location);

            //сериализация button4
            QuickSer($"{directory}\\Data\\button4Enabled.dat", form1.button4.Enabled);
            QuickSer($"{directory}\\Data\\button4Visible.dat", form1.button4.Visible);
            QuickSer($"{directory}\\Data\\button4BackColor.dat", form1.button4.BackColor);
            QuickSer($"{directory}\\Data\\button4ForeColor.dat", form1.button4.ForeColor);
            QuickSer($"{directory}\\Data\\button4Location.dat", form1.button4.Location);
            //QuickSer($"{directory}\\Data\\button4BackgroundImage.dat", form1.button4.BackgroundImage);
            QuickSer($"{directory}\\Data\\button4BackgroundImageLayout.dat", form1.button5.BackgroundImageLayout);

            //сериализация button5
            QuickSer($"{directory}\\Data\\button5Enabled.dat", form1.button5.Enabled);
            QuickSer($"{directory}\\Data\\button5Visible.dat", form1.button5.Visible);
            QuickSer($"{directory}\\Data\\button5BackColor.dat", form1.button5.BackColor);
            QuickSer($"{directory}\\Data\\button5ForeColor.dat", form1.button5.ForeColor);
            QuickSer($"{directory}\\Data\\button5Location.dat", form1.button5.Location);
            //QuickSer($"{directory}\\Data\\button5BackgroundImage.dat", form1.button5.BackgroundImage);
            QuickSer($"{directory}\\Data\\button5BackgroundImageLayout.dat", form1.button5.BackgroundImageLayout);

            //сериализация textBox1
            QuickSer($"{directory}\\Data\\textBox1Enabled.dat", form1.textBox1.Enabled);
            QuickSer($"{directory}\\Data\\textBox1Visible.dat", form1.textBox1.Visible);
            QuickSer($"{directory}\\Data\\textBox1ForeColor.dat", form1.textBox1.ForeColor);
            QuickSer($"{directory}\\Data\\textBox1BackColor.dat", form1.textBox1.BackColor);
            QuickSer($"{directory}\\Data\\textBox1Location.dat", form1.textBox1.Location);
            QuickSer($"{directory}\\Data\\textBox1Font.dat", form1.textBox1.Font);

            //сериализация textBox2
            QuickSer($"{directory}\\Data\\textBox2Enabled.dat", form1.textBox2.Enabled);
            QuickSer($"{directory}\\Data\\textBox2Visible.dat", form1.textBox2.Visible);
            QuickSer($"{directory}\\Data\\textBox2ForeColor.dat", form1.textBox2.ForeColor);
            QuickSer($"{directory}\\Data\\textBox2BackColor.dat", form1.textBox2.BackColor);
            QuickSer($"{directory}\\Data\\textBox2Location.dat", form1.textBox2.Location);

            //сериализация textBox3
            QuickSer($"{directory}\\Data\\textBox3Enabled.dat", form1.textBox3.Enabled);
            QuickSer($"{directory}\\Data\\textBox3Visible.dat", form1.textBox3.Visible);
            QuickSer($"{directory}\\Data\\textBox3ForeColor.dat", form1.textBox3.ForeColor);
            QuickSer($"{directory}\\Data\\textBox3BackColor.dat", form1.textBox3.BackColor);
            QuickSer($"{directory}\\Data\\textBox3Location.dat", form1.textBox3.Location);

            //сериализация label1
            QuickSer($"{directory}\\Data\\label1Enabled.dat", form1.label1.Enabled);
            QuickSer($"{directory}\\Data\\label1Visible.dat", form1.label1.Visible);
            QuickSer($"{directory}\\Data\\label1Text.dat", form1.label1.Text);
            QuickSer($"{directory}\\Data\\label1Location.dat", form1.label1.Location);
            QuickSer($"{directory}\\Data\\label1ForeColor.dat", form1.label1.ForeColor);
            QuickSer($"{directory}\\Data\\label1Font.dat", form1.label1.Font);

            //Сериализация label4
            QuickSer($"{directory}\\Data\\label4Enabled.dat", form1.label4.Enabled);
            QuickSer($"{directory}\\Data\\label4Visible.dat", form1.label4.Visible);
            QuickSer($"{directory}\\Data\\label4ForeColor.dat", form1.label4.ForeColor);
            QuickSer($"{directory}\\Data\\label4Location.dat", form1.label4.Location);

            //Сериализация label5
            QuickSer($"{directory}\\Data\\label5Enabled.dat", form1.label5.Enabled);
            QuickSer($"{directory}\\Data\\label5Visible.dat", form1.label5.Visible);
            QuickSer($"{directory}\\Data\\label5Text.dat", form1.label5.Text);
            QuickSer($"{directory}\\Data\\label5ForeColor.dat", form1.label5.ForeColor);
            QuickSer($"{directory}\\Data\\label5Location.dat", form1.label5.Location);
            QuickSer($"{directory}\\Data\\label5Location.dat", form1.label5.Location);

            //сериализация progressBar1
            QuickSer($"{directory}\\Data\\progressBar1Enabled.dat", form1.progressBar1.Enabled);
            QuickSer($"{directory}\\Data\\progressBar1Visible.dat", form1.progressBar1.Visible);
            QuickSer($"{directory}\\Data\\progressBar1Maximum.dat", form1.progressBar1.Maximum);
            QuickSer($"{directory}\\Data\\progressBar1Value.dat", form1.progressBar1.Value);
            QuickSer($"{directory}\\Data\\progressBar1Location.dat", form1.progressBar1.Location);

            
        }
        //метод для сокращения кода сериализации
        public void QuickSer(string path, object element)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, element);
            }
        }
    }
}
