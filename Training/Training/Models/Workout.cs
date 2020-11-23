using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Models
{
    public class Workout
    {
        public bool OnBreak { get { if (Finished) return false; return data.WorkoutMatrix[5][current] == 1; } }
        public bool Finished { get { return max == current; } }
        public bool NeedsTimer { get { if (Finished) return false; return data.WorkoutMatrix[6][current] == 1; } }
        public int Time { get { if (Finished) return 0;if(OnBreak) return data.Breaks[data.WorkoutMatrix[0][current]].GetBreak(userReps); return data.Reps[data.WorkoutMatrix[2][current]]; } }
        public bool NeedsInput { get { if (Finished) return false; return data.WorkoutMatrix[7][current] == 1; } }
        public WorkoutData Data { get { return data; } }
        public int Current { get {return current; } set { current = value; } }
        public float Progress { get { return (float)current / max; } }
        public string[] Output
        {
            get
            {
                if (current < max)
                {
                    //1.Name, 2.Reps, 3.technique 4.Set info 5.Next exercise
                    string[] output = new string[5];
                    if (data.WorkoutMatrix[5][current] == 0)
                    {
                        int index = data.WorkoutMatrix[0][current];
                        output[0] = data.Exercises[index].Name;
                        output[1] = data.RepText[data.WorkoutMatrix[1][current]];
                        switch (output[1][0])
                        {
                            case '-':
                                output[1] = output[1].Replace("-", (data.MaxReps[data.WorkoutMatrix[0][current]] - data.Reps[data.WorkoutMatrix[2][current]]).ToString());
                                break;
                            case '%':
                                output[1] = output[1].Replace("%", (data.MaxReps[data.WorkoutMatrix[0][current]] * data.Reps[data.WorkoutMatrix[2][current]] / 100).ToString());
                                break;
                            default:
                                break;
                        }
                        var holds = new string[] { "Up", "Hold", "Down" };
                        output[2] = data.Exercises[data.WorkoutMatrix[0][current]].Holdtimes;
                        /*
                        if (holdStrings != string.Empty) {
                            var strings=holdStrings.Split(':').Select(t => holds[(int.Parse(t) / 10)-1]).ToArray();
                            var timeStr = holdStrings.Split(':').Select(t=>t[1]).ToArray();
                            output[2] = String.Format("{0} {1}s, {2} {3}s, {4} {5}s", strings[0],timeStr[0], strings[1],timeStr[1], strings[2],timeStr[2]);
                        }
                        else
                        {
                            output[2] = "";
                        }
                        */
                        output[3] = String.Format("Set {0}/{1}", data.WorkoutMatrix[3][current], data.WorkoutMatrix[4][current]);
                        if (index < data.Exercises.Length - 1)
                        {
                            output[4] = "Next Exercise: "+ data.Exercises[index+1].Name;
                        }
                        else
                        {
                            output[4] = "";
                        }
                    }
                    else
                    {
                        output[0] = "Break";
                        output[1] = new TimeSpan((long)data.Breaks[data.WorkoutMatrix[0][current]].GetBreak(userReps) * 10000000).ToString(@"mm\:ss");
                        int index = data.WorkoutMatrix[0][current+1];
                        output[2] = "";
                        output[3] = String.Format("Set {0}/{1}", data.WorkoutMatrix[3][current], data.WorkoutMatrix[4][current]);
                        var curString = output[2];
                        if (index < data.Exercises.Length)
                        {
                            output[4] = "Next Exercise: " + data.Exercises[index].Name;
                        }
                        else
                        {
                            output[4] = "";
                        }

                    }
                    return output;
                }
                return new string[] { "Completed Workout", "", "", "", "" };
            }
        }
        WorkoutData data;
        private int current = 0;
        private int max;
        private int userReps;
        public Workout(WorkoutData data)
        {
            this.data = data;
            if (data.IsNull)
            {
            }
            else
            {
                max = data.WorkoutMatrix[0].Length;
            }
        }
        public void Last()
        {
            if (current > 0)
            {
                current--;
            }
            while (current > 0 && data.WorkoutMatrix[5][current] == 1)
            {
                current--;
            }
        }
        public void Next(int reps)
        {
            userReps = reps;
            if (userReps >= 0)
            {
                data.WorkoutMatrix[8][current] = userReps;
                if (data.Title == "Test")
                    data.MaxReps[data.WorkoutMatrix[0][current]] = userReps;
            }
            else
            {
                switch (data.RepText[data.WorkoutMatrix[1][current]][0])
                {
                    case '-':
                        userReps = data.MaxReps[data.WorkoutMatrix[0][current]] - data.Reps[data.WorkoutMatrix[2][current]];
                        break;
                    case '%':
                        userReps=data.MaxReps[data.WorkoutMatrix[0][current]] * data.Reps[data.WorkoutMatrix[2][current]] / 100;
                        break;
                    default:
                        userReps = data.Reps[data.WorkoutMatrix[2][current]];
                        break;
                }
                data.WorkoutMatrix[8][current] = userReps;
            }
            if (current < max)
            {
                current++;
            }
        }
        public void SetTime()
        {
            data.Completion = System.DateTime.Now.ToBinary();
        }
    }

}
