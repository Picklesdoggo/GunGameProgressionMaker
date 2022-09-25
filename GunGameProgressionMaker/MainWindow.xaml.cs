using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GunGameProgressionMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member variables

        ObservableCollection<Gun> selectedGuns = new ObservableCollection<Gun>();  
        GunJson allGuns = new GunJson();
              
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

            allGuns = JsonConvert.DeserializeObject<GunJson>(json);                       
        }

        #endregion

        #region Data updating methods

        private void populateDropDowns()
        {
            // Populate gun drop down
            foreach (Gun g in allGuns.guns)
            {
                cmbGuns.Items.Add(g.GunName);
            }
            
            // Populate enemy drop down
            foreach (string e in allGuns.enemies)
            {
                cmbEnemyType.Items.Add(e);
            }

            // Populate Era drop down with check boxes
            pouplateDropDown(allGuns.eras, cmbEraFilter);

            // Populate Category drop down with check boxes
            pouplateDropDown(allGuns.categories, cmbCategoryFilter);
           
            // Populate Nation drop down with check boxes
            pouplateDropDown(allGuns.nations, cmbNationFilter);
            
            // Populate Order drop down
            cmbOrderType.Items.Add("0 - Fixed - weapons will spawn in specific order.");
            cmbOrderType.Items.Add("1 - Random - weapons will spawn in random order.");
            cmbOrderType.Items.Add("2 - Random within category- weapons will be spawned randomly but will go through category.");
            cmbOrderType.SelectedIndex = 1;
            
            // Populate categoryID drop down
            for (int i = 0; i <= 100; i++)
            {
                cmbCategoryID.Items.Add(i);
            }

            cmbCategoryID.SelectedIndex = 0;
        }

        private void pouplateDropDown(List<string> items, ComboBox box)
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

            filteredGuns = nationFiltered;
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

            if (cmbEnemyType.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an enemy type");
                return;
            }

            if (allGuns.fileLocations.Count == 0)
            {
                MessageBox.Show("You must define a save location, update gameData.json");
                return;
            }

            ProgressionJSON json = new ProgressionJSON()
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    OrderType = cmbOrderType.SelectedIndex,
                    Guns = new object[0],
                    EnemyType = cmbEnemyType.SelectedItem.ToString(),
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
       
        #endregion

    }
}