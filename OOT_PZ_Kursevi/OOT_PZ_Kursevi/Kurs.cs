using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;

namespace OOT_PZ_Kursevi
{
    class Kurs : INotifyPropertyChanged
    {


        #region POLJA
        private int id;
        private string naziv;
        private string opis;
        private double cena;
        private string slikaPath;
        private string kategorija;
        private bool dostupan;
        BitmapImage slika;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region KONSTRUKTOR
        public Kurs(int id, string naziv, string opis, double cena, string slikaPath, string kategorija, bool dostupan)
        {
            this.id = id;
            this.naziv = naziv;
            this.opis = opis;
            this.cena = cena;
            this.slikaPath = slikaPath;
            this.kategorija = kategorija;
            this.slika = new BitmapImage();
            this.slika.BeginInit();

            if (!File.Exists(slikaPath))
                slikaPath = System.IO.Path.GetFullPath("photos\\noIcon.png");

            this.slika.UriSource = new Uri(slikaPath, UriKind.RelativeOrAbsolute);
            this.slika.EndInit();
            this.dostupan = dostupan;
            //verovatno kasnije treba dodati ocitavanje slike
        }
        #endregion

        #region PROPERTY
        public int getId() { return id; }  //id je jedinstven sto znaci da ne moze da se ponovo setuje van konstruktora
                                           //kao geter metoda je da se ne bi mesao sa propertijima
        public string Naziv
        {
            get { return naziv; }
            set
            {
                if (this.naziv != value)
                {
                    this.naziv = value;
                    this.NotifyPropertyChanged("Naziv");
                }
            }
        }
        public string Opis
        {
            get { return opis; }
            set
            {
                if (this.opis != value)
                {
                    this.opis = value;
                    this.NotifyPropertyChanged("Opis");
                }
            }
        }

        public double Cena
        {
            get { return cena; }
            set
            {
                if (this.cena != value)
                {
                    this.cena = value;
                    this.NotifyPropertyChanged("Cena");
                }
            }
        }


        public string SlikaPath
        {
            get { return slikaPath; }
            set
            {
                if (this.slikaPath != value)
                {
                    this.slikaPath = value;
                    this.slika = new BitmapImage();
                    this.slika.BeginInit();

                    if (!File.Exists(slikaPath))
                        slikaPath = System.IO.Path.GetFullPath("photos\\noIcon.png");

                    this.slika.UriSource = new Uri(slikaPath, UriKind.RelativeOrAbsolute);
                    this.slika.EndInit();
                    this.NotifyPropertyChanged("SlikaPath");
                }
            }
        }

        public string Kategorija
        {
            get { return kategorija; }
            set
            {
                if (this.kategorija != value)
                {
                    this.kategorija = value;
                    this.NotifyPropertyChanged("Kategorija");
                }
            }
        }

        public BitmapImage Slika { get {return this.slika; } }

        public FormatConvertedBitmap BWSlika
        {
            get
            {
                FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
                newFormatedBitmapSource.BeginInit();
                newFormatedBitmapSource.Source = this.slika;
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
                newFormatedBitmapSource.EndInit();

                return newFormatedBitmapSource;
            }
        }

        public bool Dostupan { 
            get {  return dostupan; } 
            set {
                if (this.dostupan != value)
                {
                    this.dostupan = value;
                    this.NotifyPropertyChanged("Dostupan");
                }
            } 
        }
      


        #endregion

        #region METODE

        private void NotifyPropertyChanged(string v)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(v));
        }


        #endregion

    }
}
