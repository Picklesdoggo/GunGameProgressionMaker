using AssetsTools.NET;
using AssetsTools.NET.Extra;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace GunGameProgressionMaker
{
    public class JsonBuilder
    {
        #region Member Variables


        static Cache cache = new Cache();
        static Config config = new Config();

        #endregion


        public static void generateJson()
        {
            if (File.Exists("config.json"))
            {
                string configJson = File.ReadAllText("config.json");
                config = JsonConvert.DeserializeObject<Config>(configJson);
                cache.Items = new List<ItemSpawnerID>();
                cache.Objects = new List<ObjectID>();
                if (!File.Exists(config.gameResourcesPath))
                {
                    MessageBox.Show("Could not find game resources update config.json");
                    return;
                }
                if (!Directory.Exists(config.gameManagedPath))
                {
                    MessageBox.Show("Could not find game managed path update config.json");
                    return;
                }

               // loadFromAssets(config.gameResourcesPath, false, "Base Game");
                loadFromAssets(@"D:\noghiri-BB_Astra_Handguns\resources\a300-core", true, "MOD");

                #region MODS
                // load list of mod files
                //if (config.modsDirectory != null)
                //{
                //    List<string> modPaths = getModPaths();
                //    foreach (string modPath in modPaths)
                //    {
                //        if(File.Exists(modPath))
                //        {
                //            try
                //            {
                //                string modDirectory = Path.GetDirectoryName(modPath);
                //                // Find manifest json file
                //                string manifestPath = modDirectory + @"\\manifest.json";
                //                if (File.Exists(manifestPath))
                //                {
                //                    string manifestJsonString = File.ReadAllText(manifestPath);
                //                    ManifestJSON manifestJSON = JsonConvert.DeserializeObject<ManifestJSON>(manifestJsonString);
                //                  //  MessageBox.Show("Processing Mod " + manifestJSON.name + "\r\n\r\n" + modPath);
                //                    loadFromAssets(modPath, true, manifestJSON.name);
                //                }
                //                else
                //                {
                //                    bool manifestFound = false;
                //                    while (!manifestFound)
                //                    {
                //                        string currentFolder = Path.GetDirectoryName(modPath);
                //                        string parentFolder = Directory.GetParent(currentFolder).FullName;
                //                        manifestPath = parentFolder + @"\\manifest.json";
                //                        if (File.Exists(manifestPath))
                //                        {
                //                            string manifestJsonString = File.ReadAllText(manifestPath);
                //                            ManifestJSON manifestJSON = JsonConvert.DeserializeObject<ManifestJSON>(manifestJsonString);
                //                           // MessageBox.Show("Processing Mod " + manifestJSON.name + "\r\n\r\n" + modPath);
                //                            loadFromAssets(modPath, true, manifestJSON.name);
                //                            manifestFound = true;
                //                        }
                //                    }
                //                }
                //            }
                //            catch(Exception ex)
                //            {
                //                MessageBox.Show("Unable to process " + modPath + " " + ex.Message);
                //            }
                //        }
                //        else
                //        {
                //            MessageBox.Show("Could not find file " + modPath);
                //        }

                //    }
                //}
                //else
                //{
                //    MessageBox.Show("No mod path detected, edit config.json if desired");
                //}

                //if (config.manuallyLoadedMods != null)
                //{
                //    foreach(string manualModPath in config.manuallyLoadedMods)
                //    {
                //        if(File.Exists(manualModPath))
                //        {
                //            try
                //            {
                //                string modDirectory = Path.GetDirectoryName(manualModPath);
                //                // Find manifest json file
                //                string manifestPath = modDirectory + @"\\manifest.json";
                //                if (File.Exists(manifestPath))
                //                {
                //                    string manifestJsonString = File.ReadAllText(manifestPath);
                //                    ManifestJSON manifestJSON = JsonConvert.DeserializeObject<ManifestJSON>(manifestJsonString);
                //                    loadFromAssets(manualModPath, true, manifestJSON.name);
                //                }
                //                else
                //                {
                //                    bool manifestFound = false;
                //                    while (!manifestFound)
                //                    {
                //                        string currentFolder = Path.GetDirectoryName(manualModPath);
                //                        string parentFolder = Directory.GetParent(currentFolder).FullName;
                //                        manifestPath = parentFolder + @"\\manifest.json";
                //                        if (File.Exists(manifestPath))
                //                        {
                //                            string manifestJsonString = File.ReadAllText(manifestPath);
                //                            ManifestJSON manifestJSON = JsonConvert.DeserializeObject<ManifestJSON>(manifestJsonString);
                //                            loadFromAssets(manualModPath, true, manifestJSON.name);
                //                            manifestFound = true;
                //                        }
                //                    }
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                MessageBox.Show("Unable to process " + manualModPath + " " + ex.Message);
                //            }
                //        }
                //    }
                //} 
                #endregion

                buildJSON();
            }


        }

        public static List<string> getModPaths()
        {
            List<string> paths = new List<string>();
            // get a list of all folders containing 
            foreach (string file in Directory.EnumerateFiles(config.modsDirectory, "*.*", SearchOption.AllDirectories))
            {
                if (file.Contains("late_"))
                {

                    string assetFile = file.Replace("late_", "");
                    paths.Add(assetFile);
                }
            }
            return paths;
        }

        public static void loadFromAssets(string filePath, bool isMod, string modName)
        {
            List<AssetTypeValueField> objectIDsRaw = new List<AssetTypeValueField>();
    

            // Paths
            string GameManagedPath = config.gameManagedPath;
            AssetsManager manager = new AssetsManager();
            manager.LoadClassPackage("classdata.tpk");
            AssetsFileInstance afileInst;

           
            if (isMod)
            {
                var bunInst = manager.LoadBundleFile(filePath, true);
                afileInst = manager.LoadAssetsFileFromBundle(bunInst, 0, false);
            }
            else
            {
                afileInst = manager.LoadAssetsFile(filePath, true);
            }

            
            var afile = afileInst.file;
           
            manager.LoadClassDatabaseFromPackage(afile.Metadata.UnityVersion);

            manager.MonoTempGenerator = new MonoCecilTempGenerator(config.gameManagedPath);

            foreach (var goInfo in afile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {
                var baseField = manager.GetBaseField(afileInst, goInfo.PathId);
                var scriptType = manager.GetExtAsset(afileInst, baseField["m_Script"]).baseField;
                var scriptName = scriptType.Get("m_Name").Value.ToString();


                if (scriptName == "ItemSpawnerID")
                {
                    int category = baseField["Category"].Value.AsInt;

                    if (category > 4)
                    {
                        continue;
                    }


                    ItemSpawnerID item = new ItemSpawnerID();
                    item.Category = (EItemCategory)baseField["Category"].AsInt;
                    item.SubCategory = (EItemSubCategory)baseField["SubCategory"].AsInt;
                    item.SpawnFromID = baseField["ItemID"].AsString;
                    item.Caliber = baseField["SubHeading"].AsString;
                    cache.Items.Add(item);
                }

                else if (scriptName == "FVRObject")
                {
                    
                    EObjectCategory category = (EObjectCategory)baseField["Category"].Value.AsInt;

                    if (category == EObjectCategory.Firearm ||
                        category == EObjectCategory.Magazine ||
                        category == EObjectCategory.Clip ||
                        category == EObjectCategory.Cartridge ||
                        category == EObjectCategory.SpeedLoader ||
                        category == EObjectCategory.Attachment)
                    {
                        // Add to list
                        objectIDsRaw.Add(baseField);

                        foreach (var tempa in baseField.Children)
                        {
                            Console.WriteLine(tempa.FieldName);
                        }

                        ObjectID item = new ObjectID();
                        item.ItemID = baseField["ItemID"].AsString;

                        var magazineType = baseField["MagazineType"];
                        if (magazineType.Value != null)
                        {
                            item.MagazineType = magazineType.AsInt;
                        }
                        
                        var clipType = baseField["ClipType"];
                        if (clipType.Value != null)
                        {
                            item.ClipType = clipType.AsInt;
                        }

                        var roundType = baseField["RoundType"];
                        if (roundType.Value != null)
                        {
                            item.RoundType = roundType.AsInt;
                        }
                        



                        item.SpawnFromID = baseField["SpawnedFromId"].AsString;

                        item.FiringModes = new List<EFirearmFiringMode>();
                        item.Category = (EObjectCategory)baseField["Category"].AsInt;
                        item.Era = (ETagEra)baseField["TagEra"].AsInt;
                        item.Set = (ETagSet)baseField["TagSet"].AsInt;
                        item.FirearmSize = (ETagFirearmSize)baseField["TagFirearmSize"].AsInt;
                        item.FirearmAction = (ETagFirearmAction)baseField["TagFirearmAction"].AsInt;
                        item.FirearmMounts = new List<ETagFirearmMount>();
                        item.AttachmentMount = (ETagFirearmMount)baseField["TagAttachmentMount"].AsInt;
                        item.ModName = modName;
                        cache.Objects.Add(item);
                    }
                    else
                    {
                        continue;
                    }

                }
            }

         


            manager.UnloadAllAssetsFiles();




        }

        private static AssetIcon GetIconForName(string type)
        {
            if (Enum.TryParse(type, out AssetIcon res))
            {
                return res;
            }
            return AssetIcon.Unknown;
        }

        public static void buildJSON()
        {
            string configJson = File.ReadAllText("config.json");
            Config config = JsonConvert.DeserializeObject<Config>(configJson);

            List<ObjectID> guns = cache.Objects.Where(g => g.Category == EObjectCategory.Firearm).ToList();
            List<ObjectID> attachments = cache.Objects.Where(a => a.Category == EObjectCategory.Attachment).ToList();

            InputJson gunJson = new InputJson()
            {

                calibers = new List<string>(),
                categories = new List<string>(),
                enemies = config.enemies,
                enemyCategories = new List<string>(),
                eras = new List<string>(),
                fileLocations = config.filelocations,
                firearmactions = new List<string>(),
                guns = new List<Gun>(),
                nations = new List<string>(),
                maxCategories = config.maxcategories,
                extras = new List<Extras>(),
                extraCategories = new List<string>(),
                mods = new List<string>()
            };

            // Add guns
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
                gun.CompatibleExtras = new List<Extras>();
                gun.UsesSpeedLoader = gunObject.UsesSpeedloader;
                gun.ModName = gunObject.ModName;

                if (!gunJson.mods.Contains(gunObject.ModName))
                {
                    gunJson.mods.Add(gunObject.ModName);
                }

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

                // Non-bespoke Extras
                foreach (ETagFirearmMount a in gunObject.FirearmMounts)
                {
                    List<ObjectID> compatibleAttachments = cache.Objects.Where(p => p.AttachmentMount == a && p.AttachmentMount != ETagFirearmMount.Bespoke).ToList();
                    foreach (ObjectID compatibleAttachment in compatibleAttachments)
                    {

                        ItemSpawnerID itemExtra = cache.Items.Where(i => i.SpawnFromID == compatibleAttachment.SpawnFromID).FirstOrDefault();
                        if (itemExtra != null)
                        {
                            Extras extra = new Extras();
                            extra.ExtraName = compatibleAttachment.ItemID;
                            extra.SubCategory = itemExtra.SubCategoryDisplay;
                            extra.AttachmentType = a;
                            gun.CompatibleExtras.Add(extra);
                        }
                    }
                }

                // bespoke extras
                foreach (ObjectID bespokeExtra in gunObject.BespokeAttachments)
                {
                    ItemSpawnerID itemBespoke = cache.Items.Where(i => i.SpawnFromID == bespokeExtra.SpawnFromID).FirstOrDefault();
                    if (itemBespoke != null)
                    {
                        Extras extra = new Extras();
                        extra.ExtraName = bespokeExtra.ItemID;
                        extra.SubCategory = itemBespoke.SubCategoryDisplay;
                        extra.AttachmentType = bespokeExtra.AttachmentMount;
                        gun.CompatibleExtras.Add(extra);
                    }
                }

                // Sort extras by attachment type, sub category, name
                gun.CompatibleExtras = gun.CompatibleExtras.OrderBy(g => g.AttachmentType).ThenBy(g => g.SubCategory).ThenBy(g => g.ExtraName).ToList();

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
            List<string> tempSubCategories = new List<string>();
            // add attachments
            foreach (ObjectID attachment in attachments)
            {

                // Make temp list of categories
                // Get info from the ItemSpawner
                ItemSpawnerID attachmentID = cache.Items.Where(a => a.SpawnFromID == attachment.SpawnFromID).FirstOrDefault();
                if (attachmentID != null)
                {
                    Extras extra = new Extras();
                    extra.ExtraName = attachment.ItemID;
                    extra.SubCategory = attachmentID.SubCategory.ToString();
                    gunJson.extras.Add(extra);

                }
            }

            // update enemy categories

            foreach (Enemy e in config.enemies)
            {
                if (!gunJson.enemyCategories.Contains(e.category))
                {
                    gunJson.enemyCategories.Add(e.category);
                }
            }

            // update extra caterogires
            foreach (Extras e in gunJson.extras)
            {
                if (!gunJson.extraCategories.Contains(e.SubCategory))
                {
                    gunJson.extraCategories.Add(e.SubCategory);
                }
            }
            // Sort lists
            gunJson.calibers.Sort();
            gunJson.categories.Sort();
            gunJson.eras.Sort();
            gunJson.nations.Sort();
            gunJson.extraCategories.Sort();

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
