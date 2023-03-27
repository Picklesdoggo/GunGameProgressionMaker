using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunGameProgressionMakerAdvanced
{

    public class AdvancedOutput
    {
        public string WeaponPoolType { get; set; }
        public string Description { get; set; }
        public int EnemyProgressionType { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Gun> Guns { get; set; }
        public string Name { get; set; }
        public int OrderType { get; set; }
    }

    public class Enemy
    {
        public int EnemyName { get; set; }
        public string EnemyNameString { get; set; }
        public int Value { get; set; }
    }

    public class Gun
    {
        public string GunName { get; set; }
        public string MagName { get; set; }
        public List<string> MagNames { get; set; }
        public int CategoryID { get; set; }
        public string Extra { get; set; }
    }

}
