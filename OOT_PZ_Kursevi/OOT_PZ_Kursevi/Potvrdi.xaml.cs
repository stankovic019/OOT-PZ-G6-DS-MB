﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OOT_PZ_Kursevi
{
    /// <summary>
    /// Interaction logic for Potvrdi.xaml
    /// </summary>
    public partial class Potvrdi : Window
    {
        
        public Potvrdi(ObservableCollection<Kurs> selektovaniKursevi)
        {
            InitializeComponent();
            Lista_Potvrdi.ItemsSource = selektovaniKursevi;
            
        }

        private void Zatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();        }
    }
}
