using Newtonsoft.Json;
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
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Gun> Guns = new List<Gun>();
        public string Name { get; set; }
        public int OrderType { get; set; }
    }

    public class Enemy
    {
        [JsonProperty(Order = 1)]
        public int EnemyName { get; set; }
        [JsonProperty(Order = 2)]
        public string EnemyNameString { get; set; }
        [JsonProperty(Order = 3)]
        public int Value = 0;

        [JsonIgnore]
        public string ValueString
        {
            get
            {
                return Value.ToString();
            }
        }

       
    }

    public class Gun
    {
        public string GunName { get; set; }
        public string MagName { get; set; }
        public List<string> MagNames = new List<string>();
        public int CategoryID { get; set; }
        public string Extra { get; set; }

        [JsonIgnore]
        public string AmmoString
        {
            get
            {
                if (MagNames.Count != 0)
                {
                    string temp = MagNames[0];
                    for (int i = 1; i < MagNames.Count; i++)
                    {

                    }
                    foreach (string mag in MagNames)
                    {
                        temp += "\r\n" + mag;
                    }
                    return temp;
                }
                return "";
            }
        }
    }

}
