using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GunGameProgressionMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member variables
        
        ObservableCollection<Gun> selectedGuns = new ObservableCollection<Gun>();  
        InputJson allGuns = new InputJson();
       
        #endregion

        public MainWindow()
        {
            InitializeComponent();       

            // Load game data
            getGameData();                

            // Pouplate drop downs
            populateDropDowns();

            // Bind data source
            grdGuns.ItemsSource = selectedGuns;

        }

        #region Data loading methods

        private void getGameData()
        {
            // load JSON file
            string json = File.ReadAllText("gameData.json");       

            allGuns = JsonConvert.DeserializeObject<InputJson>(json);
        }

        #endregion

        #region Data updating methods

        private void populateDropDowns()
        {
            cmbGuns.Items.Clear();
            // Populate gun drop down
            foreach (Gun g in allGuns.guns)
            {
                cmbGuns.Items.Add(g.GunName);
            }

            cmbEnemyName.Items.Clear();
            cmbEnemyCategory.Items.Clear();
            // Populate enemy drop downs
            foreach (string e in allGuns.enemyCategories)
            {
                cmbEnemyCategory.Items.Add(e);
            }
           

            // Populate Era drop down with check boxes
            cmbEraFilter.Items.Clear();
            populateDrowDown(allGuns.eras, cmbEraFilter);

            // Populate Category drop down with check boxes
            cmbCategoryFilter.Items.Clear();
            populateDrowDown(allGuns.categories, cmbCategoryFilter);

            // Populate Nation drop down with check boxes
            cmbNationFilter.Items.Clear();
            populateDrowDown(allGuns.nations, cmbNationFilter);

            // Populate Caliber drop down with check boxes
            cmbCaliberFilter.Items.Clear();
            populateDrowDown(allGuns.calibers, cmbCaliberFilter);

            // Populate Firearm action drop down with check boxes
            cmbFirearmActionFilter.Items.Clear();
            populateDrowDown(allGuns.firearmactions, cmbFirearmActionFilter);

            // Populate Order drop down
            cmbOrderType.Items.Clear();
            cmbOrderType.Items.Add("0 - Fixed - weapons will spawn in specific order.");
            cmbOrderType.Items.Add("1 - Random - weapons will spawn in random order.");
            cmbOrderType.Items.Add("2 - Random within category- weapons will be spawned randomly but will go through category.");
            cmbOrderType.SelectedIndex = 1;

            // Populate categoryID drop down
            cmbCategoryID.Items.Clear();
            for (int i = 0; i <= allGuns.maxCategories; i++)
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
            cmbMagazines.Items.Clear();
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
                    List<Gun> filter = allGuns.guns.Where(g => g.Era == selectedEra).ToList();
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

            // Update gun drop down
            foreach (Gun g in filteredGuns)
            {
                cmbGuns.Items.Add(g.GunName);
            }


        }

        #endregion

        #region Event Handlers

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

        private void cmbGuns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGuns.SelectedIndex != -1)
            {
                string selectedGunName = cmbGuns.SelectedItem.ToString();

                // find default magazine
                Gun selectedGun = allGuns.guns.Where(g => g.GunName == selectedGunName).FirstOrDefault();

                if (selectedGun != null)
                {
                    cmbMagazines.Items.Clear();

                    foreach (string m in selectedGun.CompatableAmmo)
                    {
                        cmbMagazines.Items.Add(m);
                    }

                    int magazineIndex = selectedGun.CompatableAmmo.IndexOf(selectedGun.DefaultMagName);

                    cmbMagazines.SelectedIndex = magazineIndex;
                }

            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            if (cmbGuns.SelectedIndex == -1)
            {
                MessageBox.Show("You must select a gun");
                return;
            }

            if (cmbMagazines.SelectedIndex == -1)
            {
                MessageBox.Show("You must select a magazine");
                return;
            }

            Gun gun = new Gun();
            gun.CategoryID = cmbCategoryID.SelectedIndex;
            gun.GunName = cmbGuns.SelectedItem.ToString();
            gun.SelctedMagName = cmbMagazines.SelectedItem.ToString();
            // find default mag
            Gun selectedGun = allGuns.guns.Where(g => g.GunName == gun.GunName).FirstOrDefault();

            if (selectedGun != null)
            {
                gun.DefaultMagName = selectedGun.DefaultMagName;
            }


            selectedGuns.Add(gun);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("You must enter a name");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("You must enter a description");
                return;
            }

            if (cmbEnemyName.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an enemy type");
                return;
            }

            if (allGuns.fileLocations.Count == 0)
            {
                MessageBox.Show("No save locations stored, update gameData.json");
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        allGuns.fileLocations.Add(dialog.SelectedPath);
                    }                                      
                }
            }

            OutputJson json = new OutputJson()
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    OrderType = cmbOrderType.SelectedIndex,
                    Guns = new object[0],
                    EnemyType = cmbEnemyName.SelectedItem.ToString(),
                    CategoryIDs = new List<int>(),
                    GunNames = new List<string>(),
                    MagNames = new List<string>()
                };

            foreach (Gun g in selectedGuns)
            {
                json.CategoryIDs.Add(g.CategoryID);
                json.GunNames.Add(g.GunName);
                json.MagNames.Add(g.SelctedMagName);
            }

            string jsonString = JsonConvert.SerializeObject(json);
            string filename = "GunGameWeaponPool_" + txtName.Text + ".json";


            foreach(string fp in allGuns.fileLocations)
            {
                string fullPath = fp + "\\" + filename;
                File.WriteAllText(fullPath, jsonString);
            }

            MessageBox.Show("Save complete");
        }
        
        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            // add all guns in the gun drop down
            foreach (string g in cmbGuns.Items)
            {
                Gun gun = new Gun();
                gun.CategoryID = cmbCategoryID.SelectedIndex;
                gun.GunName = g;
                
                // find default mag
                Gun selectedGun = allGuns.guns.Where(gs => gs.GunName == gun.GunName).FirstOrDefault();

                if (selectedGun != null)
                {
                    gun.DefaultMagName = selectedGun.DefaultMagName;
                    gun.SelctedMagName = selectedGun.DefaultMagName;
                    selectedGuns.Add(gun);
                }

            }
        }        

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            selectedGuns.Clear();
        }
        
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (allGuns.fileLocations.Count != 0)
            {
                openFileDialog.InitialDirectory = allGuns.fileLocations[0];
            }
            

            openFileDialog.Filter = "Json files (GunGame*.json)|GunGame*.json";

            if (openFileDialog.ShowDialog() == true)
            {

                string json = File.ReadAllText(openFileDialog.FileName);

                OutputJson loadedJson = JsonConvert.DeserializeObject<OutputJson>(json);

                txtDescription.Text = loadedJson.Description;
                txtName.Text = loadedJson.Name;

                cmbOrderType.SelectedIndex = loadedJson.OrderType;

                // get the category index
                Enemy selectedEnemy = allGuns.enemies.Where(se => se.name == loadedJson.EnemyType).FirstOrDefault();
                int categoryIndex = cmbEnemyCategory.Items.IndexOf(selectedEnemy.category);
                cmbEnemyCategory.SelectedIndex = categoryIndex;

                // get the enemy index
                int enemyIndex = cmbEnemyName.Items.IndexOf(loadedJson.EnemyType);
                cmbEnemyName.SelectedIndex = enemyIndex;

                // clear selected guns
                selectedGuns.Clear();

                // load the guns
                for (int i = 0; i < loadedJson.GunNames.Count; i++)
                {
                    Gun gun = new Gun();
                    gun.GunName = loadedJson.GunNames[i];
                    gun.SelctedMagName = loadedJson.MagNames[i];
                    gun.CategoryID = loadedJson.CategoryIDs[i];

                    // get the default mag
                    gun.DefaultMagName = allGuns.guns.Where(g => g.GunName == gun.GunName).FirstOrDefault().DefaultMagName;

                    selectedGuns.Add(gun);
                }

            }
        }

        

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            JsonBuilder.generateJson();

            // Load game data
            getGameData();

            // Pouplate drop downs
            populateDropDowns();

            // Bind data source
            grdGuns.ItemsSource = selectedGuns;
        }

        

        private void cmbEnemyCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEnemyCategory.SelectedIndex != -1)
            {
                cmbEnemyName.Items.Clear();
                List<Enemy> filteredEnemies = allGuns.enemies.Where(en => en.category == cmbEnemyCategory.SelectedItem.ToString()).ToList();

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
                Enemy selectedEnemy = allGuns.enemies.Where(se => se.name == cmbEnemyName.SelectedItem.ToString()).FirstOrDefault();

                if (selectedEnemy != null)
                {
                    txtEnemyAppearance.Text = selectedEnemy.appearance;
                    txtEnemyWeapon.Text = selectedEnemy.weapons;
                    txtEnemyNote.Text = selectedEnemy.note;
                }
            }
        }

        #endregion
    }
}