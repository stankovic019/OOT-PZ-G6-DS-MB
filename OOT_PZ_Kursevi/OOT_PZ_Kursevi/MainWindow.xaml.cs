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
    
    public partial class MainWindow : Window
    {
        #region DIMITRIJE tab1 polja
        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();
        private ObservableCollection<Kurs> dostupniKursevi = new ObservableCollection<Kurs> ();
        private ObservableCollection<Kurs> nedostupniKursevi = new ObservableCollection<Kurs> ();
        private ObservableCollection<Kurs> odredjeniKursevi = new ObservableCollection<Kurs>();
        private bool searchOn = false;
        private bool ignoreIndexChange = false;
        private Point _startPoint;
        private Citac citac = new Citac();

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            inicijalizujTab1();
            
        }

        #region DIMITRIJE tab1 METODE
        //funkcija za inicijalizaciju izgleda taba1
        //potrebno je ucitati promene
        private void inicijalizujTab1()
        {
            kursevi = citac.ucitajKurseve();
            dostupniKursevi = citac.KursToObservableColection(true);
            dostupniKurseviDGV.ItemsSource = dostupniKursevi;

            if (searchOn) //ako je izmenjeno tokom pretrage treba ga vratiti na pretragu
            {   
                //buduci da mi ne dozvoljava da samo pozovem "text changed" event
                //morao sam zaobilaznim putem da ga "triggerujem"
                string str = pretragaDostupnihTB.Text;
                pretragaDostupnihTB.Text = "";
                pretragaDostupnihTB.Text = str;
            }

            nedostupniKursevi = citac.KursToObservableColection(false);
            nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;
            izmeniKursB.IsEnabled = obrisiKursB.IsEnabled = false;
        }

        //cuvanje trenutnog stanja aplikacije pre otvaranja novih prozora i formi
        //kako bi i ti novi prozori bili usaglašeni sa glavnim prozorom
        private void sacuvajTrenutnoStanje()
        {
            Pisac p = new Pisac(kursevi);
            p.upisi();
        }

        private void dodajKurs(object sender, RoutedEventArgs e)
        {
            sacuvajTrenutnoStanje();
            Dodaj novi = new Dodaj();
            if (novi.ShowDialog() == true)
                inicijalizujTab1();
        }

        private void izmeniKurs(object sender, RoutedEventArgs e)
        {

            int id = -1;

            if (dostupniKurseviDGV.SelectedIndex != -1)
            {
                if (!searchOn)
                {
                    id = dostupniKursevi[dostupniKurseviDGV.SelectedIndex].ID;
                    dostupniKurseviDGV.SelectedIndex = -1;
                }
                else
                {
                    id = odredjeniKursevi[dostupniKurseviDGV.SelectedIndex].ID;
                    dostupniKurseviDGV.SelectedIndex = -1;
                }
            }
            else if (nedostupniKurseviDGV.SelectedIndex != -1)
            {
                id = nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID;
                nedostupniKurseviDGV.SelectedIndex = -1;
            }

            try
            {
                sacuvajTrenutnoStanje();
                Izmeni izmena = new Izmeni(id);

                if (izmena.ShowDialog() == true)
                    inicijalizujTab1();
            }
            catch(Exception) {
                
                MessageBox.Show("Izaberite kurs koji želite da izmenite!", "Greška: nije selektovan kurs", MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

        }

        private void obrisiKurs(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Da li zaista želite da obrišete izabrani kurs?\n" +
                                "NAPOMENA: Ova operacija se ne može opozvati. Obrisani kurs ćete morati da vratite ručno!",
                                "Obriši kurs?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                if (dostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(dostupniKursevi[dostupniKurseviDGV.SelectedIndex].ID);
                else if(nedostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID);

                sacuvajTrenutnoStanje();

                this.inicijalizujTab1();
                    
            }

        }

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
            searchOn = true;
            odredjeniKursevi.Clear();

            if(pretragaDostupnihTB.Text == "")
            {
                dostupniKurseviDGV.ItemsSource = dostupniKursevi;
                searchOn = false;
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

        //DRAG AND DROP FEATURE
        private static T FindAnchestor<T>(DependencyObject current)
        where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        //DOSTUPNI KURSEVI
        private void dropToDostupni(int nedostupanSelectedIndex)
        {

            nedostupniKursevi[nedostupanSelectedIndex].Dostupan = true;
            kursevi[nedostupniKursevi[nedostupanSelectedIndex].ID].Dostupan = true;
            dostupniKursevi.Add(nedostupniKursevi[nedostupanSelectedIndex]);
            nedostupniKursevi.RemoveAt(nedostupanSelectedIndex);

        }
        private void dostupniKurseviDGV_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void dostupniKurseviDGV_PreviewMouseMove(object sender, MouseEventArgs e)
        {   
            var dg = sender as DataGrid;
            if (dg == null) return;

           
            Point mousePos = e.GetPosition(null);
            double[] razlika = new double[2];
            razlika[0] = _startPoint.X - mousePos.X;
            razlika[1] = _startPoint.Y - mousePos.Y;
          
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(razlika[0]) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(razlika[1]) > SystemParameters.MinimumVerticalDragDistance))
            {
                var DataGridRow =
                    FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);

                if (DataGridRow == null)
                    return;
                
                var dataTodrop = (Kurs)dg.ItemContainerGenerator.
                    ItemFromContainer(DataGridRow);

                if (dataTodrop == null) return;
 
                var dataObj = new DataObject(dataTodrop);
                dataObj.SetData("DragSource", sender);
                DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Copy);
               
            }
        }
       
        private void dostupniKurseviDGV_Drop(object sender, DragEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var dgSrc = e.Data.GetData("DragSource") as DataGrid;
            var data = e.Data.GetData(typeof(Kurs));
            if (dgSrc == null || data == null) return;
            try
            {
                dropToDostupni(nedostupniKurseviDGV.SelectedIndex);
            }
            catch (Exception) { }

        }

        //NEDOSTUPNI KURSEVI
        private void dropToNedostupni(int dostupanSelectedIndex) {

            dostupniKursevi[dostupanSelectedIndex].Dostupan = false;
            kursevi[dostupniKursevi[dostupanSelectedIndex].ID].Dostupan = false;
            nedostupniKursevi.Add(dostupniKursevi[dostupanSelectedIndex]);
            dostupniKursevi.RemoveAt(dostupanSelectedIndex);
            
        }
        private void nedostupniKurseviDGV_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
        private void nedostupniKurseviDGV_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;


            Point mousePos = e.GetPosition(null);
            double[] razlika = new double[2];
            razlika[0] = _startPoint.X - mousePos.X;
            razlika[1] = _startPoint.Y - mousePos.Y;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(razlika[0]) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(razlika[1]) > SystemParameters.MinimumVerticalDragDistance))
            {
                var DataGridRow =
                    FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);

                if (DataGridRow == null)
                    return;

                var dataTodrop = (Kurs)dg.ItemContainerGenerator.
                    ItemFromContainer(DataGridRow);

                if (dataTodrop == null) return;

                var dataObj = new DataObject(dataTodrop);
                dataObj.SetData("DragSource", sender);
                DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Copy);

            }
        }


        private void nedostupniKurseviDGV_Drop(object sender, DragEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var dgSrc = e.Data.GetData("DragSource") as DataGrid;
            var data = e.Data.GetData(typeof(Kurs));
            if (dgSrc == null || data == null) return;
            try
            {
                dropToNedostupni(dostupniKurseviDGV.SelectedIndex);
            }catch (Exception) { }
        }

        #endregion
        private void Window_Closed(object sender, EventArgs e)
        {
            //potrebno je reci operativnom sistemu da ako se zatvori glavni prozor ugasi aplikaciju
            //jer u protivnom moze da ostane kao jedan od background procesa - a to ne zelimo
            sacuvajTrenutnoStanje();
            this.Close();
            Application.Current.Shutdown();
            
        }

    }
}