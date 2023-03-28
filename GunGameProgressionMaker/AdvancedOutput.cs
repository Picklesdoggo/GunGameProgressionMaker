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
        [JsonProperty(Order = 1)]
        public string WeaponPoolType = "Advanced";

        [JsonProperty(Order = 2)]
        public string Description { get; set; }

        [JsonProperty(Order = 3)]
        public int EnemyProgressionType { get; set; }

        [JsonProperty(Order = 4)]
        public List<Enemy> Enemies = new List<Enemy>();

        [JsonProperty(Order = 5)]
        public List<Gun> Guns = new List<Gun>();

        [JsonProperty(Order = 6)]
        public string Name { get; set; }

        [JsonProperty(Order = 7)]
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
        [JsonProperty(Order = 1)]
        public string GunName { get; set; }

        [JsonProperty(Order = 2)]
        public string MagName { get; set; }

        [JsonProperty(Order = 3)]
        public List<string> MagNames = new List<string>();


        [JsonProperty(Order = 4)] 
        public int CategoryID { get; set; }


        [JsonProperty(Order = 5)] 
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
