using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OOT_PZ_Kursevi
{
    class Citac
    {
        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();

        private Dictionary<int, Kategorija> kategorije = new Dictionary<int, Kategorija>();

        public Citac() { }

        public Dictionary<int, Kurs> ucitajKurseve()
        {


            string sadrzajDatoteke = File.ReadAllText(@"datoteke\kursevi.dat");

            if (sadrzajDatoteke == "")
                return kursevi;

            string[] podeliRedove = sadrzajDatoteke.Split("\r\n"); // \r\n je Windows nacin za pamcenje novog reda

            foreach (string line in podeliRedove)
            {
                string[] polja = line.Split(":;:");

                string path = polja[4];

                if (!System.IO.Path.Exists(path))
                    path = Environment.CurrentDirectory + polja[4];
                    

                Kurs k = new Kurs(Convert.ToInt32(polja[0]), polja[1], polja[2], Convert.ToDouble(polja[3]), path, polja[5],
                                  (polja[6].ToLower() == "true" ? true : false));

                if (!kursevi.ContainsKey(k.getId()))
                    kursevi.Add(k.getId(), k);
                else
                    propertyChange(kursevi[k.ID], k);
                
            }

            return kursevi;
        }

        private void propertyChange(Kurs stari, Kurs novi)
        {
            stari.Naziv = novi.Naziv;
            stari.Opis = novi.Opis;
            stari.Cena = novi.Cena;
            stari.SlikaPath = novi.SlikaPath;
            stari.Kategorija = novi.Kategorija;
            stari.Dostupan = novi.Dostupan;
        }


        public ObservableCollection<Kurs> KursToObservableColection(bool dostupan)
        {
            ObservableCollection<Kurs> kurs = new ObservableCollection<Kurs>();

            foreach (int key in kursevi.Keys)
                if (kursevi[key].Dostupan == dostupan)
                    kurs.Add(kursevi[key]);

            return kurs;

        }

        public ObservableCollection<Kurs> KursToObservableColection()
        {
            ObservableCollection<Kurs> kurs = new ObservableCollection<Kurs>();

            foreach (int key in kursevi.Keys)
                    kurs.Add(kursevi[key]);

            return kurs;

        }

        public Dictionary<int, Kategorija> ucitajKategorije()
        {
            string sadrzajDatoteke = File.ReadAllText(@"datoteke\kategorije.dat");

            string[] podeliRedove = sadrzajDatoteke.Split("\r\n"); // \r\n je Windows nacin za pamcenje novog reda

            foreach (string line in podeliRedove)
            {

                string[] polja = line.Split(":;:");

                string path = polja[3];

                if (!System.IO.Path.Exists(path))
                    path = Environment.CurrentDirectory + polja[3];


                Kategorija k = new Kategorija(Convert.ToInt32(polja[0]), polja[1], polja[2],  path);
                
                if (!kategorije.ContainsKey(k.getId()))
                    kategorije.Add(k.getId(), k);
            }

            return kategorije;
        }

        public ObservableCollection<Kategorija> KategorijaToObservableColection()
        {
            ObservableCollection<Kategorija> kate = new ObservableCollection<Kategorija>();

            foreach (int key in kategorije.Keys)
                    kate.Add(kategorije[key]);

            return kate;

        }




    }
}
