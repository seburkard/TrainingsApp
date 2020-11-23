using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Models
{
    public class WorkoutData
    {
        public string Title { get; }
        public int Day { get; }
        public int Week { get; }
        public long Completion { get; set; }
        public Exercise[] Exercises { get; }
        public Break[] Breaks { get; }
        public int[] MaxReps { get; set; }
        public string[] RepText { get; }
        public int[] Reps { get; }
        public int[][] WorkoutMatrix { get; set; }
        public bool IsNull { get; }
        public WorkoutData(string input)
        {
            IsNull = true;
            //0. WorkoutTitle 1.Week 2.Day 3.Time of completion 4.Exercises 5.Breaks 6.MaxReps of exercises 7.repText 8.Reps
            //0/9 current exercise or break 1/10 current Text 2/11 current Reps 3/12 current set 4/13 max set 5/14 on Break 6/15 need Timer 7/16 need User Reps 8/17 user Reps 
            var splits = input.Split(',');
            if (splits.Length == 18)
            {
                Title = splits[0];
                Week = int.Parse(splits[1]);
                Day = int.Parse(splits[2]);
                Completion = long.Parse(splits[3]);
                Exercises = FromString<Exercise>(splits[4]);
                Breaks = FromString<Break>(splits[5]);
                MaxReps = FromString(splits[6]);
                RepText = splits[7].Split('¦');
                Reps = FromString(splits[8]);
                WorkoutMatrix = new int[9][];
                for (int i = 0; i < 9; i++)
                {
                    WorkoutMatrix[i] = FromString(splits[9 + i]);
                }
                IsNull = false;
            }
            else if (splits.Length == 4)
            {
                Title = splits[0];
                Week = int.Parse(splits[1]);
                Day = int.Parse(splits[2]);
                Completion = long.Parse(splits[3]);
                IsNull = true;
            }
            else
            {
                Console.WriteLine("Error," + splits.Length + " elements.");
            }
        }
        public override string ToString()
        {
            string res = string.Join(",",
                new string[] { Title.ToString(), Week.ToString(), Day.ToString(), Completion.ToString() });
            if (!IsNull)
            {
                res = string.Join(",", new string[] { res,string.Join<Exercise>("¦", Exercises),
                    string.Join<Break>("¦", Breaks),string.Join("¦", MaxReps),string.Join("¦", RepText),string.Join("¦", Reps)});
                for (int i = 0; i < 9; i++)
                {
                    res += "," + string.Join("¦", WorkoutMatrix[i]);
                }
            }
            return res;
        }
        private int[] FromString(string str)
        {
            var splits = str.Split('¦');
            int[] res = new int[splits.Length];
            for (int i = 0; i < splits.Length; i++)
            {
                res[i] = int.Parse(splits[i]);
            }
            return res;
        }
        private T[] FromString<T>(string str)
        {
            var splits = str.Split('¦');
            T[] res = new T[splits.Length];
            for (int i = 0; i < splits.Length; i++)
            {
                res[i] = (T)Activator.CreateInstance(typeof(T), splits[i]);
            }
            return res;
        }
        public void SetMaxReps(Dictionary<Exercise, int> maxReps)
        {
            if (!IsNull)
            {
                for (int i = 0; i < Exercises.Length; i++)
                {
                    int reps;
                    if (maxReps.TryGetValue(Exercises[i], out reps))
                        MaxReps[i] = reps;
                }
            }
        }
    }
}
