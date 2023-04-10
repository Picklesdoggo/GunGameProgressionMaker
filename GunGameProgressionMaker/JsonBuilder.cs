using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

                // load base objects first
                loadFromAssets(config.gameResourcesPath, false);

                // load list of mod files
                if (config.modsDirectory != null)
                {
                    List<string> modPaths = getModPaths();
                    foreach (string modPath in modPaths)
                    {
                        if(File.Exists(modPath))
                        {
                            try
                            {
                                loadFromAssets(modPath, true);
                            }
                            catch
                            {
                                MessageBox.Show("Unable to process " + modPath);
                            }
                        }
                        
                    }
                }
                else
                {
                    MessageBox.Show("No mod path detected, edit config.json if desired");
                }

                if (config.manuallyLoadedMods != null)
                {
                    foreach(string manualModPath in config.manuallyLoadedMods)
                    {
                        if(File.Exists(manualModPath))
                        {
                            try
                            {
                                loadFromAssets(manualModPath, true);
                            }
                            catch
                            {
                                MessageBox.Show("Unable to process " + manualModPath);
                            }
                        }
                    }
                }

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

        public static void loadFromAssets(string path, bool isMod)
        {

            // Paths
            string GameManagedPath = config.gameManagedPath;

            // Setup the assets manager
            AssetsManager am = new AssetsManager();
            AssetsFileInstance inst;
            if (isMod)
            {
                BundleFileInstance bfi = am.LoadBundleFile(path);
                inst = am.LoadAssetsFileFromBundle(bfi, 0);
            }
            else
            {
                inst = am.LoadAssetsFile(path, true);
            }

            am.LoadClassPackage("classdata.tpk");
            am.LoadClassDatabaseFromPackage(inst.file.typeTree.unityVersion);

            // List to keep track of our Item Spawner IDs

            List<AssetTypeValueField> itemSpawnerIDsRaw = new List<AssetTypeValueField>();


            List<AssetTypeValueField> objectIDsRaw = new List<AssetTypeValueField>();


            foreach (var inf in inst.table.GetAssetsOfType((int)AssetClassID.GameObject))
            {
                var baseField = am.GetTypeInstance(inst, inf).GetBaseField();

                var name = baseField.Get("m_Name").GetValue().AsString();
                Console.WriteLine(name);
            }


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
                    int temp = monoBf["Category"].GetValue().AsInt();
                    if (monoBf["Category"].GetValue().AsInt() > 4)
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
                    string temp = monoBf["ItemID"].GetValue().AsString();

                    // Skip any non-firearm or ammo object
                    if ((EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Firearm ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Magazine ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Clip ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Cartridge ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.SpeedLoader ||
                        (EObjectCategory)monoBf["Category"].GetValue().AsInt() == EObjectCategory.Attachment)
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
                item.FirearmMounts = new List<ETagFirearmMount>();
                item.AttachmentMount = (ETagFirearmMount)obj["TagAttachmentMount"].GetValue().AsInt();

                List<AssetTypeValueField> firingModes = obj["TagFirearmFiringModes"].GetChildrenList().ToList();
                foreach (AssetTypeValueField mode in firingModes)
                {
                    item.FiringModes.Add((EFirearmFiringMode)mode.GetValue().AsInt());
                }

                List<AssetTypeValueField> firearmMounts = obj["TagFirearmMounts"].GetChildrenList().ToList();
                foreach (AssetTypeValueField mount in firearmMounts)
                {
                    item.FirearmMounts.Add((ETagFirearmMount)mount.GetValue().AsInt());
                }

                List<AssetTypeValueField> bespokeAttachments = obj["BespokeAttachments"].GetChildrenList().ToList();
                foreach (AssetTypeValueField bespoke in bespokeAttachments)
                {
                    int bespokeFileID = bespoke["m_FileID"].GetValue().AsInt();
                    int bespokePathID = bespoke["m_PathID"].GetValue().AsInt();
                    AssetExternal bespokeExt = am.GetExtAsset(inst, bespokeFileID, bespokePathID, true);
                    AssetTypeValueField bespokeObj = MonoDeserializer.GetMonoBaseField(am, bespokeExt.file, bespokeExt.info, GameManagedPath);
                    ObjectID bespkoeItem = new ObjectID();
                    bespkoeItem.MagazineType = bespokeObj["MagazineType"].GetValue().AsInt();
                    bespkoeItem.ClipType = bespokeObj["ClipType"].GetValue().AsInt();
                    bespkoeItem.RoundType = bespokeObj["RoundType"].GetValue().AsInt();
                    bespkoeItem.SpawnFromID = bespokeObj["SpawnedFromId"].GetValue().AsString();
                    bespkoeItem.ItemID = bespokeObj["ItemID"].GetValue().AsString();
                    bespkoeItem.FiringModes = new List<EFirearmFiringMode>();
                    bespkoeItem.Category = (EObjectCategory)bespokeObj["Category"].GetValue().AsInt();
                    bespkoeItem.Era = (ETagEra)bespokeObj["TagEra"].GetValue().AsInt();
                    bespkoeItem.Set = (ETagSet)bespokeObj["TagSet"].GetValue().AsInt();
                    bespkoeItem.FirearmSize = (ETagFirearmSize)bespokeObj["TagFirearmSize"].GetValue().AsInt();
                    bespkoeItem.FirearmAction = (ETagFirearmAction)bespokeObj["TagFirearmAction"].GetValue().AsInt();
                    bespkoeItem.FirearmMounts = new List<ETagFirearmMount>();
                    bespkoeItem.AttachmentMount = (ETagFirearmMount)bespokeObj["TagAttachmentMount"].GetValue().AsInt();
                    item.BespokeAttachments.Add(bespkoeItem);
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
                extraCategories = new List<string>()
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
