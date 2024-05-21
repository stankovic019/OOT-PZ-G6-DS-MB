using Microsoft.Win32;
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

namespace OOT_PZ_Kursevi
{
    /// <summary>
    /// Interaction logic for Dodaj.xaml
    /// </summary>
    public partial class Dodaj : Window
    {

        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();
        private Citac citac = new Citac();
        
        private Dictionary<int, Kategorija> kategorije = new Dictionary<int, Kategorija>();


        public Dodaj()
        {
            InitializeComponent();
            kursevi = citac.ucitajKurseve();
            kategorije = citac.ucitajKategorije();

            foreach(Kategorija k in kategorije.Values)
                KategorijeCB.Items.Add(k.Naziv);

            

        }

        private void browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp";



            if (ofd.ShowDialog() == true)
            {
                string[] imeSlike = ofd.FileName.Split('\\');
                int count = imeSlike.Count();
                string noviPathSlike = "\\photos\\" + imeSlike[count - 1];
                Ikonicatb.Text = noviPathSlike;
            }
            

        }

        private bool isNumber(string str)
        {
            foreach(char c in str)
                if(c < '0' || c > '9')
                    return false;
            
            return true;
        }

        private bool isDouble(string str)
        {

            foreach (char c in str)
                if (c < '0' || c > '9')
                    if (c == '.')
                        continue;
                    else return false;

            return true;
        }

        private void dodaj(object sender, RoutedEventArgs e)
        {
            if(IDtb.Text == "" || Nazivtb.Text == "" || Opistb.Text == "" || Cenatb.Text == ""
                || KategorijeCB.SelectedIndex == -1 && (DostupanRB.IsChecked == false || NedostupanRB.IsChecked == false))
            { 
                MessageBox.Show("Sva masna polja moraju biti popunjena!", "Greška: nedovoljno informacija", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isNumber(IDtb.Text)){
                MessageBox.Show("Jedinstveni ID mora da bude broj!", "Greška: neispravan unos", MessageBoxButton.OK, MessageBoxImage.Error);
                IDtb.Text = "";
                return;
            }

            if (kursevi.ContainsKey(Convert.ToInt32(IDtb.Text)))
            {
                MessageBox.Show("Kurs sa jedinstvenim ID \"" + IDtb.Text + "\" već postoji u kolekciji.\n" +
                                "Unesite drugi ili izaberite automatski.", "Greška: ID već postoji", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                IDtb.Text = "";
                return;
            }

            if (!isDouble(Cenatb.Text))
            {
                MessageBox.Show("Cena kursa mora da bude broj!\n" +
                                "(Primer: 300, 420.69, ...)", "Greška: neispravan unos", MessageBoxButton.OK, MessageBoxImage.Error);
                Cenatb.Text = "";
                return;
            }



            Kurs newK = new Kurs(Convert.ToInt32(IDtb.Text), Nazivtb.Text, Opistb.Text, Convert.ToDouble(Cenatb.Text),
                                Ikonicatb.Text, KategorijeCB.SelectedItem.ToString(), 
                                (DostupanRB.IsChecked == true ? true : false));

            kursevi.Add(newK.getId(), newK);

            MessageBox.Show("Uspešno je dodat novi kurs.", "USPEŠNO", MessageBoxButton.OK, MessageBoxImage.Information);

            Pisac p = new Pisac(kursevi);

            p.upisi();

            this.DialogResult = true;

            this.Close();

        }

        private void autoID(object sender, RoutedEventArgs e)
        {
            Random r = new Random();

            int br = r.Next();
           
            if(!kursevi.ContainsKey(br))  
                IDtb.Text = br.ToString();
             
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            

        }
    }
}
