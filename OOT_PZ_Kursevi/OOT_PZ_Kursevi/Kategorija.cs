using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace OOT_PZ_Kursevi
{
    class Kategorija : INotifyPropertyChanged
    {


        #region POLJA
        private int id;
        private string naziv;
        private string opis;
        private string slikaPath;
        BitmapImage slika;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region KONSTRUKTOR
        public Kategorija(int id, string naziv, string opis, string slikaPath)
        {
            this.id = id;
            this.naziv = naziv;
            this.opis = opis;
            this.slikaPath = slikaPath;
            this.slika = new BitmapImage();
            this.slika.BeginInit();

            if (!File.Exists(slikaPath))
                slikaPath = System.IO.Path.GetFullPath("photos\\noIcon.png");

            this.slika.UriSource = new Uri(slikaPath, UriKind.RelativeOrAbsolute);
            this.slika.EndInit();
        }
        #endregion

        #region PROPERTY
        public int getId() { return id; }  //id je jedinstven sto znaci da ne moze da se ponovo setuje van konstruktora
                                            //kao geter metoda je da se ne bi mesao sa propertijima

        public int ID { get { return id; } }

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

        public BitmapImage Slika { get { return this.slika; } }
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
