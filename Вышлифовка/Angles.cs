using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Вышлифовка
{
    public class Angles
    {
        public int startAngle;
        public int stopAngle;
    }
    public partial class Form1 : Form
    {
        public Angles GetAngles(double start, double stop, double diameter)
        {
            double perimeter = Math.Round(Convert.ToDouble(diameter) * Math.PI);
            Angles result = new Angles();
            result.startAngle = Convert.ToInt32(Math.Round((360 * start) / perimeter));
            result.stopAngle = Convert.ToInt32(Math.Round((360 * stop) / perimeter));
            return result;
        }
        public Angles GetAngles2(double start, double stop, double diameter)
        {
            double perimeter = Math.Round(Convert.ToDouble(diameter) * Math.PI);
            Angles result = new Angles();
            result.stopAngle = 360 - Convert.ToInt32(Math.Round((360 * start) / perimeter)) - 90;
            result.startAngle = 360 - Convert.ToInt32(Math.Round((360 * stop) / perimeter)) - 90;
            if (result.stopAngle - result.startAngle < 1)
            {
                result.stopAngle = result.startAngle + 1;
            }
            return result;
        }

        public Angles GetAngles3(double start, double stop, double diameter, bool isNoСlockwise)
        {
            Angles result = new Angles();
            double perimeter = Math.Round(Convert.ToDouble(diameter) * Math.PI);
            if (isNoСlockwise==false)//по часовой стрелке
            {
                result.startAngle = Convert.ToInt32(Math.Round((360 * start) / perimeter)) - 90;
                result.stopAngle = Convert.ToInt32(Math.Round((360 * stop) / perimeter)) - 90;
                if (result.stopAngle - result.startAngle < 1)
                {
                    result.stopAngle = result.startAngle + 1;
                }
            }
            else//против часовой стрелке
            {
                result.stopAngle = 360 - Convert.ToInt32(Math.Round((360 * start) / perimeter)) - 90;
                result.startAngle = 360 - Convert.ToInt32(Math.Round((360 * stop) / perimeter)) - 90;
                if (result.stopAngle - result.startAngle < 1)
                {
                    result.stopAngle = result.startAngle + 1;
                }
            }
            return result;
        }
    }
}
