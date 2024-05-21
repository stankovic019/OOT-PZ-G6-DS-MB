using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace OOT_PZ_Kursevi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();
        private ObservableCollection<Kurs> dostupniKursevi = new ObservableCollection<Kurs> ();
        private ObservableCollection<Kurs> nedostupniKursevi = new ObservableCollection<Kurs> ();
        private Citac citac = new Citac();

        public MainWindow()
        {
            InitializeComponent();

            inicijalizuj();
            
            //DRAG AND DROP
            izmeniKursB.IsEnabled = obrisiKursB.IsEnabled = false;

        }

        private void inicijalizuj()
        {
            kursevi = citac.ucitajKurseve();
            dostupniKursevi = citac.KursToObservableColection(true);
            dostupniKurseviDGV.ItemsSource = dostupniKursevi;

            nedostupniKursevi = citac.KursToObservableColection(false);
            nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;
        }


        private void dodajKurs(object sender, RoutedEventArgs e)
        {
            Dodaj novi = new Dodaj();
            if (novi.ShowDialog() == true)
                inicijalizuj();
        }

        private void izmeniKurs(object sender, RoutedEventArgs e)
        {

            int id = -1;

            if (dostupniKurseviDGV.SelectedIndex != -1)
            {
                id = dostupniKursevi[dostupniKurseviDGV.SelectedIndex].ID;
                dostupniKurseviDGV.SelectedIndex = -1;
            }
            else if (nedostupniKurseviDGV.SelectedIndex != -1)
            {
                id = nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID;
                nedostupniKurseviDGV.SelectedIndex = -1;
            }

            Izmeni izmena = new Izmeni(id);

            if (izmena.ShowDialog() == true)
                inicijalizuj();
            

        }

        private void obrisiKurs(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Da li zaista želite da obrišete izabrani kurs?\n" +
                                "NAPOMENA: Ova operacija se ne može vratiti. Obrisani kurs ćete morati da vratite ručno!",
                                "Obriši kurs?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                if (dostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(dostupniKursevi[dostupniKurseviDGV.SelectedIndex].ID);
                else if(nedostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID);
                
                Pisac newPisac = new Pisac(kursevi);

                newPisac.upisi();

                this.inicijalizuj();
                    
            }

        }

        private bool ignoreIndexChange = false;

        private void dostupniKurseviDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(ignoreIndexChange) return;

            ignoreIndexChange = true;

            try
            {
                nedostupniKurseviDGV.SelectedIndex = -1;

                if (dostupniKurseviDGV.SelectedIndex != -1)
                    izmeniKursB.IsEnabled = obrisiKursB.IsEnabled = true;
            }
            finally
            {
                ignoreIndexChange = false;
            }
        }

        private void nedostupniKurseviDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if(ignoreIndexChange) return;

            ignoreIndexChange = true;

            try
            {
                dostupniKurseviDGV.SelectedIndex = -1;

                if (nedostupniKurseviDGV.SelectedIndex != -1)
                    izmeniKursB.IsEnabled = obrisiKursB.IsEnabled = true;
            }

            finally
            {
                ignoreIndexChange = false;
            }
        }

        private void pretragaNazivRB_Checked(object sender, RoutedEventArgs e)
        {
            pretragaDostupnihTB.IsEnabled = true;
            pretragaDostupnihTB.Text = "";
        }

        private void pretragaKategorijaRB_Checked(object sender, RoutedEventArgs e)
        {
            pretragaDostupnihTB.IsEnabled = true;
            pretragaDostupnihTB.Text = "";
        }

        private void pretragaDostupnihTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObservableCollection<Kurs> odredjeniKursevi = new ObservableCollection<Kurs>();

            if(pretragaDostupnihTB.Text == "")
            {
                dostupniKurseviDGV.ItemsSource = dostupniKursevi;
                return;
            }

            foreach (Kurs k in dostupniKursevi)
            {
                string parametarPretrazivanja = "";
                if (pretragaNazivRB.IsChecked == true)
                    parametarPretrazivanja = k.Naziv.ToLower();
                else if(pretragaKategorijaRB.IsChecked == true)
                    parametarPretrazivanja = k.Kategorija.ToLower();

                if (parametarPretrazivanja.StartsWith(pretragaDostupnihTB.Text.ToLower()))
                    odredjeniKursevi.Add(k);
            }

            dostupniKurseviDGV.ItemsSource = odredjeniKursevi;

        }

        private void PopuniTreeView()
        {

            Dictionary<int, Kategorija> kategorije = citac.ucitajKategorije();
            foreach(var k in kategorije)
            {
                BitmapImage kategorijaSlika = new BitmapImage(new Uri(k.Value.SlikaPath, UriKind.RelativeOrAbsolute));
                TreeViewItem kategorijaNode = NapraviTreeViewItem(k.Value.Naziv, kategorijaSlika);
                
                Dictionary<int, Kurs> kursevi = citac.ucitajKurseve();
                foreach (var kurs in kursevi)
                {
                    if (k.Value.Naziv == kurs.Value.Kategorija)
                    {
                        BitmapImage kursSlika = new BitmapImage(new Uri(kurs.Value.SlikaPath, UriKind.RelativeOrAbsolute));
                        TreeViewItem kursNode = NapraviTreeViewItem(kurs.Value.Naziv, kursSlika);
                        kategorijaNode.Items.Add(kursNode);
                    }
                }
                MyTreeView.Items.Add(kategorijaNode);

            }
        }

        private TreeViewItem NapraviTreeViewItem(string text, BitmapImage imageSource)
        { 
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Image image = new Image();
            image.Source = imageSource;
            image.Width = 30; 
            image.Height = 50; 
            image.Margin = new Thickness(0, 0, 5, 0); 

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);

            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = stackPanel;

            return treeViewItem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopuniTreeView();
        }
    }
}