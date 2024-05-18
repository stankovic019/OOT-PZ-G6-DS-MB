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

            kursevi = citac.ucitajKurseve();
            dostupniKursevi = citac.KursToObservableColection(true);
            dostupniKurseviDGV.ItemsSource = dostupniKursevi;

            nedostupniKursevi = citac.KursToObservableColection(false);
            nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;



        }

       
    }
}