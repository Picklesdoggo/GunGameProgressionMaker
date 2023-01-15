using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


namespace GunGameProgressionMaker
{
    public class JsonBuilder
    {
        #region Member Variables

        static List<GameObject> gameObjects = new List<GameObject>();

        static List<string> entryJson;

        static List<string> itemSpawnerIDS = new List<string>();
        static List<string> itemSpawnerIDWhiteList = new List<string>
        {
            "itemspawnerids/meatfortress",
            "itemspawnerids/pistols",
            "itemspawnerids/shotguns",
            "itemspawnerids/smgrifle",
            "itemspawnerids/smgrifle2",
            "itemspawnerids/support"
        };

        static List<string> objectIDS = new List<string>();
        static List<string> objectIDSWhitelist = new List<string>
        {
            "objectids/weaponry_historical",
            "objectids/weaponry_mf2/guns",
            "objectids/weaponry_pistols/automatic",
            "objectids/weaponry_pistols/boltaction",
            "objectids/weaponry_pistols/breechloading",
            "objectids/weaponry_pistols/derringer",
            "objectids/weaponry_pistols/leveraction",
            "objectids/weaponry_pistols/machinepistol",
            "objectids/weaponry_pistols/revolver",
            "objectids/weaponry_rifle/antimaterial",
            "objectids/weaponry_rifle/assaultrifle",
            "objectids/weaponry_rifle/battlerifle",
            "objectids/weaponry_rifle/boltaction",
            "objectids/weaponry_rifle/carbine",
            "objectids/weaponry_rifle/leveraction",
            "objectids/weaponry_shotguns/shotguns/breakaction",
            "objectids/weaponry_shotguns/shotguns/lever action",
            "objectids/weaponry_shotguns/shotguns/magazinefed",
            "objectids/weaponry_shotguns/shotguns/tubefed",
            "objectids/weaponry_smg/pdw",
            "objectids/weaponry_smg/smg",
            "objectids/weaponry_support/exotic",
            "objectids/weaponry_support/machinegun",
            "objectids/weaponry_support/ordnance"

        };

        static Dictionary<string, string> categoryDictionary = new Dictionary<string, string>
        {
            {"antimaterial" , "Anti-Material"},
            {"assault rifle","Assault Rifle"},
            {"assaultrifle","Assault Rifle"},
            {"automatic","Automatic"},
            {"battle rifle","Battle Rifle"},
            {"battlerifle","Battle Rifle"},
            {"boltaction","Bolt-Action"},
            {"breachloading","Breach Loading"},
            {"breakaction","Break ACtion"},
            {"breechloader","Breach Loading"},
            {"breechloading","Breach Loading"},
            {"carbine","Carbine"},
            {"derringer","Derringer"},
            {"exotic","Exotic"},
            {"flintlock","Flintlock"},
            {"grenadelauncher","Grenade Launcher"},
            {"lever action","Lever Action"},
            {"leveraction","Lever Action"},
            {"locomotionguns","Locomotion"},
            {"machinegun","Machine Gun"},
            {"machinepistol","Machine Pistol"},
            {"magazinefed","Magazine Fed"},
            {"meatfortress","Meat Fortress"},
            {"mf_all","Meat Fortress"},
            {"mf_scout","Meat Fortress"},
            {"muzzleloader","Muzzle Loader"},
            {"ordnance","Ordnance"},
            {"pdw","PDW"},
            {"pistols","Pistol"},
            {"revolver","Revolver"},
            {"rocketmissilelauncher","Rocket Launcher"},
            {"shotguns","Shotgun"},
            {"smg","SMG"},
            {"smgrifle","Rifle"},
            {"smgrifle2","Rifle"},
            {"support","Support"},
            {"tubefed","Tube Fed"}
        };

        static GunJson gunJson = new GunJson();

        static Cache fullCache = new Cache();

        static List<string> gunBlackList = new List<string>
        {
            "BrownBess",
            "HeavyFlintlock18thCentury",
            "Flaregun",
            "FlaregunHighPressure",
            "GrappleGun",
            "GravitonBeamer",
            "JunkyardFlameThrower",
            "MF_Syringegun",
            "PlungerLauncher",
            "PotatoGun",
            "RPG7",
            "Stinger",
            "SustenanceCrossbow",
            "Whizzbanger"
        };

        static List<string> enemies = new List<string>
        {
            "D_Bandito",
            "D_Boss",
            "D_BountyHunter",
            "D_BountyHunterBoss",
            "D_Gambler",
            "D_Gunfighter",
            "D_Sheriff",
            "D_Sniper",
            "Gladiator_Hoplite",
            "Gladiator_Maximus",
            "Gladiator_Murmillo",
            "Gladiator_Porcus",
            "Gladiator_Secutor",
            "Gladiator_Thraex",
            "H_BreadCrabZombie_Fast",
            "H_BreadCrabZombie_HEV",
            "H_BreadCrabZombie_Poison",
            "H_BreadCrabZombie_Standard",
            "H_BreadCrabZombie_Zombie",
            "H_CivicErection_Meathack",
            "H_CivicErection_Melee",
            "H_CivicErection_Pistol",
            "H_CivicErection_SMG",
            "H_OberwurstElite_AR2",
            "H_OberwurstSoldier_Shotgun",
            "H_OberwurstSoldier_SMG",
            "H_OberwurstSoldier_SMGNade",
            "H_OberwurstSoldier_Sniper",
            "J_Commando",
            "J_Flamewiener",
            "J_Grenadier",
            "J_Guard",
            "J_Machinegunner",
            "J_Officer",
            "J_Patrol",
            "J_Riflewiener",
            "J_Sniper",
            "Junkbot_Broken",
            "Junkbot_ElfBroken",
            "Junkbot_Flamer",
            "Junkbot_Heavy",
            "Junkbot_Patrol",
            "Junkbot_Pistoler",
            "Junkbot_Rocket",
            "Junkbot_Sniper",
            "Junkbot_Soldier",
            "Kolbasa_Boss_Kotleta",
            "Kolbasa_Boss_Pirozhok",
            "Kolbasa_Boss_Shashlyk",
            "Kolbasa_Boss_Svyokolnik",
            "Kolbasa_Boss_Tushyonka",
            "Kolbasa_PMC_Pistols",
            "Kolbasa_PMC_Rifle",
            "Kolbasa_PMC_SMGs",
            "Kolbasa_Scavenger_Pistols",
            "Kolbasa_Scavenger_Rifle",
            "Kolbasa_Scavenger_Shotguns",
            "Kolbasa_Scavenger_SMG",
            "Kolbasa_SweatyPMC_Rifle",
            "Kolbasa_SweatyPMG_Shotgun",
            "M_GreaseGremlins_Breacher",
            "M_GreaseGremlins_Guard",
            "M_GreaseGremlins_Heavy",
            "M_GreaseGremlins_Markswiener",
            "M_GreaseGremlins_Officer",
            "M_GreaseGremlins_Ranger",
            "M_GreaseGremlins_Riflewiener",
            "M_GreaseGremlins_Scout",
            "M_GreaseGremlins_Shield",
            "M_GreaseGremlins_Sniper",
            "M_GreaseGremlins_SpecOps",
            "M_MercWiener_Breacher",
            "M_MercWiener_Guard",
            "M_MercWiener_Heavy",
            "M_MercWiener_Markswiener",
            "M_MercWiener_Officer",
            "M_MercWiener_Ranger",
            "M_MercWiener_Riflewiener",
            "M_MercWiener_Scout",
            "M_MercWiener_Shield",
            "M_MercWiener_Sniper",
            "M_MercWiener_SpecOps",
            "M_Popsicles_Breacher",
            "M_Popsicles_Guard",
            "M_Popsicles_Heavy",
            "M_Popsicles_Markswiener",
            "M_Popsicles_Officer",
            "M_Popsicles_Ranger",
            "M_Popsicles_Riflewiener",
            "M_Popsicles_Scout",
            "M_Popsicles_Shield",
            "M_Popsicles_Sniper",
            "M_Popsicles_SpecOps",
            "M_Swat_Breacher",
            "M_Swat_Guard",
            "M_Swat_Heavy",
            "M_Swat_Markswiener",
            "M_Swat_Officer",
            "M_Swat_Ranger",
            "M_Swat_Riflewiener",
            "M_Swat_Scout",
            "M_Swat_Shield",
            "M_Swat_Sniper",
            "M_Swat_SpecOps",
            "M_VeggieDawgs_Breacher",
            "M_VeggieDawgs_Guard",
            "M_VeggieDawgs_Heavy",
            "M_VeggieDawgs_Markswiener",
            "M_VeggieDawgs_Officer",
            "M_VeggieDawgs_Ranger",
            "M_VeggieDawgs_Riflewiener",
            "M_VeggieDawgs_Scout",
            "M_VeggieDawgs_Shield",
            "M_VeggieDawgs_Sniper",
            "M_VeggieDawgs_SpecOps",
            "MF_BlueFranks_Demo",
            "MF_BlueFranks_Engineer",
            "MF_BlueFranks_Heavy",
            "MF_BlueFranks_Medic",
            "MF_BlueFranks_Pyro",
            "MF_BlueFranks_Scout",
            "MF_BlueFranks_Sniper",
            "MF_BlueFranks_Soldier",
            "MF_BlueFranks_Spy",
            "MF_RedHots_Demo",
            "MF_RedHots_Engineer",
            "MF_RedHots_Heavy",
            "MF_RedHots_Medic",
            "MF_RedHots_Pyro",
            "MF_RedHots_Scout",
            "MF_RedHots_Sniper",
            "MF_RedHots_Soldier",
            "MF_RedHots_Spy",
            "MG_Soldier_Artic_Rifle",
            "MG_Soldier_Chemwar_Rifle",
            "MG_Soldier_Heavy_Rifle",
            "MG_Soldier_Heavy_Shield",
            "MG_Soldier_LInfantry_Rifle",
            "MG_Soldier_LInfantry_Shotgun",
            "MG_Special_Boss",
            "MG_Special_Duelist",
            "MG_Special_Ninja",
            "MG_Special_Sniper",
            "MG_Special_Steak",
            "MG_Special_Support",
            "MG_Special_Telekine",
            "Misc_Dummy",
            "Misc_Elf",
            "MountainMeat_Melee",
            "MountainMeat_Pistol",
            "MountainMeat_Rifle",
            "MountainMeat_Shotgun",
            "MountainMeat_SMG",
            "RW_Beefkicker",
            "RW_Boner",
            "RW_Driller",
            "RW_Floater",
            "RW_FunGuy",
            "RW_HamFister",
            "RW_Lemonhead",
            "RW_OldSmokey",
            "RW_Pig",
            "RW_Prick",
            "RW_RedLumberjack",
            "RW_Rot",
            "RWP_Cultist",
            "RWP_PacSquad_Commander",
            "RWP_PacSquad_Flanker",
            "RWP_PacSquad_Sniper",
            "RWP_PacSquad_Trooper",
            "RWP_Prospector_Bar",
            "RWP_Prospector_Pistol",
            "RWP_Prospector_Rifle",
            "RWP_Prospector_Shotgun",
            "RWP_Skulker_Pistol",
            "RWP_Skulker_Rifler",
            "RWP_Skulker_Shotgun",
            "W_Brown_Antitank",
            "W_Brown_Flamewiener",
            "W_Brown_Grenadier",
            "W_Brown_Guard",
            "W_Brown_HeavyRiflewiener",
            "W_Brown_Machinegunner",
            "W_Brown_Officer",
            "W_Brown_Patrol",
            "W_Brown_Riflewiener",
            "W_Green_Antitank",
            "W_Green_Flamewiener",
            "W_Green_Grenadier",
            "W_Green_Guard",
            "W_Green_HeavyRiflewiener",
            "W_Green_Machinegunner",
            "W_Green_Officer",
            "W_Green_Patrol",
            "W_Green_Riflewiener",
            "W_Grey_Antitank",
            "W_Grey_Flamewiener",
            "W_Grey_Grenadier",
            "W_Grey_Guard",
            "W_Grey_HeavyRiflewiener",
            "W_Grey_Machinegunner",
            "W_Grey_Officer",
            "W_Grey_Patrol",
            "W_Grey_Riflewiener",
            "W_Tan_Antitank",
            "W_Tan_Flamewiener",
            "W_Tan_Grenadier",
            "W_Tan_Guard",
            "W_Tan_HeavyRiflewiener",
            "W_Tan_Machinegunner",
            "W_Tan_Officer",
            "W_Tan_Patrol",
            "W_Tan_Riflewiener"
        };

        static List<string> fileLocations = new List<string>
        {
                    "C:\\Users\\John\\AppData\\Roaming\\Thunderstore Mod Manager\\DataFolder\\H3VR\\profiles\\john\\BepInEx\\plugins\\localpcnerd-NuketownGunGame",
                    "C:\\Users\\John\\AppData\\Roaming\\Thunderstore Mod Manager\\DataFolder\\H3VR\\profiles\\john\\BepInEx\\plugins\\Kodeman-GunGame",
                    "C:\\Users\\John\\AppData\\Roaming\\Thunderstore Mod Manager\\DataFolder\\H3VR\\profiles\\john\\BepInEx\\plugins\\HLin_Mods-GunGame_Progressions",
                    "C:\\Users\\John\\AppData\\Roaming\\Thunderstore Mod Manager\\DataFolder\\H3VR\\profiles\\john\\BepInEx\\plugins\\GEnigma-SandpitGunGame"
        };

        static int maxCategories = 100;

        #endregion


        public static void generateJson()
        {
            loadGameObjects();
            loadCache();
            loadInGameIDs();
            buildJsonData();
        }


        public static void loadInGameIDs()
        {
            List<string> names = new List<string>();
            var am = new AssetsManager();
            am.LoadClassPackage("classdata.tpk");

            var ggm = am.LoadAssetsFile("globalgamemanagers", true);
            am.LoadClassDatabaseFromPackage(ggm.file.typeTree.unityVersion);

            var rsrcInfo = ggm.table.GetAssetsOfType((int)AssetClassID.ResourceManager)[0];
            var rsrcBf = am.GetTypeInstance(ggm, rsrcInfo).GetBaseField();

            var m_Container = rsrcBf.Get("m_Container").Get("Array");

            foreach (var data in m_Container.children)
            {
                var name = data[0].GetValue().AsString();
                var pathId = data[1].Get("m_PathID").GetValue().AsInt64();
                names.Add(name);
            }
            //
            foreach (string iString in itemSpawnerIDWhiteList)
            {
                List<string> temp = names.Where(n => n.Contains(iString)).ToList();
                foreach (string t in temp)
                {
                    itemSpawnerIDS.Add(t);
                }
            }

            foreach (string oString in objectIDSWhitelist)
            {
                List<string> temp = names.Where(n => n.Contains(oString)).ToList();
                foreach (string t in temp)
                {
                    objectIDS.Add(t);
                }
            }

            am.UnloadAllAssetsFiles();
        }

        public static void loadCache()
        {
            entryJson = File.ReadAllLines("CachedCompatibleMags.json").ToList();

            // Step 1, convert file into better format for deserializing into classes
            // Step1a, work from bottom of document up begining with bullet data
            int bulletStartIndex = entryJson.IndexOf("  \"BulletData\": {");
            int bulletEndIndex = entryJson.Count - 2;
            string bulletStartString = "  \"BulletData\": [";
            string endTag = "]";
            convertDataTags(bulletStartString, endTag, bulletStartIndex, bulletEndIndex);

            // Step 2 Speedloader data
            int speedLoaderStartIndex = entryJson.IndexOf("  \"SpeedLoaderData\": {");
            int speedLoaderEndIndex = bulletStartIndex - 1;
            string speedLoadStartString = "  \"SpeedLoaderData\": [";
            endTag = "],";
            convertDataTags(speedLoadStartString, endTag, speedLoaderStartIndex, speedLoaderEndIndex);

            // Step 3 clip data
            int clipStartIndex = entryJson.IndexOf("  \"ClipData\": {");
            int clipEndIndex = speedLoaderStartIndex - 1;
            string clipStartString = "  \"ClipData\": [";
            convertDataTags(clipStartString, endTag, clipStartIndex, clipEndIndex);

            // Step 4 magazine data
            int magazineStartIndex = entryJson.IndexOf("  \"MagazineData\": {");
            int magazineEndIndex = clipStartIndex - 1;
            string magazineStartString = "  \"MagazineData\": [";
            convertDataTags(magazineStartString, endTag, magazineStartIndex, magazineEndIndex);

            // Step 5 ammo objects
            int ammoStartIndex = entryJson.IndexOf("  \"AmmoObjects\": {");
            int ammoEndIndex = magazineStartIndex - 1;
            string ammoStartTag = "  \"AmmoObjects\": [";
            convertObjectTags(ammoStartTag, ammoStartIndex, ammoEndIndex);

            // Step 6 entries
            int entryStartIndex = entryJson.IndexOf("  \"Entries\": {");
            int entryEndIndex = ammoStartIndex - 1;
            string entryStartTag = "  \"Entries\": [";
            convertObjectTags(entryStartTag, entryStartIndex, entryEndIndex);

            // Step 7 deserialize into object
            File.WriteAllLines("CachedCompatibleMagsParsed.json", entryJson);
            string parsedJson = File.ReadAllText("CachedCompatibleMagsParsed.json");
            fullCache = JsonConvert.DeserializeObject<Cache>(parsedJson);
            File.Delete("CachedCompatibleMagsParsed.json");
        }

        public static void convertDataTags(string startTag, string endTag, int startIndex, int endIndex)
        {
            // Step 2a Convert tags to square braces
            entryJson[startIndex] = startTag;
            entryJson[endIndex] = endTag;

            // Step 2b Loop through bullet entries

            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (entryJson[i].Contains(": [") || entryJson[i].Contains("]"))
                {
                    entryJson[i] = "";
                }
                if (entryJson[i] == "      }")
                {
                    if (i != endIndex - 2)
                    {
                        entryJson[i] = "      },";
                    }
                    else
                    {
                        entryJson[i] = "      }";
                    }
                }
            }
        }

        public static void convertObjectTags(string startTag, int startIndex, int endIndex)
        {
            entryJson[startIndex] = startTag;
            entryJson[endIndex] = "],";
            for (int i = startIndex; i < endIndex; i++)
            {
                if (entryJson[i].Contains("{"))
                {
                    entryJson[i] = "{";
                }
            }
        }

        public static void loadGameObjects()
        {
            List<string> gameIDS = File.ReadAllLines("ObjectIDs.csv").ToList();
            for (int i = 1; i < gameIDS.Count; i++)
            {
                string gameID = gameIDS[i];
                GameObject gameObject = new GameObject();
                List<string> gameObjectString = gameID.Split(',').ToList();
                gameObject.ObjectID = gameObjectString[0];
                gameObject.Category = gameObjectString[1];
                gameObject.Era = gameObjectString[2];
                gameObject.Set = gameObjectString[3];
                gameObject.CountryofOrigin = gameObjectString[4];
                gameObject.AttachmentFeature = gameObjectString[5];
                gameObject.FirearmAction = gameObjectString[6];
                gameObject.FirearmFeedOption = gameObjectString[7];
                gameObject.FiringModes = gameObjectString[8];
                gameObject.FirearmMounts = gameObjectString[9];
                gameObject.AttachmentMount = gameObjectString[10];
                gameObject.RoundPower = gameObjectString[11];
                gameObject.Size = gameObjectString[12];
                gameObject.MeleeHandedness = gameObjectString[13];
                gameObject.MeleeStyle = gameObjectString[14];
                gameObject.PowerupType = gameObjectString[15];
                gameObject.ThrownDamageType = gameObjectString[16];
                gameObject.ThrownType = gameObjectString[17];

                gameObjects.Add(gameObject);
            }


        }

        public static void buildJsonData()
        {
            gunJson.guns = new ObservableCollection<Gun>();
            gunJson.nations = new List<string>();
            gunJson.eras = new List<string>();
            gunJson.firearmActions = new List<string>();
            gunJson.calibers = new List<string>();
            gunJson.categories = new List<string>();
            gunJson.enemies = enemies;
            gunJson.fileLocations = fileLocations;
            gunJson.maxCategories = maxCategories;

            // loop through all the entries
            foreach (Entry e in fullCache.Entries)
            {
                // First make sure Entry is valid
                GameObject go = gameObjects.Where(g => g.ObjectID == e.FirearmID).FirstOrDefault();



                if (go != null)
                {
                    if (!gunBlackList.Contains(go.ObjectID))
                    {
                        Gun gun = new Gun();
                        gun.CompatableAmmo = new List<string>();

                        gun.GunName = e.FirearmID;

                        gun.NationOfOrigin = go.CountryofOrigin;
                        gunJson.nations.Add(go.CountryofOrigin);

                        gun.Era = go.Era;
                        gunJson.eras.Add(go.Era);

                        gun.FirearmAction = go.FirearmAction;
                        gunJson.firearmActions.Add(go.FirearmAction);

                        gun.CategoryID = 0;
                        gun.SelctedMagName = "";

                        gun.Categories = new List<string>();

                        // add magazines
                        foreach (string m in e.CompatibleMagazines)
                        {
                            // check if magazine is in game
                            GameObject mag = gameObjects.Where(mo => mo.ObjectID == m).FirstOrDefault();
                            if (mag != null)
                            {
                                gun.CompatableAmmo.Add(m);
                                // add first mag as default
                                if (gun.CompatableAmmo.Count == 1)
                                {
                                    gun.DefaultMagName = m;
                                }
                            }
                        }

                        // add clips
                        foreach (string c in e.CompatibleClips)
                        {
                            GameObject clip = gameObjects.Where(co => co.ObjectID == c).FirstOrDefault();

                            if (clip != null)
                            {
                                gun.CompatableAmmo.Add(c);
                                // add first clip as default
                                if (gun.CompatableAmmo.Count == 1)
                                {
                                    gun.DefaultMagName = c;
                                }
                            }
                        }

                        // add bullets if gun does not take a magazine or a clip
                        if (gun.CompatableAmmo.Count == 0)
                        {
                            foreach (string b in e.CompatibleBullets)
                            {
                                GameObject bullet = gameObjects.Where(bo => bo.ObjectID == b).FirstOrDefault();
                                if (bullet != null)
                                {
                                    gun.CompatableAmmo.Add(b);
                                    // add first bullet as default if gun does not take speed loader
                                    if (gun.CompatableAmmo.Count == 1 && e.DoesUseSpeedloader == false)
                                    {
                                        gun.DefaultMagName = b;
                                    }
                                }
                            }

                        }

                        // add speed loaders
                        foreach (string s in e.CompatibleSpeedLoaders)
                        {
                            GameObject speedLoader = gameObjects.Where(so => so.ObjectID == s).FirstOrDefault();

                            if (speedLoader != null)
                            {
                                gun.CompatableAmmo.Add(s);
                                // add first  speed loader as default
                                gun.DefaultMagName = s;

                            }
                        }

                        // determine the caliber
                        List<string> nonMagAmmo = gun.CompatableAmmo.Where(s => !s.Contains("Mag")).ToList();

                        if (nonMagAmmo.Count > 1)
                        {
                            string common = nonMagAmmo[0];
                            for (int i = 1; i < nonMagAmmo.Count; i++)
                            {
                                common = string.Concat(common.TakeWhile((c, ii) => c == nonMagAmmo[i][ii]));
                            }
                            //trim _

                            gun.Caliber = common;
                        }
                        else
                        {
                            // find round type of ammo
                            List<BulletData> bullets = fullCache.BulletData.Where(b => b.RoundType == e.BulletType).ToList();
                            if (bullets.Count > 1)
                            {
                                string common = bullets[0].ObjectID;
                                for (int i = 1; i < bullets.Count; i++)
                                {
                                    common = string.Concat(common.TakeWhile((c, ii) => c == bullets[i].ObjectID[ii]));
                                }
                                common = common.Replace("_", "");
                                gun.Caliber = common;
                            }
                            if (bullets.Count == 0)
                            {
                                gun.Caliber = "Unknown";
                            }
                            else
                            {
                                List<string> bulletName = bullets[0].ObjectID.Split('_').ToList();
                                gun.Caliber = bulletName[0];
                            }
                        }
                        gunJson.calibers.Add(gun.Caliber);

                        // Find category
                        string itemSpawnerString = itemSpawnerIDS.Where(cs => cs.ToUpper().Contains(gun.GunName.ToUpper())).FirstOrDefault();
                        if (itemSpawnerString != null)
                        {
                            List<string> itemSpawnerID = itemSpawnerString.Split('/').ToList();
                            foreach (string i in itemSpawnerID)
                            {
                                if (i.ToUpper() != gun.GunName.ToUpper() && i != "itemspawnerids" && i != itemSpawnerID.Last())
                                {

                                    gunJson.categories.Add(categoryDictionary[i]);
                                    gun.Categories.Add(categoryDictionary[i]);
                                }
                            }
                        }
                        string objectIDString = objectIDS.Where(cs => cs.ToUpper().Contains(gun.GunName.ToUpper())).FirstOrDefault();
                        if (objectIDString != null)
                        {
                            List<string> objectID = objectIDString.Split('/').ToList();
                            foreach (string o in objectID)
                            {
                                if (o.ToUpper() != gun.GunName.ToUpper() && o != "objectids"
                                    && o != objectID.Last() && !o.Contains("weaponry_") && o != "mp5" && o != "guns")
                                {
                                    gunJson.categories.Add(categoryDictionary[o]);
                                    gun.Categories.Add(categoryDictionary[o]);
                                }
                            }
                        }

                        gun.Categories = gun.Categories.Distinct().ToList();
                        gun.Categories.Sort();
                        gunJson.guns.Add(gun);
                    }
                }

            }

            // Sort guns Animals = new ObservableCollection<string>(Animals.OrderBy(i => i));
            gunJson.guns = new ObservableCollection<Gun>(gunJson.guns.OrderBy(g => g.GunName));

            // remove duplicates from lists
            gunJson.nations = gunJson.nations.Distinct().ToList();
            gunJson.nations.Sort();

            gunJson.eras = gunJson.eras.Distinct().ToList();
            gunJson.eras.Sort();

            gunJson.firearmActions = gunJson.firearmActions.Distinct().ToList();
            gunJson.firearmActions.Sort();

            gunJson.calibers = gunJson.calibers.Distinct().ToList();
            gunJson.calibers.Sort();

            gunJson.categories = gunJson.categories.Distinct().ToList();
            gunJson.categories.Sort();

            string jsonString = JsonConvert.SerializeObject(gunJson, Formatting.Indented);

            File.WriteAllText("gameData.json", jsonString);
        }
    }
}
