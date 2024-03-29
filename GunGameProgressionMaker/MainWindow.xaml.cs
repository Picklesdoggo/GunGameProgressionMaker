﻿using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSimple_Click(object sender, RoutedEventArgs e)
        {
            BasicWindow bw = new BasicWindow();
            bw.Show();
            Close();
        }

        private void btnAdvanced_Click(object sender, RoutedEventArgs e)
        {
            AdvancedWindow aw = new AdvancedWindow();
            aw.Show();
            Close();
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            JsonBuilder.generateJson();
            MessageBox.Show("Game data refreshed");
           
        }

       
    }
}
