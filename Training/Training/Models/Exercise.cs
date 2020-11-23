using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Models
{
    public class Exercise:IEquatable<Exercise>
    {
        public string Name { get; }
        public string Link { get; }
        public string Holdtimes { get; }
        public Exercise(string input)
        {
            var splits = input.Split('$');
            Name = splits[0];
            Link = splits[1];
            if (splits.Length > 2)
            {
                Holdtimes = splits[2];
            }
            else
            {
                Holdtimes = "";
            }
        }
        public override string ToString()
        {
            string result = Name + "$" + Link;
            if (Holdtimes != "")
            {
                result += "$" + Holdtimes;
            }
            return result;
        }

        public bool Equals(Exercise other)
        {
            return this.Name == other.Name;
        }
    }
}
