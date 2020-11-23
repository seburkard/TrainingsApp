using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Training.Models;

namespace Training.Services
{
    static class DataManager
    {
        private static string progressPath = "progressFiles.txt";
        private static string profilePath = "profileFiles.txt";
        public static WorkoutData[] LoadPlan(int n=-1)
        {
            if (n == -1)
            {
                var profile = LoadProfile();
                n = profile.WorkoutLevel;
            }
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Training.Resources.plan"+n.ToString()+".txt";
            List<WorkoutData> result = new List<WorkoutData>();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    result.Add(new WorkoutData(reader.ReadLine()));
                }
            }
            return result.ToArray();
        }
        public static string[] GetPath()
        {
            return new string[] { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), profilePath) ,
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), progressPath)};
        }
        public static void SaveWorkout(WorkoutData data)
        {
            var oldProgress = LoadProgress();
            WorkoutData[] newProgress = new WorkoutData[oldProgress.Length + 1];
            for (int i = 0; i < oldProgress.Length; i++)
            {
                newProgress[i] = oldProgress[i];
            }
            data.Completion = DateTime.Now.ToBinary();
            newProgress[oldProgress.Length] = data;
            SaveProgress(newProgress);
        }
        public static WorkoutData[] LoadProgress()
        {
            return FromPath(progressPath);
        }
        public static Profile LoadProfile()
        {
            var a = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), profilePath);
            return File.Exists(path) ? new Profile(File.ReadAllText(path)) : new Profile("1,0,1");
        }
        public static void SaveProgress(WorkoutData[] data)
        {
            SaveData(data, progressPath);
        }
        public static void SaveProfile(Profile profile)
        {
            SaveData(new Profile[] { profile }, profilePath);
        }
        public static void ClearProgress()
        {
            SaveProgress(new WorkoutData[0]);
        }
        public static void ClearProfile()
        {
            SaveProfile(new Profile("1,0,1")); 
        }
        private static WorkoutData[] FromPath(string path)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path);
            if (File.Exists(path))
            {
                var allLines = File.ReadAllLines(path);
                var result = new WorkoutData[allLines.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new WorkoutData(allLines[i]);
                }
                return result;
            }
            else
            {
                return new WorkoutData[0];
            }
        }
        private static void SaveData<T>(T[] data,string path)
        {
            string[] lines = new string[data.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = data[i].ToString();
            }
            File.WriteAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path), lines);
        }
    }
}
