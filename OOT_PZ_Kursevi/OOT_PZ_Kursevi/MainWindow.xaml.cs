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
        private ObservableCollection<Kurs> dostupniKursevi = new ObservableCollection<Kurs> ();
        private ObservableCollection<Kurs> nedostupniKursevi = new ObservableCollection<Kurs> ();

        public MainWindow()
        {
            InitializeComponent();

           
            dostupniKursevi.Add(new Kurs(1, "C Programiranje", "niga", 32, "", "Predavanja"));
            dostupniKursevi.Add(new Kurs(2, "OOP C#", "niiga", 44, "", "Vezbe"));
            dostupniKursevi.Add(new Kurs(3, "Vega IT", "opalac", 0, "", "Praksa"));
            
            dostupniKurseviDGV.ItemsSource = dostupniKursevi;

            
            
            nedostupniKursevi.Add(new Kurs(3, "Vega IT", "opalac", 0, "", "Praksa"));
            nedostupniKursevi.Add(new Kurs(2, "OOP C#", "niiga", 44, "", "Vezbe"));
            nedostupniKursevi.Add(new Kurs(1, "C Programiranje", "niga", 32, "", "Predavanja"));

            nedostupniKurseviDGV.ItemsSource = nedostupniKursevi;



        }

        private void dostupniKurseviDGV_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}