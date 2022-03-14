using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Вышлифовка
{
    public class GrindingZone
    {
        public double grindingStart;
        public double grindingEnd;
        public int endID;//номер последнего дефекта, попавшего в вышлифовку
    }

    public partial class Form1 : Form
    {
        private GrindingZone GetOneGrindingZone(List<Defect> defects, int startPoint, double grindingDiam)//получаем зону вышлифовки, отложенную от дефекта с заданным номером
        {
            GrindingZone zone = new GrindingZone();
            zone.grindingStart = defects[startPoint].start;

            if (defects[startPoint].defectLength < grindingDiam)
            {
                zone.grindingEnd = defects[startPoint].start + grindingDiam;
                zone.endID = startPoint;
                //richTextBox1.AppendText(Environment.NewLine + grindingZones.Count+ " Случай 1: " + startPoint);
            }
            else
            {
                zone.grindingEnd = defects[startPoint].start + defects[startPoint].defectLength;
                zone.endID = startPoint;
                //richTextBox1.AppendText(Environment.NewLine + grindingZones.Count + " Случай 2: " + startPoint);
            }
            bool isOne = true;
            if (startPoint < defects.Count)
            {
                int j = zone.endID + 1;
                if (j < defects.Count)
                {
                    while (j < defects.Count)
                    {
                        if (defects[j].start + defects[j].defectLength > zone.grindingEnd & defects[j].start <= zone.grindingEnd + grindingDiam)
                        {
                            zone.grindingEnd = defects[j].start + defects[j].defectLength;
                            zone.endID = j;
                            isOne = false;
                            //richTextBox1.AppendText(Environment.NewLine + grindingZones.Count + " Случай 3: " + j);
                        }
                        j++;
                    }
                }
            }
            if (startPoint + 1 <= defects.Count & isOne/*& defects[startPoint].isDiscontinuity==true*/& zone.grindingStart - (grindingDiam / 2 - defects[startPoint].defectLength / 2) - defects[startPoint].start < 0)//defects[startPoint + 1].start-(defects[startPoint].start+ defects[startPoint].defectLength
            {
                double length = defects[startPoint].defectLength;
                zone.grindingStart -= (grindingDiam / 2 - length / 2);
                zone.grindingEnd -= (grindingDiam / 2 - length / 2);
            }

            return zone;
        }
        private void printZones(List<GrindingZone> grindingZones)
        {
            double summ = 0;
            for (int i = 0; i < grindingZones.Count; i++)
            {
                richTextBox1.AppendText(Environment.NewLine + " Уч. № " + Convert.ToString(i + 1) + ". (" + grindingZones[i].grindingStart + " мм -  " + grindingZones[i].grindingEnd + " мм). Протяженность: " + Convert.ToString(grindingZones[i].grindingEnd - grindingZones[i].grindingStart) + " мм.");
                summ += grindingZones[i].grindingEnd - grindingZones[i].grindingStart;
            }
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.AppendText(Environment.NewLine + "Количество участков, подлежащих сквозной выборке:  " + grindingZones.Count + ".");
            richTextBox1.AppendText(Environment.NewLine + "Протяженность зон сквозной выборки: " + Convert.ToString(summ) + " мм.");
        }
        private void calcZones()
        {
            List<Defect> increaseDef = increaseBiases(defects);
            grindingZones = GetGrindingZones(increaseDef);//список зон вышлифовки
            grindingZones = deleteBadGrindingZones(increaseDef, grindingZones);
            printZones(grindingZones);
            printDefects(defects);
            printPipe(defects, grindingZones, Convert.ToInt16(diameterTextBox.Text));
        }
        private void printPipe(List<Defect> defects, List<GrindingZone> grindingZones, double diameter)//Вывод на экран одного сварного соединения.
        {
            float gWidht = (float)pictureBox2.Width;
            float gHeight = (float)pictureBox2.Height;
            float halfgWidht = (float)pictureBox2.Width / 2;
            float halfgHeight = pictureBox2.Height / 2;
            float minim = Math.Min(gWidht, gHeight);

            Bitmap b1 = new Bitmap(Convert.ToInt32(Math.Round(minim)), Convert.ToInt32(Math.Round(minim)));
            using (Graphics g = Graphics.FromImage(b1))
            {
                
                g.Clear(Color.White);
                // Создаем объекты-кисти для закрашивания фигур
                SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
                SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
                SolidBrush myTrub = new SolidBrush(Color.DeepPink);
                SolidBrush mySeа = new SolidBrush(Color.Blue);
                float penWidht = 7;

                Pen myWindYellow = new Pen(Color.Yellow, penWidht);
                
                Pen myWindGray = new Pen(Color.Aqua, 3);

                Pen myWindBlack = new Pen(Color.Black, 20);
                
                Pen myWindBlue = new Pen(Color.Blue, penWidht);
                
                Pen myWindRed = new Pen(Color.Lime, penWidht);
                
                Pen myWindGreen = new Pen(Color.Red, penWidht);
               
                //запрашиваем ширину и высоту окна с графикой
                
                float xCoeff = 1500 / minim;
                int startAngle = 0;
                int sweepAngle = 360;
                int delta = 8;
                int a0 = Convert.ToInt32(Math.Round(minim)) - 20;
                int x1 = Convert.ToInt32(Math.Round((Convert.ToDouble(minim) - Convert.ToDouble(a0)) / 2));
                int y1 = Convert.ToInt32(Math.Round((Convert.ToDouble(minim) - Convert.ToDouble(a0)) / 2));
                int x2 = x1 + delta;
                int y2 = y1 + delta;
                int x3 = x2 + delta;
                int y3 = y2 + delta;
                int x4 = x3 + delta;
                int y4 = y3 + delta;
                int x5 = x4 + delta;
                int y5 = y4 + delta;

                int width1 = a0 - 2 * delta;
                int height1 = a0 - 2 * delta;

                int width2 = a0 - 4 * delta;
                int height2 = a0 - 4 * delta;

                int width3 = a0 - 6 * delta;
                int height3 = a0 - 6 * delta;

                int width4 = a0 - 8 * delta;
                int height4 = a0 - 8 * delta;

                int width5 = a0 - 10 * delta;
                int height5 = a0 - 10 * delta;

                g.Clear(Color.White);
                g.DrawArc(myWindGray, x4, y4, width4, height4, startAngle, sweepAngle); //рисуем дугу
                bool isNoClockWise = false;

                if (counterClockWise.Checked)
                {
                    isNoClockWise = true;
                }

                Angles angles = new Angles();
                for (int i = 0; i < grindingZones.Count; i++)
                {
                    angles = GetAngles3(grindingZones[i].grindingStart, grindingZones[i].grindingEnd, Convert.ToDouble(diameterTextBox.Text), isNoClockWise);
                    g.DrawArc(myWindRed, x3, y3, width3, height3, angles.startAngle, angles.stopAngle - angles.startAngle); //рисуем дугу                
                }

                for (int i = 0; i < defects.Count; i++)
                {
                    if (defects[i].isDiscontinuity)
                    {
                        angles = GetAngles3(defects[i].start, defects[i].start + defects[i].defectLength, Convert.ToDouble(diameterTextBox.Text), isNoClockWise);
                        g.DrawArc(myWindGreen, x4, y4, width4, height4, angles.startAngle, angles.stopAngle - angles.startAngle); //рисуем дугу
                    }
                    else
                    {
                        angles = GetAngles3(defects[i].start, defects[i].start + defects[i].defectLength, Convert.ToDouble(diameterTextBox.Text), isNoClockWise);
                        g.DrawArc(myWindBlue, x4, y4, width4, height4, angles.startAngle, angles.stopAngle - angles.startAngle); //рисуем дугу
                    }
                }

                for (int i = 0; i < 12; i++)//Добавляем часовые метки
                {
                    g.DrawArc(myWindBlack, x5, y5, width5, height5, 30 * i, 1); //рисуем дугу
                }

                g.DrawLine(myWindRed, halfgWidht / 2, halfgHeight, halfgWidht / 2 + 50, halfgHeight);
                g.DrawString("Сквозная выборка", new Font("Arial", 12), Brushes.Black, halfgWidht / 2 + 55, halfgHeight - 15);


                g.DrawLine(myWindGreen, halfgWidht / 2, halfgHeight + 30, halfgWidht / 2 + 50, halfgHeight + 30);
                g.DrawString("Внутренний дефект", new Font("Arial", 12), Brushes.Black, halfgWidht / 2 + 55, halfgHeight + 15);

                g.DrawLine(myWindBlue, halfgWidht / 2, halfgHeight + 60, halfgWidht / 2 + 50, halfgHeight + 60);
                g.DrawString("Смещение", new Font("Arial", 12), Brushes.Black, halfgWidht / 2 + 55, halfgHeight + 45);

                if (counterClockWise.Checked)
                {
                    g.DrawString("Обход против часовой стрелки", new Font("Arial", 12), Brushes.Black, halfgWidht / 2 + 55, halfgHeight - 45);
                }
                else
                {
                    g.DrawString("Обход по часовой стрелке", new Font("Arial", 12), Brushes.Black, halfgWidht / 2 + 55, halfgHeight - 45);
                }
            }
            pictureBox2.Image = b1;

            b1.Save(@"C:\\Result\joint.BMP");
        }
        private List<GrindingZone> deleteBadGrindingZones(List<Defect> defects, List<GrindingZone> input)
        {
            List<GrindingZone> result= new List<GrindingZone>();                       
            double perimeter = Math.Round(Convert.ToDouble(diameterTextBox.Text) * Math.PI);
            if (perimeter+ input[0].grindingStart< input[input.Count - 1].grindingEnd+ Convert.ToDouble(grinderDiameter.Text))
            {
                input[0].grindingStart = input[input.Count - 1].grindingStart-perimeter;
                
                /*double fin = 0;
                for (int i = 0; i < defects.Count; i++)
                {
                    if (defects[i].start+ defects[i].defectLength< input[0].grindingEnd)
                    {
                        fin = defects[i].start + defects[i].defectLength;
                    }
                    if (fin>0)
                    {
                        input[0].grindingEnd = fin;
                    }
                }*/

                for (int i = 0; i < input.Count - 1; i++)
                {
                    result.Add(input[i]);
                }
            }
            else
            {
                for (int i = 0; i < input.Count; i++)
                {
                    result.Add(input[i]);
                }
            }
            return result;
        }
        private List<GrindingZone> GetGrindingZones(List<Defect> defects)
        {
            defects = defects.OrderBy(i => i.start).ToList();
            List<GrindingZone> result = new List<GrindingZone>();

            int f = 0;
            GrindingZone zone = GetOneGrindingZone(defects, 0, Convert.ToDouble(grinderDiameter.Text));
            result.Add(zone);
            int end = result[0].endID;
            while (end < defects.Count - 1)
            {
                zone = GetOneGrindingZone(defects, end + 1, Convert.ToDouble(grinderDiameter.Text));
                if (result.Count == 0)
                {
                    result.Add(zone);
                }
                else if (result[result.Count - 1].grindingStart != zone.grindingStart & result[result.Count - 1].grindingEnd < zone.grindingStart)
                {
                    result.Add(zone);
                }

                end = zone.endID;
                f++;
            }
            return result;
        }
        private void exportToExcel(List<GrindingZone> grindingZone, Report report)//Создание Excel файла
        {
            report = notNullItems(report);


            double perimeter = Math.Round(Convert.ToDouble(diameterTextBox.Text) * Math.PI);
            object Nothing = System.Reflection.Missing.Value;
            var app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workBook = app.Workbooks.Add(Nothing);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Sheets[1];
            worksheet.Name = "Result";
            // Write data
            worksheet.Cells[1, 2] = "Автоматический расчет участков сквозной выборки";
            worksheet.Cells[1, 2].Font.Bold = true;
            worksheet.Cells[1, 2].Font.Size = 14;

            Microsoft.Office.Interop.Excel.Range excelCells1 = (Microsoft.Office.Interop.Excel.Range)worksheet.get_Range("B2", "G2").Cells;
            // Производим объединение
            excelCells1.Merge(Type.Missing);

            worksheet.Cells[2, 2].RowHeight = worksheet.Cells[2, 2].RowHeight * 4;

            worksheet.Cells[2, 2] = report.note;

            worksheet.Cells[3, 2] = "Номер протокола";
            worksheet.Cells[4, 2] = "Объект";
            worksheet.Cells[5, 2] = "Диаметр";
            worksheet.Cells[6, 2] = "Дата контроля";
            worksheet.Cells[7, 2] = "Толщина";
            worksheet.Cells[8, 2] = "Номер стыка";
            worksheet.Cells[9, 2] = "Суммарная протяженность дефектов";
            double summDefectsLength = 0;
            for (int i = 0; i < defects.Count; i++)
            {
                summDefectsLength += defects[i].defectLength;
            }

            worksheet.Cells[3, 6] = report.protocolNumber;
            worksheet.Cells[4, 6] = report.productName;
            worksheet.Cells[5, 6] = report.pipeDiameter;
            worksheet.Cells[6, 6] = report.date;
            worksheet.Cells[7, 6] = report.thekness;
            worksheet.Cells[8, 6] = report.jointName;
            worksheet.Cells[5, 7] = " мм";
            worksheet.Cells[7, 7] = " мм";
            worksheet.Cells[9, 6] = summDefectsLength;
            worksheet.Cells[9, 7] = " мм";


            int strNunber = 33;//номер строки в формируемой таблице
            worksheet.Cells[strNunber-1, 2] = "№ уч.";
            worksheet.Cells[strNunber - 1, 3] = "Начало";
            worksheet.Cells[strNunber - 1, 4] = "Конец";
            worksheet.Cells[strNunber - 1, 5] = "Протяженность";
            worksheet.Cells[strNunber - 1, 2].Font.Bold = true;
            worksheet.Cells[strNunber - 1, 3].Font.Bold = true;
            worksheet.Cells[strNunber - 1, 4].Font.Bold = true;
            worksheet.Cells[strNunber - 1, 5].Font.Bold = true;
            
            for (int i = 0; i < grindingZone.Count; i++)
            {
                //double pipeNumber = grindingZone[i].grindingStart;//начало текущего участка
                //worksheet.Cells[strNunber, 1] = "";//N_OSOB
                worksheet.Cells[strNunber, 2] = String.Concat("Участок ", i+1);//N_SEK
                worksheet.Cells[strNunber, 3] = grindingZone[i].grindingStart;//L_ODOM
                worksheet.Cells[strNunber, 4] = grindingZone[i].grindingEnd;//               
                worksheet.Cells[strNunber, 5] = grindingZone[i].grindingEnd- grindingZone[i].grindingStart;//
                worksheet.Cells[strNunber, 6] = "мм";//
                strNunber++;
            }
            double summ = 0;
            for (int i = 0; i < grindingZone.Count; i++)
            {
                summ += grindingZone[i].grindingEnd - grindingZone[i].grindingStart;
            }
            worksheet.Cells[strNunber, 2] = "Сумма сквозной выборки";
            worksheet.Cells[strNunber, 5] = summ;//
            worksheet.Cells[strNunber, 6] = "мм";//
            double procentOfPerimetr = 100*summ/perimeter;
            worksheet.Cells[strNunber+1, 2] = "Доля сквозной выборки";
            worksheet.Cells[strNunber+1, 5] = Math.Round( procentOfPerimetr);//
            worksheet.Cells[strNunber+1, 6] = "%";//

            worksheet.Shapes.AddPicture(@"C:\\Result\joint.BMP", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 10, 185, 300, 307);// Shapes.AddPicture(@"C:\\test.BMP", false, false, 50, 50, 300, 45);
            File.Delete(@"C:\\Result\joint.BMP");
            string filename = String.Concat(@"C:\Result\МГ_",report.productName.Replace("/","-").Trim(),"_номер_стыка-", report.jointName.Replace("/", "-").Trim(), "_диаметр-", report.pipeDiameter, "_номер протокола-", report.protocolNumber, "_дата-", report.date, ".xlsx");

            try
            {
                worksheet.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {

            }
           
            //workBook.Close(false, Type.Missing, Type.Missing);
            //app.Quit();
        }
    }
}
