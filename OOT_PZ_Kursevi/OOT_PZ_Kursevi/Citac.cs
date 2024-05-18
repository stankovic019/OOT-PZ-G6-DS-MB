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

            string[] podeliRedove = sadrzajDatoteke.Split("\r\n"); // \r\n je Windows nacin za pamcenje novog reda

            foreach (string line in podeliRedove)
            {
                string[] polja = line.Split(":;:");

                Kurs k = new Kurs(Convert.ToInt32(polja[0]), polja[1], polja[2], Convert.ToDouble(polja[3]), polja[4], polja[5],
                                  (polja[6] == "true" ? true : false));

                if (!kursevi.ContainsKey(k.getId()))
                    kursevi.Add(k.getId(), k);
            }

            return kursevi;
        }

        public ObservableCollection<Kurs> KursToObservableColection(bool dostupan)
        {
            ObservableCollection<Kurs> kurs = new ObservableCollection<Kurs>();

            foreach (int key in kursevi.Keys)
                if (kursevi[key].Dostupan == dostupan)
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

                Kategorija k = new Kategorija(Convert.ToInt32(polja[0]), polja[1], polja[2],  polja[3]);
                
                if (!kategorije.ContainsKey(k.getId()))
                    kategorije.Add(k.getId(), k);
            }

            return kategorije;
        }


    }
}
