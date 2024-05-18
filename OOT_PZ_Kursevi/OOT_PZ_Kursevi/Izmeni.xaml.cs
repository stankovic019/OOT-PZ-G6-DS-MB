using Microsoft.Win32;
using System;
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

namespace OOT_PZ_Kursevi
{
    
    public partial class Izmeni : Window
    {
        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();
        private Citac citac = new Citac();

        private Dictionary<int, Kategorija> kategorije = new Dictionary<int, Kategorija>();

        private int id;

        public Izmeni(int id)
        {
            InitializeComponent();
            this.id = id;
            ucitaj();
            
        }

        private void ucitaj()
        {
            kursevi = citac.ucitajKurseve();
            kategorije = citac.ucitajKategorije();

            foreach (Kategorija kat in kategorije.Values)
                KategorijeCB.Items.Add(kat.Naziv);

            Kurs k = kursevi[id];

            IDtb.Text = k.getId().ToString();
            Nazivtb.Text = k.Naziv;
            Opistb.Text = k.Opis;
            Cenatb.Text = k.Cena.ToString();
            Ikonicatb.Text = k.SlikaPath;
            KategorijeCB.Text = k.Kategorija;
            


            DostupanRB.IsChecked = !(NedostupanRB.IsChecked = !k.Dostupan);

            
        }



        private void browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp";



            if (ofd.ShowDialog() == true)
                Ikonicatb.Text = System.IO.Path.GetFullPath(ofd.FileName);
            
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

     

        private void sacuvaj(object sender, RoutedEventArgs e)
        {
            if (IDtb.Text == "" || Nazivtb.Text == "" || Opistb.Text == "" || Cenatb.Text == ""
               || KategorijeCB.SelectedIndex == -1 && (DostupanRB.IsChecked == false || NedostupanRB.IsChecked == false))
            {
                MessageBox.Show("Sva masna polja moraju biti popunjena!", "Greška: nedovoljno informacija", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isDouble(Cenatb.Text))
            {
                MessageBox.Show("Cena kursa mora da bude broj!\n" +
                                "(Primer: 300, 420.69, ...)", "Greška: neispravan unos", MessageBoxButton.OK, MessageBoxImage.Error);
                Cenatb.Text = "";
                return;
            }

            kursevi[id].Naziv = Nazivtb.Text;
            kursevi[id].Opis = Opistb.Text;
            kursevi[id].Cena = Convert.ToDouble(Cenatb.Text);
            kursevi[id].SlikaPath = Ikonicatb.Text;
            kursevi[id].Kategorija = KategorijeCB.SelectedItem.ToString();
            kursevi[id].Dostupan = (DostupanRB.IsChecked == true ? true : false);

            Pisac p = new Pisac(kursevi);

            p.upisi();


            MessageBox.Show("Promene su uspešno sačuvane.", "", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();

        }

    }
}
