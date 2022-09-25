using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GunGameProgressionMaker
{
    class GunJson
    {
        public List<string> fileLocations { get; set; }
        public ObservableCollection<Gun> guns { get; set; }
        public List<string> enemies { get; set; }
        public List<string> eras { get; set; }
        public List<string> categories { get; set; }
        public List<string> nations { get; set; }
    }

    public class Gun
    {
        public string GunName { get; set; }
        public string DefaultMagName { get; set; }
        public int CategoryID { get; set; }
        public string SelctedMagName { get; set; }
        public string Era { get; set; }
        public List<string> Categories { get; set; }
        public List<string> CompatableAmmo { get; set; }
        public string NationOfOrigin { get; set; }
    }
}
