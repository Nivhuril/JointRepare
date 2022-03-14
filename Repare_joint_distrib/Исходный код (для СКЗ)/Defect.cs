using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Вышлифовка
{
    public class Defect
    {
        public bool isDiscontinuity;
        public double start;
        public double defectLength;
        public bool isInadmissible;

    }
    public partial class Form1 : Form
    {
        private List<Defect> getDefectsFromReport(Report report)
        {
            List<Defect> result=new List<Defect>();

            for (int i = 0; i < report.Defects.Count; i++)
            {
                Defect def = new Defect();
                if (report.Defects[i].isInadmissible)
                {
                    def.start = report.Defects[i].start;
                    def.defectLength = report.Defects[i].defectLength;
                    def.isDiscontinuity = report.Defects[i].isDiscontinuity;
                    result.Add(def);
                }
            }

            return result;
        }
        private List<Defect> get_defects_from_file(string fileName)//метод для чтения списка дефектов
        {
            //Создаём приложение.
            Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
            //Открываем книгу.                                                                                                                                                        
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(fileName, 0, true, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            ObjWorkBook.Unprotect();

            //получаем названия вкладок книги
            List<string> workSheets = new List<string>();
            for (int g = 1; g < ObjWorkBook.Worksheets.Count + 1; g++)
            {
                workSheets.Add(ObjWorkBook.Worksheets[g].Name);
            }
            //Выбираем таблицу(лист).
            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;

            //string WorksheetName = ObjWorkBook.Sheets[1].ToString();
            string WorksheetName = workSheets[0];
            //string WorksheetName = "Лист1";//получаем название вкладки
            ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[WorksheetName];
            List<Defect> result = new List<Defect>();//создаём список строк
            richTextBox2.AppendText(Environment.NewLine + "Выполняется чтение данных журнала дефектов...");
            richTextBox2.AppendText(Environment.NewLine + "->*");
            int incrementor = 0;//переменная для прогресс - индикатора
            int i = 1;
            bool mark = true;
            while (mark)
            {
                Defect def = new Defect();//создаём экземпляр класса строки журнала аномалий
                def.isDiscontinuity = true;
                try
                {
                    char ch = '-';
                    string str = Convert.ToString(ObjWorkSheet.Cells[i, 1].Text);
                    int index = str.IndexOf(ch);
                    //richTextBox2.AppendText(Environment.NewLine + index);
                    def.start = Convert.ToInt32(str.Substring(0, index));
                    //richTextBox2.AppendText(Environment.NewLine + Convert.ToInt32(str.Substring(1, index - 1)));
                    //def.start = Convert.ToInt32(ObjWorkSheet.Cells[i, 1].Text.Trim());
                }
                catch (Exception)
                {
                    def.start = 0;
                }
                try
                {
                    def.defectLength = Convert.ToInt32(ObjWorkSheet.Cells[i, 2].Text.Trim());//Столбец 1, кодовый номер объекта;
                }
                catch (Exception)
                {
                    def.defectLength = 0;
                }


                if (String.IsNullOrWhiteSpace(ObjWorkSheet.Cells[i, 1].Text))
                {
                    mark = false;//дошли до конца трубного журлала
                }
                else
                {
                    result.Add(def);//добавляем заполненный экземпляр класса к списку
                    //richTextBox1.AppendText(FurnishingsLog.characterFeatures);
                }

                incrementor++;//сделаем прогресс-индикатор, чтобы было не так скучно ждать.
                if (incrementor > 9)
                {
                    richTextBox2.AppendText("*");
                    incrementor = 0;
                }
                i++;
            }
            richTextBox2.AppendText(Environment.NewLine + "Массив данных из журнала дефектов прочитан, количество строк: " + result.Count);
            richTextBox2.AppendText(Environment.NewLine + "==========================================");
            ObjWorkBook.Close(false, Type.Missing, Type.Missing);
            ObjExcel.Quit();
            return result;
        }
        private List<Defect> increaseBiases(List<Defect> defectsFor)//формируем массив дефектов для расчета вышлифовки. Для этого увеличиваем протяженность смещений кромок в обе стороны на половину величины протяженности дефекта.
        {
            double perimeter = Math.Round(Convert.ToDouble(diameterTextBox.Text) * Math.PI);
            List<Defect> result = new List<Defect>();
            for (int i = 0; i < defectsFor.Count; i++)
            {
                if (defectsFor[i].isDiscontinuity)//если дефект это несплошность, просто добавляем его к новому списку
                {
                    result.Add(defectsFor[i]);
                }
                else//если это смещение кромок, увеличиваем его размеры до размеров вышлифовки.
                {
                    Defect def1 = new Defect();
                    Defect def2 = new Defect();
                    def1.start = defectsFor[i].start - defectsFor[i].defectLength / 2;
                    def1.defectLength = 2 * defectsFor[i].defectLength;
                    if (def1.start < 0)//проверяем, чтобы значение начала не перешагнуло за начало отсчета. Если перешагнула, делим область на две.
                    {
                        def2.start = perimeter + def1.start;
                        def2.defectLength = perimeter - def2.start;//протяженность от начала дефекта до конца периметра
                        def1.start = 0;
                        def1.defectLength = def1.defectLength - def2.defectLength;
                        result.Add(def1);
                        result.Add(def2);
                    }
                    else
                    {
                        result.Add(def1);
                    }

                }
            }
            return result;
        }
        private void printDefects(List<Defect> defects)//выводим в текстовое поле список дефектов
        {
            double summ = 0;
            for (int i = 0; i < defects.Count; i++)
            {
                string type = "Смещение";
                if (defects[i].isDiscontinuity)
                {
                    type = "Внутренний дефект";
                }
                richTextBox2.AppendText(Environment.NewLine + "№ " + Convert.ToString(i + 1) + ", " + type + ". (Нач. " + defects[i].start + " мм, протяж. " + defects[i].defectLength + " мм).");
                summ += defects[i].defectLength;
            }
            richTextBox2.AppendText(Environment.NewLine);
            richTextBox2.AppendText(Environment.NewLine + "Суммарная протяженность дефектов:  " + Convert.ToString(summ) + " мм.");
        }
        
    }



}
