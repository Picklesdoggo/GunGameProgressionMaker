using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunGameProgressionMaker
{
    public class Config
    {
        public List<Enemy> enemies { get; set; }
        public List<string> gunblacklist { get; set; }
        public List<string> filelocations { get; set; }

        public int maxcategories { get; set; }

        public string gameResourcesPath { get; set; }
        public string gameManagedPath { get; set; }
        public string modsDirectory { get; set; }
        public List<string> manuallyLoadedMods { get; set; }
    }

    public class Enemy
    {
        public string category { get; set; }
        public string name { get; set; }
        public string appearance { get; set; }
        public string weapons { get; set; }
        public string note { get; set; }
    }
}
