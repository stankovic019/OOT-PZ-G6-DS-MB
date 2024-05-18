using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OOT_PZ_Kursevi 
{
    class Kategorija : INotifyPropertyChanged
    {


        #region POLJA
        private int id;
        private string naziv;
        private string opis;
        private string slikaPath;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region KONSTRUKTOR
        public Kategorija(int id,  string naziv, string opis, string slikaPath)
        {
            this.id = id;
            this.naziv = naziv;
            this.opis = opis;
            this.slikaPath = slikaPath;
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
        public string Opis { 
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
        public string SlikaPath { 
            get { return slikaPath; }
            set
            {
                if (this.slikaPath != value)
                {
                    this.slikaPath = value;
                    this.NotifyPropertyChanged("SlikaPath");
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
