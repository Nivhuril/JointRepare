using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Вышлифовка
{
    public class Report
    {
        public List<Defect> Defects;
        public string protocolNumber;
        public string date;
        public string objectName;
        public string jointName;
        public double pipeDiameter;
        public string controlledZoneLength;
        public string perimeter;
        public string productName;
        public string thekness;
    }
    public partial class Form1 : Form
    {
        private Report get_report_from_file(string fileName)//метод для чтения протокола из файла
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
            string WorksheetOUT = workSheets[0];
            string WorksheetReport = workSheets[1];
            //Выбираем таблицу(лист).

            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheetOUT = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[WorksheetOUT];
            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheetReport = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[WorksheetReport];

            List<Defect> defects = new List<Defect>();//создаём список строк
            Report report = new Report();


            report.thekness= Convert.ToString(ObjWorkSheetOUT.Cells[5, 9].Text);
            report.protocolNumber = Convert.ToString(ObjWorkSheetOUT.Cells[1, 2].Text);
            report.objectName = Convert.ToString(ObjWorkSheetOUT.Cells[3, 2].Text);
            report.date = Convert.ToString(ObjWorkSheetOUT.Cells[2, 2].Text);
            report.jointName = Convert.ToString(ObjWorkSheetOUT.Cells[4, 2].Text);
            report.productName = Convert.ToString(ObjWorkSheetOUT.Cells[4, 9].Text);
            report.perimeter = Convert.ToString(ObjWorkSheetOUT.Cells[2, 9].Text);

            string PD= Convert.ToString(ObjWorkSheetOUT.Cells[5, 2].Text);
            if (string.IsNullOrWhiteSpace(PD)==false)
            {
                try
                {
                    report.pipeDiameter = Convert.ToDouble(PD.Replace(" мм", "").Replace("мм", ""));
                }
                catch (Exception)
                {
                    report.pipeDiameter = 0;
                }
            }
            else
            {
                report.pipeDiameter = 0;
            }
            

            richTextBox2.AppendText(Environment.NewLine + "Выполняется чтение данных журнала дефектов...");
            richTextBox2.AppendText(Environment.NewLine + "->*");
            int incrementor = 0;//переменная для прогресс - индикатора
            int i = 5;
            bool mark = true;
           
            while (mark)
            {
                Defect def = new Defect();//создаём экземпляр класса строки журнала аномалий
                def.isDiscontinuity = true;
                try
                {
                    char ch = '-';
                    string str = Convert.ToString(ObjWorkSheetReport.Cells[i, 3].Text);
                    int index = str.IndexOf(ch);
                    def.start = Convert.ToInt32(str.Substring(0, index));                   
                }
                catch (Exception)
                {
                    def.start = 0;
                }
                if (String.IsNullOrWhiteSpace(ObjWorkSheetReport.Cells[i, 8].Text))
                {
                    def.isInadmissible = false;
                }
                else
                {
                    def.isInadmissible = true;
                }

                try
                {
                    def.defectLength = Convert.ToInt32(ObjWorkSheetReport.Cells[i, 4].Text.Trim());//Столбец 1, кодовый номер объекта;
                }
                catch (Exception)
                {
                    def.defectLength = 0;
                }


                if (String.IsNullOrWhiteSpace(Convert.ToString(ObjWorkSheetReport.Cells[i, 2].Text)))
                {
                    mark = false;//дошли до конца трубного журлала
                }
                else
                {
                    defects.Add(def);//добавляем заполненный экземпляр класса к списку
                }

                incrementor++;//сделаем прогресс-индикатор, чтобы было не так скучно ждать.
                if (incrementor > 9)
                {
                    richTextBox2.AppendText("*");
                    incrementor = 0;
                }
                i++;
            }
            report.Defects=defects; 
            richTextBox2.AppendText(Environment.NewLine + "Массив данных из журнала дефектов прочитан, количество строк: " + report.Defects.Count);
            richTextBox2.AppendText(Environment.NewLine + "==========================================");


            ObjWorkBook.Close(false, Type.Missing, Type.Missing);
            ObjExcel.Quit();
            return report;
        }

    }
}
