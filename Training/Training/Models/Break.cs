using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Models
{
    public class Break
    {
        int[] breakTimes;
        int[,] limits;
        bool fixedBreak;
        public Break(string input)
        {
            var splits = input.Split(';');
            if (splits.Length > 1)
            {
                //input = 1-3:4-6:7-100;180:120:90
                var strLimits = splits[0].Split(':');
                limits = new int[strLimits.Length, 2];
                breakTimes = new int[strLimits.Length];
                for (int i = 0; i < strLimits.Length; i++)
                {
                    limits[i, 0] = int.Parse(strLimits[i].Split('-')[0]);
                    limits[i, 1] = int.Parse(strLimits[i].Split('-')[1]);
                    breakTimes[i] = int.Parse(splits[1].Split(':')[i]);
                }
                fixedBreak = false;
            }
            else
            {
                breakTimes = new int[] { int.Parse(input) };
                fixedBreak = true;
            }
        }
        public int GetBreak(int input = -1)
        {
            if (fixedBreak)
                return breakTimes[0];
            else
            {
                for (int i = 0; i < breakTimes.Length; i++)
                {
                    if (limits[i, 0] <= input && input <= limits[i, 1])
                        return breakTimes[i];
                }
                return breakTimes[0];
            }
        }
        public override string ToString()
        {
            if (fixedBreak)
                return breakTimes[0].ToString();
            else
            {
                string result = "";
                for (int i = 0; i < breakTimes.Length; i++)
                {
                    result += limits[i, 0].ToString() + "-" + limits[i, 1].ToString();
                    if (i != breakTimes.Length - 1)
                    {
                        result += ":";
                    }
                    else
                    {
                        result += ";";
                    }
                }
                for (int i = 0; i < breakTimes.Length; i++)
                {
                    result += breakTimes[i].ToString();
                    if (i != breakTimes.Length - 1)
                    {
                        result += ":";
                    }
                }
                return result;
            }
        }
    }
}
