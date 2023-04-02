using GunGameProgressionMakerAdvanced;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace GunGameProgressionMaker
{
    /// <summary>
    /// Interaction logic for AdvancedWindow.xaml
    /// </summary>
    public partial class AdvancedWindow : Window
    {
        InputJson allGameData = new InputJson();
        GunGameProgressionMakerAdvanced.AdvancedOutput advancedOutput = new AdvancedOutput();
        ObservableCollection<GunGameProgressionMakerAdvanced.Enemy> selectedEnemies = new ObservableCollection<GunGameProgressionMakerAdvanced.Enemy>();
        ObservableCollection<GunGameProgressionMakerAdvanced.Gun> selectedGuns = new ObservableCollection<GunGameProgressionMakerAdvanced.Gun>();
        public AdvancedWindow()
        {
            InitializeComponent();

            getGameData();

            populateDropDowns();
            grdEnemies.ItemsSource = selectedEnemies;
            grdGuns.ItemsSource = selectedGuns;
        }

        private void getGameData()
        {
            // load JSON file
            string json = File.ReadAllText("gameData.json");

            allGameData = JsonConvert.DeserializeObject<InputJson>(json);
        }

        private void populateDropDowns()
        {
            // Populate Order drop down
            cmbOrderType.Items.Clear();
            cmbOrderType.Items.Add("0 - Fixed - weapons will spawn in specific order.");
            cmbOrderType.Items.Add("1 - Random - weapons will spawn in random order.");
            cmbOrderType.Items.Add("2 - Random within category- weapons will be spawned randomly but will go through category.");
            cmbOrderType.SelectedIndex = 1;

            // Populate Enemy Progression Type drop down
            cmbEnemyProgressionType.Items.Clear();
            cmbEnemyProgressionType.Items.Add("0 - Count - Default.Kill this many enemies to progress.");
            cmbEnemyProgressionType.Items.Add("1 - Points - Different enemy types have different points.Reach enough points to progress.");
            cmbEnemyProgressionType.Items.Add("2 - Tiers - Keep spawning enemies of one type until enough of them are killed, then proceed to spawn the next type, and promoting when all tiers are cleared");
            cmbEnemyProgressionType.SelectedIndex = 0;

            cmbEnemyName.Items.Clear();
            cmbEnemyCategory.Items.Clear();
            // Populate enemy drop downs
            foreach (string e in allGameData.enemyCategories)
            {
                cmbEnemyCategory.Items.Add(e);
            }

            cmbGuns.Items.Clear();
            // Populate advancedGun drop down
            foreach (Gun g in allGameData.guns)
            {
                cmbGuns.Items.Add(g.GunName);
            }

            // Populate Era drop down with check boxes
            cmbEraFilter.Items.Clear();
            populateDrowDown(allGameData.eras, cmbEraFilter);

            // Populate Category drop down with check boxes
            cmbCategoryFilter.Items.Clear();
            populateDrowDown(allGameData.categories, cmbCategoryFilter);

            // Populate Nation drop down with check boxes
            cmbNationFilter.Items.Clear();
            populateDrowDown(allGameData.nations, cmbNationFilter);

            // Populate Caliber drop down with check boxes
            cmbCaliberFilter.Items.Clear();
            populateDrowDown(allGameData.calibers, cmbCaliberFilter);

            // Populate Firearm action drop down with check boxes
            cmbFirearmActionFilter.Items.Clear();
            populateDrowDown(allGameData.firearmactions, cmbFirearmActionFilter);

            // Populate categoryID drop down
            cmbCategoryID.Items.Clear();
            for (int i = 0; i <= allGameData.maxCategories; i++)
            {
                cmbCategoryID.Items.Add(i);
            }

            cmbCategoryID.SelectedIndex = 0;           
        }

        private void populateDrowDown(List<string> items, ComboBox box)
        {

            // add select all
            CheckBox SelectAll = new CheckBox();
            SelectAll.Content = "Select All";
            SelectAll.IsChecked = true;
            SelectAll.Click += Filter_Click;
            box.Items.Add(SelectAll);

            foreach (string i in items)
            {
                CheckBox cb = new CheckBox();
                cb.Content = i;
                cb.Click += Filter_Click;
                cb.IsChecked = true;
                box.Items.Add(cb);
            }
        }

        private void filterGuns()
        {
            // Clear combo box
            cmbGuns.Items.Clear();
            List<Gun> eraFilteredGuns = new List<Gun>();
            List<Gun> categoryFiltered = new List<Gun>();
            List<Gun> nationFiltered = new List<Gun>();
            List<Gun> caliberFiltered = new List<Gun>();
            List<Gun> firearmActionFiltered = new List<Gun>();
            List<Gun> filteredGuns = new List<Gun>();

            // Filter on Era first
            foreach (object o in cmbEraFilter.Items)
            {
                CheckBox cb = (CheckBox)o;
                if (cb.IsChecked == true)
                {
                    string selectedEra = cb.Content.ToString();
                    List<Gun> filter = allGameData.guns.Where(g => g.Era == selectedEra).ToList();
                    eraFilteredGuns = eraFilteredGuns.Concat(filter).ToList();
                }
            }

            // remove duplicates
            eraFilteredGuns = eraFilteredGuns.Distinct().ToList();

            // Filter on Category
            foreach (object o in cmbCategoryFilter.Items)
            {
                CheckBox cb = (CheckBox)o;

                if (cb.IsChecked == true)
                {
                    string selectedCategory = cb.Content.ToString();

                    List<Gun> filter = eraFilteredGuns.Where(g => g.Categories.Contains(selectedCategory)).ToList();
                    categoryFiltered = categoryFiltered.Concat(filter).ToList();
                }


            }

            // Remove duplicates
            categoryFiltered = categoryFiltered.Distinct().ToList();

            // Filter on Nation
            foreach (object o in cmbNationFilter.Items)
            {
                CheckBox cb = (CheckBox)o;

                if (cb.IsChecked == true)
                {
                    string selectedNation = cb.Content.ToString();

                    List<Gun> filter = categoryFiltered.Where(g => g.NationOfOrigin == selectedNation).ToList();
                    nationFiltered = nationFiltered.Concat(filter).ToList();
                }
            }

            // Remove duplicates
            nationFiltered = nationFiltered.Distinct().ToList();

            // Filter on Caliber
            foreach (object o in cmbCaliberFilter.Items)
            {
                CheckBox cb = (CheckBox)o;

                if (cb.IsChecked == true)
                {
                    string selectedCaliber = cb.Content.ToString();
                    List<Gun> filter = nationFiltered.Where(g => g.Caliber == selectedCaliber).ToList();
                    caliberFiltered = caliberFiltered.Concat(filter).ToList();
                }
            }

            // remove duplicates
            caliberFiltered = caliberFiltered.Distinct().ToList();

            // Filter on firearm action
            foreach (object o in cmbFirearmActionFilter.Items)
            {
                CheckBox cb = (CheckBox)o;

                if (cb.IsChecked == true)
                {
                    string selectedFirearmAction = cb.Content.ToString();
                    List<Gun> filter = caliberFiltered.Where(g => g.FirearmAction == selectedFirearmAction).ToList();
                    firearmActionFiltered = firearmActionFiltered.Concat(filter).ToList();
                }
            }

            // remove duplicates
            firearmActionFiltered = firearmActionFiltered.Distinct().ToList();

            filteredGuns = firearmActionFiltered;
            filteredGuns = filteredGuns.OrderBy(g => g.GunName).ToList();

            // Update advancedGun drop down
            foreach (Gun g in filteredGuns)
            {
                cmbGuns.Items.Add(g.GunName);
            }


        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string cbContent = cb.Content.ToString();
            if (cbContent == "Select All")
            {
                ItemCollection items = null;
                ComboBox comboBox = (ComboBox)cb.Parent;

                if (comboBox.Name == "cmbEraFilter")
                {
                    items = cmbEraFilter.Items;
                }
                else if (comboBox.Name == "cmbCategoryFilter")
                {
                    items = cmbCategoryFilter.Items;
                }
                else if (comboBox.Name == "cmbNationFilter")
                {
                    items = cmbNationFilter.Items;
                }
                else if (comboBox.Name == "cmbCaliberFilter")
                {
                    items = cmbCaliberFilter.Items;
                }
                else if (comboBox.Name == "cmbFirearmActionFilter")
                {
                    items = cmbFirearmActionFilter.Items;
                }
                else
                {
                    // Do nothing
                }

                foreach (object o in items)
                {
                    CheckBox checkBox = (CheckBox)o;
                    checkBox.IsChecked = cb.IsChecked;
                }

            }
            filterGuns();
        }

          private void Ammo_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string cbContent = cb.Content.ToString();
            if (cbContent == "Select All")
            {                
                foreach (object o in cmbAmmo.Items)
                {
                    CheckBox checkBox = (CheckBox)o;
                    checkBox.IsChecked = cb.IsChecked;
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("You must enter a map name");
                return;
            }
            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("You must enter a map description");
                return;
            }
            if (selectedEnemies.Count == 0)
            {
                MessageBox.Show("You must select at least 1 enemy");
                return;
            }
            if (selectedGuns.Count == 0)
            {
                MessageBox.Show("You must select at least 1 advancedGun");
                return;
            }

            if (cmbEnemyProgressionType.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an enemy progression type");
                return;
            }

            if (cmbOrderType.SelectedIndex == -1)
            {
                MessageBox.Show("You must select a advancedGun order type");
                return;
            }

            if (allGameData.fileLocations.Count == 0)
            {
                MessageBox.Show("No save locations stored, update config.json and refresh game data");
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        allGameData.fileLocations.Add(dialog.SelectedPath);
                    }
                }
            }

            foreach (GunGameProgressionMakerAdvanced.Enemy enemy in selectedEnemies)
            {
                advancedOutput.Enemies.Add(enemy);
            }
           
            foreach(GunGameProgressionMakerAdvanced.Gun gun in selectedGuns)
            {
                advancedOutput.Guns.Add(gun);
            }
            advancedOutput.EnemyProgressionType = cmbEnemyProgressionType.SelectedIndex;
            advancedOutput.OrderType = cmbOrderType.SelectedIndex;
            advancedOutput.Name = txtName.Text;
            advancedOutput.Description = txtDescription.Text;
            string jsonString = JsonConvert.SerializeObject(advancedOutput, Formatting.Indented);
            string filename = "AdvancedGunGameWeaponPool_" + txtName.Text + ".json";
            foreach (string fp in allGameData.fileLocations)
            {
                string fullPath = fp + "\\" + filename;
                File.WriteAllText(fullPath, jsonString);
            }

            MessageBox.Show("Save complete");
        }

        private void cmbEnemyCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEnemyCategory.SelectedIndex != -1)
            {
                cmbEnemyName.Items.Clear();
                List<Enemy> filteredEnemies = allGameData.enemies.Where(en => en.category == cmbEnemyCategory.SelectedItem.ToString()).ToList();

                foreach (Enemy fe in filteredEnemies)
                {
                    cmbEnemyName.Items.Add(fe.name);
                }
            }
        }



        private void cmbEnemyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEnemyName.SelectedIndex != -1)
            {
                Enemy selectedEnemy = allGameData.enemies.Where(se => se.name == cmbEnemyName.SelectedItem.ToString()).FirstOrDefault();

                if (selectedEnemy != null)
                {
                    txtEnemyAppearance.Text = selectedEnemy.appearance;
                    txtEnemyWeapon.Text = selectedEnemy.weapons;
                    txtEnemyNote.Text = selectedEnemy.note;
                }
            }
        }

        private void btnAddEnemy_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEnemyName.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an enemy name");
                e.Handled = true;
                return;
            }
            GunGameProgressionMakerAdvanced.Enemy enemey = new GunGameProgressionMakerAdvanced.Enemy();
            enemey.EnemyNameString = cmbEnemyName.SelectedItem.ToString();
            enemey.Value = Convert.ToInt32(txtEnemyValue.Text);
            selectedEnemies.Add(enemey);
        }

        private void cmbGuns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGuns.SelectedIndex != -1)
            {
                string selectedGunName = cmbGuns.SelectedItem.ToString();

                // find default magazine
                Gun selectedGun = allGameData.guns.Where(g => g.GunName == selectedGunName).FirstOrDefault();

                if (selectedGun != null)
                {
                    cmbAmmo.Items.Clear();

                    // add select all
                    CheckBox SelectAll = new CheckBox();
                    SelectAll.Content = "Select All";
                    SelectAll.IsChecked = true;
                    SelectAll.Click += Ammo_Click;
                    cmbAmmo.Items.Add(SelectAll);

                    

                    foreach (string m in selectedGun.CompatableAmmo)
                    {
                        CheckBox cb = new CheckBox();
                        cb.Content = m;
                        cb.IsChecked = true;
                        cmbAmmo.Items.Add(cb);
                    }

                    List<string> extraCategories = new List<string>();
                    foreach(Extras ex in selectedGun.CompatibleExtras)
                    {
                        if(!extraCategories.Contains(ex.SubCategory))
                        {
                            extraCategories.Add(ex.SubCategory);
                        }
                    }
                    extraCategories.Sort();
                    cmbExtraCategory.Items.Clear();
                    cmbExtra.Items.Clear();
                    foreach(string m in extraCategories)
                    {
                        cmbExtraCategory.Items.Add(m);
                    }

                }

            }
        }

        private void btnAddGun_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGuns.SelectedIndex == -1)
            {
                MessageBox.Show("You must pick a advancedGun");
                return;
            }
            GunGameProgressionMakerAdvanced.Gun gun = new GunGameProgressionMakerAdvanced.Gun();
            gun.GunName = cmbGuns.SelectedItem.ToString();
            foreach (object o in cmbAmmo.Items)
            {
                CheckBox checkBox = (CheckBox)o;
                if (checkBox.IsChecked == true)
                {
                    string name = checkBox.Content.ToString();
                    if (name != "Select All")
                    {
                        gun.MagNames.Add(name);
                    }
                    
                }
            }
            if (gun.MagNames.Count == 0)
            {
                MessageBox.Show("You must pick at least one ammo type");
                return;
            }

            if (cmbExtra.SelectedIndex != -1)
            {
                gun.Extra = cmbExtra.SelectedItem.ToString();
            }

            selectedGuns.Add(gun);
        }

        private void btnAddAllGuns_Click(object sender, RoutedEventArgs e)
        {
            foreach(object o in cmbGuns.Items)
            {
                GunGameProgressionMakerAdvanced.Gun advancedGun = new GunGameProgressionMakerAdvanced.Gun();
                advancedGun.GunName = o.ToString();

                Gun selectedGun = allGameData.guns.Where(g => g.GunName == advancedGun.GunName).FirstOrDefault();
                if (selectedGun != null)
                {
                    foreach(string a in selectedGun.CompatableAmmo)
                    {
                        advancedGun.MagNames.Add(a);
                    }
                    selectedGuns.Add(advancedGun);
                }
            }
        }

        private void cmbExtraCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get selected gun
            string selectedGunName = cmbGuns.SelectedItem.ToString();

            // find default magazine
            Gun selectedGun = allGameData.guns.Where(g => g.GunName == selectedGunName).FirstOrDefault();

            if (cmbExtraCategory.SelectedItem != null)
            {
                List<Extras> compatibleExtras = selectedGun.CompatibleExtras.Where(g => g.SubCategory == cmbExtraCategory.SelectedItem.ToString()).ToList();

                cmbExtra.Items.Clear();
                cmbExtra.Items.Add("None");
                foreach (Extras ex in compatibleExtras)
                {
                    cmbExtra.Items.Add(ex.ExtraName);
                }
            }
           
            
        }
    }
}
