using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GunGameProgressionMaker
{
    class InputJson
    {

        public List<string> fileLocations { get; set; }
        public int maxCategories { get; set; }
        public List<Gun> guns { get; set; }
        public List<Enemy> enemies { get; set; }
        public List<string> enemyCategories { get; set; }
        public List<string> eras { get; set; }
        public List<string> extraCategories { get; set; }
        public List<string> firearmactions { get; set; }
        public List<string> categories { get; set; }
        public List<string> nations { get; set; }
        public List<string> calibers { get; set; }
        public List<Extras> extras { get; set; }

    }

    public class Gun
    {
        public string GunName { get; set; }

        public string Caliber { get; set; }
        public int CategoryID { get; set; }
        public List<string> Categories { get; set; }
        public List<string> CompatableAmmo { get; set; }
        public string DefaultMagName { get; set; }
        public string Era { get; set; }
        public string FirearmAction { get; set; }
        public string NationOfOrigin { get; set; }
        public string SelctedMagName { get; set; }
        public bool UsesSpeedLoader { get; set; }        
    }

    public class Extras
    {
        public string ExtraName { get; set; }
        public string SubCategory { get; set; }
    }
}
