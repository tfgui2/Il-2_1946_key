using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagerLibrary
{
    public class DataManager
    {
        public DataManager(string filename)
        {
            _filename = filename;
            Init(filename);
        }

        private string _filename;

        private void Init(string filename)
        {
            var lines = System.IO.File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                ParseLine(line);
            }
        }

        public void Save()
        {
            List<string> list = new List<string>();
            foreach (var cat in Categorys)
            {
                list.Add(cat.Name);

                foreach (var item in cat.Entitys)
                {
                    list.Add(item.Line);
                }
            }

            string[] contents = list.ToArray();
            System.IO.File.WriteAllLines(_filename, contents);
        }

        public List<Category> Categorys { get; set; } = new List<Category>();
        public List<Category> Joysticks { get; set; } = new List<Category>();

        public void Delete(Entity e)
        {
            foreach (var cat in Categorys)
            {
                cat.Delete(e);
            }
        }

        private Category CurrentCategory { get; set; }

        public void SortEntity(Category category)
        {
            category.Sort();
        }

        private void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line[0] == '[')
            {
                NewCategory(line);
            }
            else
            {
                NewEntity(line);
            }
        }

        private void NewCategory(string name)
        {
            CurrentCategory = new Category() { Name = name };
            Categorys.Add(CurrentCategory);
        }

        private void NewEntity(string line)
        {
            var e = new Entity(line);
            CurrentCategory.Entitys.Add(e);

            if (e.IsJoystick == true)
            {
                Category joyCat = FindJoyCategory(e.Value);
                joyCat.Entitys.Add(e);
            }
        }

        private Category FindJoyCategory(string value)
        {
            string joystickName = GetJoystickName(value);
            if (string.IsNullOrWhiteSpace(joystickName) == true)
                throw new Exception("Invalid joystick name");

            foreach (var j in Joysticks)
            {
                if (j.Name == joystickName)
                    return j;
            }

            Category joy = new Category() { Name = joystickName };
            Joysticks.Add(joy);
            return joy;
        }

        private string GetJoystickName(string value)
        {
            var list = value.Split(' ');
            foreach (var item in list)
            {
                if (item.ToLower().Contains("device") == true)
                {
                    return item;
                }
            }
            return "";
        }
    }
}
