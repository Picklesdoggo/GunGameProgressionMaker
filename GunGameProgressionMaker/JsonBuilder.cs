using AssetsTools.NET;
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


        static Cache cache = new Cache();


        #endregion

           
        public static void generateJson()
        {
            loadFromAssets();
            buildJSON();

        }

        public static void loadFromAssets()
        {

            // Paths
            const string GameResourcesPath = @"C:\Program Files (x86)\Steam\steamapps\common\H3VR\h3vr_Data\resources.assets";
            const string GameManagedPath = @"C:\Program Files (x86)\Steam\steamapps\common\H3VR\h3vr_Data\Managed";

            // Setup the assets manager
            AssetsManager am = new AssetsManager();
            AssetsFileInstance inst = am.LoadAssetsFile(GameResourcesPath, true);
            am.LoadClassPackage("classdata.tpk");
            am.LoadClassDatabaseFromPackage(inst.file.typeTree.unityVersion);

            // List to keep track of our Item Spawner IDs

            List<AssetTypeValueField> itemSpawnerIDsRaw = new List<AssetTypeValueField>();
            cache.Items = new List<ItemSpawnerID>();

            List<AssetTypeValueField> objectIDsRaw = new List<AssetTypeValueField>();
            cache.Objects = new List<ObjectID>();

            // Find all MonoBehaviour Assets in the resources file
            foreach (var inf in inst.table.GetAssetsOfType((int)AssetClassID.MonoBehaviour))
            {
                // Get the name of the script this object is
                var baseField = am.GetTypeInstance(inst, inf).GetBaseField();
                var scriptType = am.GetExtAsset(inst, baseField["m_Script"]).instance.GetBaseField();
                var scriptName = scriptType.Get("m_Name").GetValue().AsString();

                //Console.WriteLine(scriptName);

                // Add to list if is ItemSpawnerID
                if (scriptName == "ItemSpawnerID")
                {
                    // Get the full data for this script
                    var monoBf = MonoDeserializer.GetMonoBaseField(am, inst, inf, GameManagedPath);
                    // Skip any non-firearm object
                    if (monoBf["Category"].GetValue().AsInt() > 3)
                    {
                        continue;
                    }

                    // Add to list
                    itemSpawnerIDsRaw.Add(monoBf);


                }
                else if (scriptName == "FVRObject")
                {
                    // Get the full data for this script
                    var monoBf = MonoDeserializer.GetMonoBaseField(am, inst, inf, GameManagedPath);
                    int temp = monoBf["Category"].GetValue().AsInt();
                    // Skip any non-firearm or ammo object
                    if ((EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Firearm ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Magazine ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Clip ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Cartridge ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.SpeedLoader)
                    {
                        // Add to list
                        objectIDsRaw.Add(monoBf);
                    }
                    else
                    {
                        continue;
                    }


                }
                else
                {
                    continue;
                }
            }
            foreach (var obj in itemSpawnerIDsRaw)
            {
                ItemSpawnerID item = new ItemSpawnerID();
                item.Category = (EItemCategory)obj["Category"].GetValue().AsInt();
                item.SubCategory = (EItemSubCategory)obj["SubCategory"].GetValue().AsInt();
                item.SpawnFromID = obj["ItemID"].GetValue().AsString();
                item.Caliber = obj["SubHeading"].GetValue().AsString();
                cache.Items.Add(item);

            }

            foreach (var obj in objectIDsRaw)
            {

                ObjectID item = new ObjectID();
                item.MagazineType = obj["MagazineType"].GetValue().AsInt();
                item.ClipType = obj["ClipType"].GetValue().AsInt();
                item.RoundType = obj["RoundType"].GetValue().AsInt();
                item.SpawnFromID = obj["SpawnedFromId"].GetValue().AsString();
                item.ItemID = obj["ItemID"].GetValue().AsString();
                item.FiringModes = new List<EFirearmFiringMode>();
                item.Category = (EObjectCategory)obj["Category"].GetValue().AsInt();
                item.Era = (ETagEra)obj["TagEra"].GetValue().AsInt();
                item.Set = (ETagSet)obj["TagSet"].GetValue().AsInt();
                item.FirearmSize = (ETagFirearmSize)obj["TagFirearmSize"].GetValue().AsInt();
                item.FirearmAction = (ETagFirearmAction)obj["TagFirearmAction"].GetValue().AsInt();

                List<AssetTypeValueField> firingModes = obj["TagFirearmFiringModes"].GetChildrenList().ToList();
                foreach (AssetTypeValueField mode in firingModes)
                {
                    item.FiringModes.Add((EFirearmFiringMode)mode.GetValue().AsInt());
                }

                List<AssetTypeValueField> compatibleSpeedLoaders = obj["CompatibleSpeedLoaders"].GetChildrenList().ToList();

                if (compatibleSpeedLoaders.Count != 0)
                {
                    item.UsesSpeedloader = true;

                }
                else
                {
                    item.UsesSpeedloader = false;
                }

                item.FirearmRoundPower = (ETagFirearmRoundPower)obj["TagFirearmRoundPower"].GetValue().AsInt();
                item.CountryOfOrigin = (ETagFirearmCountryOfOrigin)obj["TagFirearmCountryOfOrigin"].GetValue().AsInt();

                cache.Objects.Add(item);

            }
            am.UnloadAllAssetsFiles();

            string jsonString = JsonConvert.SerializeObject(cache, Formatting.Indented);

           
        }

        public static void buildJSON()
        {
            string configJson = File.ReadAllText("config.json");
            Config config = JsonConvert.DeserializeObject<Config>(configJson);

            List<ObjectID> guns = cache.Objects.Where(g => g.Category == EObjectCategory.Firearm).ToList();

            InputJson gunJson = new InputJson()
            {

                calibers = new List<string>(),
                categories = new List<string>(),
                enemies = config.enemies,
                eras = new List<string>(),
                fileLocations = config.filelocations,
                firearmactions = new List<string>(),
                guns = new List<Gun>(),
                nations = new List<string>()
            };

            foreach (ObjectID gunObject in guns)
            {
                // skip blacklist
                if (config.gunblacklist.Contains(gunObject.ItemID))
                {
                    continue;
                }

                Gun gun = new Gun();
                gun.CategoryID = 0;
                gun.SelctedMagName = "";
                gun.DefaultMagName = "";

                gun.UsesSpeedLoader = gunObject.UsesSpeedloader;

                gun.NationOfOrigin = gunObject.CountryOfOriginDisplay;
                if (!gunJson.nations.Contains(gun.NationOfOrigin))
                {
                    gunJson.nations.Add(gun.NationOfOrigin);
                }

                gun.Era = gunObject.EraDisplay;
                if (!gunJson.eras.Contains(gun.Era))
                {
                    gunJson.eras.Add(gun.Era);
                }

                gun.GunName = gunObject.ItemID;

                gun.Categories = new List<string>();

                #region Compatible Ammo
                gun.CompatableAmmo = new List<string>();

                // Find compatible ammo

                // Start with magazines
                if (gunObject.MagazineType != 0)
                {
                    List<ObjectID> compatibleMags = cache.Objects.Where(o => o.MagazineType == gunObject.MagazineType
                                                                       && o.Category == EObjectCategory.Magazine).ToList();

                    foreach (ObjectID mag in compatibleMags)
                    {
                        if (gun.DefaultMagName == "")
                        {
                            gun.DefaultMagName = mag.ItemID;
                        }
                        gun.CompatableAmmo.Add(mag.ItemID);
                    }
                }
                // Get clips next
                if (gunObject.ClipType != 0)
                {
                    List<ObjectID> compatibleClips = cache.Objects.Where(o => o.ClipType == gunObject.ClipType
                                                   && o.Category == EObjectCategory.Clip).ToList();

                    foreach (ObjectID clip in compatibleClips)
                    {
                        if (gun.DefaultMagName == "")
                        {
                            gun.DefaultMagName = clip.ItemID;
                        }
                        gun.CompatableAmmo.Add(clip.ItemID);
                    }
                }

                // Get bullets if gun does not take a magazine
                if (gunObject.MagazineType == 0)
                {

                    if (gun.UsesSpeedLoader && gun.GunName != "P6Twelve" && gun.GunName != "Jackhammer")
                    {
                        // Speedloaders first
                        List<ObjectID> compatibleSpeedloader = cache.Objects.Where(o => o.RoundType == gunObject.RoundType
                                   && o.Category == EObjectCategory.SpeedLoader).ToList();

                        foreach (ObjectID speedLoader in compatibleSpeedloader)
                        {
                            if (gun.DefaultMagName == "")
                            {
                                gun.DefaultMagName = speedLoader.ItemID;
                            }
                            gun.CompatableAmmo.Add(speedLoader.ItemID);
                        }
                    }
                    // Cartridges next
                    List<ObjectID> compatibleCartridges = cache.Objects.Where(o => o.RoundType == gunObject.RoundType
                               && o.Category == EObjectCategory.Cartridge).ToList();

                    foreach (ObjectID cartridge in compatibleCartridges)
                    {
                        if (gun.DefaultMagName == "")
                        {
                            gun.DefaultMagName = cartridge.ItemID;
                        }
                        gun.CompatableAmmo.Add(cartridge.ItemID);
                    }

                }
                #endregion

                // Populate categories

                // Firing Mode
                foreach (string m in gunObject.FiringModesDisplay)
                {
                    if (!gun.Categories.Contains(m))
                    {
                        gun.Categories.Add(m);
                    }
                    if (!gunJson.categories.Contains(m))
                    {
                        gunJson.categories.Add(m);
                    }
                }

                // Set
                if (!gun.Categories.Contains(gunObject.SetDisplay))
                {
                    gun.Categories.Add(gunObject.SetDisplay);

                    if (!gunJson.categories.Contains(gunObject.SetDisplay))
                    {
                        gunJson.categories.Add(gunObject.SetDisplay);
                    }
                }

                // Firearm Size
                if (!gun.Categories.Contains(gunObject.FirearmSizeDisplay))
                {
                    gun.Categories.Add(gunObject.FirearmSizeDisplay);

                    if (!gunJson.categories.Contains(gunObject.FirearmSizeDisplay))
                    {
                        gunJson.categories.Add(gunObject.FirearmSizeDisplay);
                    }
                }

                // Firearm Round Power
                if (!gun.Categories.Contains(gunObject.FirearmRoundPowerDisplay))
                {
                    gun.Categories.Add(gunObject.FirearmRoundPowerDisplay);

                    if (!gunJson.categories.Contains(gunObject.FirearmRoundPowerDisplay))
                    {
                        gunJson.categories.Add(gunObject.FirearmRoundPowerDisplay);
                    }
                }

                // Firearm Actions
                gun.FirearmAction = gunObject.FirearmActionDisplay;

                if (!gunJson.firearmactions.Contains(gunObject.FirearmActionDisplay))
                {
                    gunJson.firearmactions.Add(gunObject.FirearmActionDisplay);
                }


                // Get info from the ItemSpawner
                ItemSpawnerID itemGun = cache.Items.Where(i => i.SpawnFromID == gunObject.SpawnFromID).FirstOrDefault();


                if (itemGun != null)
                {
                    // Caliber
                    if (itemGun.Caliber == "")
                    {
                        itemGun.Caliber = "Unknown";
                    }
                    gun.Caliber = itemGun.Caliber;
                    if (!gunJson.calibers.Contains(itemGun.Caliber))
                    {
                        gunJson.calibers.Add(itemGun.Caliber);
                    }



                    // Category
                    if (!gun.Categories.Contains(itemGun.CategoryDisplay))
                    {
                        gun.Categories.Add(itemGun.CategoryDisplay);
                    }

                    if (!gunJson.categories.Contains(itemGun.CategoryDisplay))
                    {
                        gunJson.categories.Add(itemGun.CategoryDisplay);
                    }

                    // SubCategory
                    if (!gun.Categories.Contains(itemGun.SubCategoryDisplay))
                    {
                        gun.Categories.Add(itemGun.SubCategoryDisplay);
                    }

                    if (!gunJson.categories.Contains(itemGun.SubCategoryDisplay))
                    {
                        gunJson.categories.Add(itemGun.SubCategoryDisplay);
                    }

                }

                gun.Categories.Sort();
                gunJson.guns.Add(gun);
            }

            // Sort lists
            gunJson.calibers.Sort();
            gunJson.categories.Sort();
            gunJson.eras.Sort();
            gunJson.nations.Sort();

            gunJson.guns = gunJson.guns.OrderBy(g => g.GunName).ToList();

            string jsonString = JsonConvert.SerializeObject(gunJson, Formatting.Indented);

            File.WriteAllText("gameData.json", jsonString);

            List<string> gunNames = new List<string>();
            foreach (Gun g in gunJson.guns)
            {
                gunNames.Add(g.GunName);
            }
        }


     
       

    
    }
}
