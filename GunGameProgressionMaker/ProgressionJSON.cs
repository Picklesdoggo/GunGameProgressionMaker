using System.Collections.Generic;

namespace GunGameProgressionMaker
{


    public class ProgressionJSON
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderType { get; set; }
        public string EnemyType { get; set; }
        public object[] Guns { get; set; }
        public List<string> GunNames { get; set; }
        public List<string> MagNames { get; set; }
        public List<int> CategoryIDs { get; set; }
    }


   

}
