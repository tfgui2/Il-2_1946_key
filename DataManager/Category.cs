using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagerLibrary
{
    public class Category
    {
        public string Name { get; set; }
        public List<Entity> Entitys { get; set; } = new List<Entity>();

        public void Sort()
        {
            Entitys.Sort(comparison);
        }

        public void Delete(Entity e)
        {
            Entitys.Remove(e);
        }

        private int comparison(Entity x, Entity y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }
}
