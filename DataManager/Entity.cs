using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagerLibrary
{
    public class Entity
    {
        public Entity(string line)
        {
            Parse(line);
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Line { get { return $"{Value}={Key}"; } }

        private void Parse(string line)
        {
            var temp = line.Split('=');
            Value = temp[0];
            Key = temp[1];
        }
    }
}
