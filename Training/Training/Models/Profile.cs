using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Models
{
    public class Profile
    {
        public int Week { get; set; }
        public int Day { get; set; }
        public int WorkoutLevel { get; set; }
        public Dictionary<string, int> maxReps;
        public Profile(string input)
        {
            var splits = input.Split(',');
            WorkoutLevel = int.Parse(splits[0]);
            Week = int.Parse(splits[1]);
            Day = int.Parse(splits[2]);
            maxReps = new System.Collections.Generic.Dictionary<string, int>();
            for (int i = 3; i < splits.Length; i++)
            {
                string ex = splits[i].Split('¦')[0];
                int num = int.Parse(splits[i].Split('¦')[1]);
                if (maxReps.ContainsKey(ex))
                {
                    maxReps.Remove(ex);
                }
                maxReps.Add(ex, num);
            }
        }
        public void UpdateMaxReps(WorkoutData[] workouts)
        {
            maxReps = new Dictionary<string, int>();
            for (int i = 0; i < workouts.Length; i++)
            {
                if (workouts[i].Title == "Test")
                    for (int e = 0; e < workouts[i].Exercises.Length; e++)
                    {
                        if (maxReps.ContainsKey(workouts[i].Exercises[e].Name))
                        {
                            maxReps.Remove(workouts[i].Exercises[e].Name);
                        }
                        maxReps.Add(workouts[i].Exercises[e].Name, workouts[i].MaxReps[e]);
                    }
            }
        }
        public int MaxReps(Exercise ex)
        {
            int res;
            maxReps.TryGetValue(ex.Name, out res);
            return res;
        }
        public override string ToString()
        {
            string res = string.Join(",", new string[] {WorkoutLevel.ToString(), Week.ToString(), Day.ToString() });
            foreach (var item in maxReps.Keys)
            {
                res += "," + item.ToString() + "¦" + maxReps[item].ToString();
            }
            return res;
        }
        public string NextString()
        {
            int nD, nW;
            if (Day < 7)
            {
                nD = Day + 1;
                nW = Week;
            }
            else
            {
                nD = 1;
                nW = Week + 1;
            }
            string res = string.Join(",", new string[] {WorkoutLevel.ToString(), nW.ToString(), nD.ToString() });
            foreach (var item in maxReps.Keys)
            {
                res += "," + item.ToString() + "¦" + maxReps[item].ToString();
            }
            return res;
        }
    }
}
