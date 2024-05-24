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
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;

namespace OOT_PZ_Kursevi
{

    public partial class MainWindow : Window
    {


        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();
        private ObservableCollection<Kurs> dostupniKursevi = new ObservableCollection<Kurs>();
        private ObservableCollection<Kurs> nedostupniKursevi = new ObservableCollection<Kurs>();
        private ObservableCollection<Kurs> odredjeniKurseviD = new ObservableCollection<Kurs>();
        private ObservableCollection<Kurs> odredjeniKurseviN = new ObservableCollection<Kurs>();
        private ObservableCollection<Kurs> Korp { get; set; } = new ObservableCollection<Kurs>();
        private bool searchOn = false;
        private bool ignoreIndexChange = false;
        private Point _startPoint;
        private Citac citac = new Citac();

        public MainWindow()
        {
            InitializeComponent();

            inicijalizujTab1();
            inicijalizujTab3();

            //MAJA INICIJALIZACIJA
            PopuniTreeView();

        }
        //funkcija za inicijalizaciju izgleda taba1
        //potrebno je ucitati promene
        private void inicijalizujTab1()
        {
            kursevi = citac.ucitajKurseve();
            dostupniKursevi = citac.KursToObservableColection(true);
            dostupniKurseviDGV.ItemsSource = dostupniKursevi;

            nedostupniKursevi = citac.KursToObservableColection(false);
            nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;

            if (searchOn) //ako je izmenjeno tokom pretrage treba ga vratiti na pretragu
            {
                //buduci da mi ne dozvoljava da samo pozovem "text changed" event
                //morao sam zaobilaznim putem da ga "triggerujem"
                string str = pretragaDostupnihTB.Text;
                pretragaDostupnihTB.Text = "";
                pretragaDostupnihTB.Text = str;
            }

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
            {
                this.inicijalizujTab1();
                this.PopuniTreeView();
                this.inicijalizujTab3();
            }
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
                    id = odredjeniKurseviD[dostupniKurseviDGV.SelectedIndex].ID;
                    dostupniKurseviDGV.SelectedIndex = -1;
                }
            }
            else if (nedostupniKurseviDGV.SelectedIndex != -1)
            {
                if (!searchOn)
                {
                    id = nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID;
                    nedostupniKurseviDGV.SelectedIndex = -1;
                }
                else
                {
                    id = odredjeniKurseviN[nedostupniKurseviDGV.SelectedIndex].ID;
                    nedostupniKurseviDGV.SelectedIndex = -1;
                }
            }

            try
            {
                sacuvajTrenutnoStanje();
                Izmeni izmena = new Izmeni(id);

                if (izmena.ShowDialog() == true)
                {
                    this.inicijalizujTab1();
                    this.PopuniTreeView();
                    this.inicijalizujTab3();

                }
            }
            catch (Exception) {

                MessageBox.Show("Izaberite kurs koji želite da izmenite!", "Greška: nije selektovan kurs", MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

        }

        private void obrisiKurs(object sender, RoutedEventArgs e)
        {
            if (dostupniKurseviDGV.SelectedIndex == -1 && nedostupniKurseviDGV.SelectedIndex == -1)
            {
                MessageBox.Show("Izaberite kurs koji želite da obrišete!", "Greška: nije selektovan kurs", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Da li zaista želite da obrišete izabrani kurs?\n" +
                                "NAPOMENA: Ova operacija se ne može opozvati. Obrisani kurs ćete morati da vratite ručno!",
                                "Obriši kurs?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                if (dostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(dostupniKursevi[dostupniKurseviDGV.SelectedIndex].ID);
                else if (nedostupniKurseviDGV.SelectedIndex != -1)
                    kursevi.Remove(nedostupniKursevi[nedostupniKurseviDGV.SelectedIndex].ID);

                sacuvajTrenutnoStanje();

                this.inicijalizujTab1();
                this.PopuniTreeView();
                this.inicijalizujTab3();

            }

        }

        private void dostupniKurseviDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ignoreIndexChange) return;

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

            if (ignoreIndexChange) return;

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
            odredjeniKurseviD.Clear();
            odredjeniKurseviN.Clear();

            if (pretragaDostupnihTB.Text == "")
            {
                dostupniKurseviDGV.ItemsSource = dostupniKursevi;
                nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;
                searchOn = false;
                return;

            }

            foreach (Kurs k in dostupniKursevi)
            {
                string parametarPretrazivanja = "";
                if (pretragaNazivRB.IsChecked == true)
                    parametarPretrazivanja = k.Naziv.ToLower();
                else if (pretragaKategorijaRB.IsChecked == true)
                    parametarPretrazivanja = k.Kategorija.ToLower();

                if (parametarPretrazivanja.StartsWith(pretragaDostupnihTB.Text.ToLower()))
                    odredjeniKurseviD.Add(k);
            }

            dostupniKurseviDGV.ItemsSource = odredjeniKurseviD;

            foreach (Kurs k in nedostupniKursevi)
            {
                string parametarPretrazivanja = "";
                if (pretragaNazivRB.IsChecked == true)
                    parametarPretrazivanja = k.Naziv.ToLower();
                else if (pretragaKategorijaRB.IsChecked == true)
                    parametarPretrazivanja = k.Kategorija.ToLower();

                if (parametarPretrazivanja.StartsWith(pretragaDostupnihTB.Text.ToLower()))
                    odredjeniKurseviN.Add(k);
            }

            nedostupniKurseviDGV.ItemsSource = odredjeniKurseviN;

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

            if (searchOn)
            {
                MessageBox.Show("\"Prevuci i pusti\" opcija nije dozvoljena tokom pretrage.", "Zabranjena opcija", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


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

            if (searchOn)
            {
                MessageBox.Show("\"Prevuci i pusti\" opcija nije dozvoljena tokom pretrage.", "Zabranjena opcija", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


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
            } catch (Exception) { }
        }



        private Dictionary<int, Kategorija> kategorijeTab3 = new Dictionary<int, Kategorija>();
        private ObservableCollection<Kategorija> kategorijeCollection = new ObservableCollection<Kategorija>();
        private ObservableCollection<Kurs> kurseviOdredjeneKategorije = new ObservableCollection<Kurs>();
        private string exportMsg = "Exported by: Kurs+ © Dimitrije Stankovic, Maja Bogicevic 2024.";
        private void inicijalizujTab3()
        {

            kategorijeTab3 = citac.ucitajKategorije();
            kategorijeCollection = citac.KategorijaToObservableColection();
            kategorijeKursevaDGV.ItemsSource = kategorijeCollection;
            exportBtn.IsEnabled = false;
            kategorijeKursevaDGV.SelectedIndex = -1;


        }


        private void nadjiKurseveOdredjeneKategorije()
        {
            ObservableCollection<Kurs> sviKursevi = citac.KursToObservableColection();
            kurseviKategorijeDGV.ItemsSource = null;
            kurseviOdredjeneKategorije.Clear();

            foreach (Kurs k in sviKursevi)
                if (k.Kategorija == nazivSelektovaneKategorije)
                    kurseviOdredjeneKategorije.Add(k);

            kurseviKategorijeDGV.ItemsSource = kurseviOdredjeneKategorije;
        }





        private string nazivSelektovaneKategorije = "";
        private void kategorijeKursevaDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (kategorijeKursevaDGV.SelectedIndex == -1)
            {
                exportBtn.IsEnabled = false;
                kurseviKategorijeTBl.Text = "KURSEVI KATEGORIJE ";
                kurseviKategorijeDGV.ItemsSource = null;
                return;
            }

            exportBtn.IsEnabled = true;

            nazivSelektovaneKategorije = kategorijeCollection[kategorijeKursevaDGV.SelectedIndex].Naziv;

            kurseviKategorijeTBl.Text = "KURSEVI KATEGORIJE \"" + nazivSelektovaneKategorije + "\"";



            nadjiKurseveOdredjeneKategorije();

        }



        private void exportToExcel(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            app.Visible = true;

            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;

            worksheet.Name = nazivSelektovaneKategorije + "_EXPORTED";


            int[] maksDuzinaPolja = new int[kurseviKategorijeDGV.Columns.Count];
            worksheet.Cells[1, 1] = "iconPath";
            maksDuzinaPolja[0] = (new String("iconPath").Length);
            for (int i = 2; i < kurseviKategorijeDGV.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = kurseviKategorijeDGV.Columns[i - 1].Header.ToString();
                maksDuzinaPolja[i - 1] = kurseviKategorijeDGV.Columns[i - 1].Header.ToString().Length;
            }


            int brojacDuzina = 0;

            for (int i = 0; i < kurseviOdredjeneKategorije.Count; ++i)
            {
                string slika = "\\photos\\" + System.IO.Path.GetFileName(kurseviOdredjeneKategorije[i].SlikaPath);
                worksheet.Cells[i + 2, 1] = slika;
                if (maksDuzinaPolja[brojacDuzina] < slika.Length)
                    maksDuzinaPolja[brojacDuzina] = slika.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 2] = kurseviOdredjeneKategorije[i].ID.ToString();
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].ID.ToString().Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].ID.ToString().Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 3] = kurseviOdredjeneKategorije[i].Naziv;
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].Naziv.Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].Naziv.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 4] = kurseviOdredjeneKategorije[i].Cena.ToString();
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].Cena.ToString().Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].Cena.ToString().Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 5] = kurseviOdredjeneKategorije[i].Kategorija;
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].Kategorija.Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].Kategorija.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 6] = kurseviOdredjeneKategorije[i].Dostupnost;
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].Dostupnost.Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].Dostupnost.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 7] = kurseviOdredjeneKategorije[i].Opis;
                if (maksDuzinaPolja[brojacDuzina] < kurseviOdredjeneKategorije[i].Opis.Length)
                    maksDuzinaPolja[brojacDuzina] = kurseviOdredjeneKategorije[i].Opis.Length;
                brojacDuzina = 0;
            }

            worksheet.Columns[1].ColumnWidth = maksDuzinaPolja[0];
            worksheet.Columns[2].ColumnWidth = maksDuzinaPolja[1];
            worksheet.Columns[3].ColumnWidth = maksDuzinaPolja[2];
            worksheet.Columns[4].ColumnWidth = maksDuzinaPolja[3];
            worksheet.Columns[5].ColumnWidth = maksDuzinaPolja[4];
            worksheet.Columns[6].ColumnWidth = maksDuzinaPolja[5];
            worksheet.Columns[7].ColumnWidth = maksDuzinaPolja[6];


            worksheet.Cells[kurseviOdredjeneKategorije.Count + 3, 1] = exportMsg;
            this.WindowState = WindowState.Minimized;
            app.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMaximized;
            app.ActiveWindow.Activate();

        }

        private void exportToExcelKat(object sender, RoutedEventArgs e)
        {

            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            app.Visible = true;

            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;

            worksheet.Name = "SVE_KATEGORIJE_EXPORTED";

            int[] maksDuzinaPolja = new int[kategorijeCollection.Count];
            worksheet.Cells[1, 1] = "iconPath";
            maksDuzinaPolja[0] = (new String("iconPath").Length);
            for (int i = 2; i < kategorijeKursevaDGV.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = kategorijeKursevaDGV.Columns[i - 1].Header.ToString();
                maksDuzinaPolja[i - 1] = kategorijeKursevaDGV.Columns[i - 1].Header.ToString().Length;
            }

            int brojacDuzina = 0;

            for (int i = 0; i < kategorijeCollection.Count; ++i)
            {
                string slika = "\\photos\\" + System.IO.Path.GetFileName(kategorijeCollection[i].SlikaPath);
                worksheet.Cells[i + 2, 1] = slika;
                if (maksDuzinaPolja[brojacDuzina] < slika.Length)
                    maksDuzinaPolja[brojacDuzina] = slika.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 2] = kategorijeCollection[i].ID.ToString();
                if (maksDuzinaPolja[brojacDuzina] < kategorijeCollection[i].ID.ToString().Length)
                    maksDuzinaPolja[brojacDuzina] = kategorijeCollection[i].ID.ToString().Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 3] = kategorijeCollection[i].Naziv;
                if (maksDuzinaPolja[brojacDuzina] < kategorijeCollection[i].Naziv.Length)
                    maksDuzinaPolja[brojacDuzina] = kategorijeCollection[i].Naziv.Length;
                brojacDuzina++;

                worksheet.Cells[i + 2, 4] = kategorijeCollection[i].Opis;
                if (maksDuzinaPolja[brojacDuzina] < kategorijeCollection[i].Opis.Length)
                    maksDuzinaPolja[brojacDuzina] = kategorijeCollection[i].Opis.Length;
                brojacDuzina = 0;
            }

            worksheet.Columns[1].ColumnWidth = maksDuzinaPolja[0];
            worksheet.Columns[2].ColumnWidth = maksDuzinaPolja[1];
            worksheet.Columns[3].ColumnWidth = maksDuzinaPolja[2];
            worksheet.Columns[4].ColumnWidth = maksDuzinaPolja[3];

            worksheet.Cells[kategorijeCollection.Count + 3, 1] = exportMsg;
            this.WindowState = WindowState.Minimized;
            app.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMaximized;
            app.ActiveWindow.Activate();


        }

        //NAPRAVI TREEVIEW
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



        private List<Kurs> GetSelectedCoursesFromTreeView(TreeView treeView)
        {
            var izabraniKursevi = new List<Kurs>();
            foreach (var item in treeView.Items)
            {
                var treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeViewItem != null && treeViewItem.IsSelected)
                {
                    izabraniKursevi.Add(treeViewItem.DataContext as Kurs);

                }
                GetSelectedCoursesFromTreeViewItem(treeViewItem, izabraniKursevi);
            }
            return izabraniKursevi;
        }

        //BIRANJE KURSA ZA PRIJAVU
        private void GetSelectedCoursesFromTreeViewItem(TreeViewItem item, List<Kurs> izabraniKursevi)
        {
            if (item == null)
            {
                return;
            }

            foreach (var item2 in item.Items)
            {
                var subTreeViewItem = item.ItemContainerGenerator.ContainerFromItem(item2) as TreeViewItem;
                if (subTreeViewItem != null && subTreeViewItem.IsSelected)
                {

                    izabraniKursevi.Add(subTreeViewItem.DataContext as Kurs);
                }
                GetSelectedCoursesFromTreeViewItem(subTreeViewItem, izabraniKursevi);
            }

        }

        //PREBACIVANJE KURSA IZ TREEVIEW U LISTVIEW
        private void MyTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var izabraniKurs = GetSelectedCoursesFromTreeView(MyTreeView);
            foreach (var item in izabraniKurs)
            {
                if (item is Kurs kurs)
                {
                    if (!Korp.Contains(kurs) && kurs.Dostupan)
                    {
                        Korp.Add(kurs);
                    }
                    else if (!kurs.Dostupan)
                    {
                        MessageBox.Show($"Kurs trenutno nije dostupan", "Kurs nedostupan", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Kurs je vec u korpi", "Kurs u korpi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
            Korpa.ItemsSource = Korp;
        }

        private void PopuniTreeView()
        {
            MyTreeView.Items.Clear();
            Dictionary<int, Kategorija> kategorije = citac.ucitajKategorije();// inicijalizujTab2Kategorija();
            foreach (var k in kategorije)
            {
                ;
                TreeViewItem kategorijaNode = NapraviTreeViewItem(k.Value.Naziv, k.Value.Slika);

                Dictionary<int, Kurs> kursevi = citac.ucitajKurseve();//inicijalizujTab2Kurs();
                foreach (var kurs in kursevi)
                {
                    if (k.Value.Naziv == kurs.Value.Kategorija)
                    {
                        //BitmapImage kursSlika = kurs.Value.Slika;
                        TreeViewItem kursNode = NapraviTreeViewItem(kurs.Value.Naziv, kurs.Value.Slika);
                        kursNode.DataContext = kurs.Value;

                        kategorijaNode.Items.Add(kursNode);
                    }
                }
                MyTreeView.Items.Add(kategorijaNode);

            }
        }
        private void Potvrdi_Click(object sender, RoutedEventArgs e)
        {
            var drugi_prozor = new Potvrdi(Korp);
            if (drugi_prozor.ShowDialog() == false)
            {
                Korp.Clear();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //potrebno je reci operativnom sistemu da ako se zatvori glavni prozor ugasi aplikaciju
            //jer u protivnom moze da ostane kao jedan od background procesa - a to ne zelimo
            sacuvajTrenutnoStanje();
            this.Close();
            Application.Current.Shutdown();

        }
       /* private Dictionary<int, Kategorija> kategorijeTab2 = new Dictionary<int, Kategorija>();
        private Dictionary<int, Kurs> kurseviTab2 = new Dictionary<int, Kurs>();

        private Dictionary<int, Kategorija> inicijalizujTab2Kategorija()
        {
            
            kategorijeTab2 = citac.ucitajKategorije();
            
            return kategorijeTab2;

         }

        private Dictionary<int, Kurs> inicijalizujTab2Kurs()
        {

            kurseviTab2 = citac.ucitajKurseve();

            return kurseviTab2;

        }*/
    }
    

    
}