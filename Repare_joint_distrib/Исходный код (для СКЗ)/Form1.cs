using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace Вышлифовка
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public double pipeDiameter = 1;//диаметр трубы в мм 
        bool isDiscontinuity = true;
        public List<GrindingZone> grindingZones = new List<GrindingZone>();
        List<Defect> defects = new List<Defect>();//список дефектов
        Report report=new Report();//отчет
        private void discontinuity_Click(object sender, EventArgs e)//внутренний дефект
        {
            discontinuity.BackColor = Color.Green;
            bias.BackColor = Color.Empty;
            isDiscontinuity = true;
        }
        private void bias_Click(object sender, EventArgs e)//смещение
        {
            bias.BackColor = Color.Green;
            discontinuity.BackColor = Color.Empty;
            isDiscontinuity = false;
        }
        private void addDefect_Click(object sender, EventArgs e)
        {
            Defect newDef = new Defect();
            newDef.defectLength = Convert.ToDouble(defectLength.Text);
            newDef.start = Convert.ToDouble(defectStart.Text);
            newDef.isDiscontinuity = isDiscontinuity;
            defects.Add(newDef);
            discontinuity.ForeColor = Color.Empty;
            bias.ForeColor = Color.Empty;
            numberOfDefects.Text = Convert.ToString(defects.Count);
            richTextBox1.Clear();
            richTextBox2.Clear();
            calcZones();

        }
        private void OpenDefectsFile_Click(object sender, EventArgs e)
        {
            string path = @"C:\\Result";//проверяем есть ли такая папка. Если нет - то создаём

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            string fileName;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                this.Text = String.Concat("Joint Repare - ", fileName);
                defects.Clear();
                report = get_report_from_file(fileName);
                if (report.pipeDiameter!=0)
                {
                    diameterTextBox.Text = Convert.ToString(report.pipeDiameter);
                }
                defects = getDefectsFromReport(report);
                discontinuity.ForeColor = Color.Empty;
                bias.ForeColor = Color.Empty;
                numberOfDefects.Text = Convert.ToString(defects.Count);
                richTextBox1.Clear();
                richTextBox2.Clear();
                calcZones();
                //exportToExcel(grindingZones);
            }
        }
        private void cleanDefects_Click(object sender, EventArgs e)
        {
            defects.Clear();
            numberOfDefects.Clear();
            richTextBox1.Clear();
            richTextBox2.Clear();
        }
       

        public void sss(string ST)
        {
            int nWidth = 50, nHeight = 50;
            Pen redPen = new Pen(Color.Red, 8);
            Bitmap b1 = new Bitmap(nWidth, nHeight);
            using (Graphics g1 = Graphics.FromImage(b1))
            {
                //рисуем
                //g1.DrawRectangle(redPen, 0, 0, nWidth, nHeight);
            }
            pictureBox2.Image = b1;
            b1.Save(ST+"test.bmp");

        }


        private void button1_Click(object sender, EventArgs e)
        {
            grindingZones.Clear();
            discontinuity.ForeColor = Color.Empty;
            bias.ForeColor = Color.Empty;
            numberOfDefects.Text = Convert.ToString(defects.Count);
            richTextBox1.Clear();
            richTextBox2.Clear();
            calcZones();
        }

        private void saveToExcel_Click(object sender, EventArgs e)
        {
            exportToExcel(grindingZones, report);
        }

        private void open_folder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", @"C:\Result");
        }
    }
}