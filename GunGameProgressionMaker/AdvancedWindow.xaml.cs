using GunGameProgressionMakerAdvanced;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace GunGameProgressionMaker
{
    /// <summary>
    /// Interaction logic for AdvancedWindow.xaml
    /// </summary>
    public partial class AdvancedWindow : Window
    {
        InputJson allGameData = new InputJson();
        public AdvancedWindow()
        {
            InitializeComponent();

            getGameData();

            populateDropDowns();
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

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {


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
    }
}
