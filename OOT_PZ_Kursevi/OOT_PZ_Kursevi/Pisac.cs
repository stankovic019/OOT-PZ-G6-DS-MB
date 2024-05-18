using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace OOT_PZ_Kursevi
{
    class Pisac
    {
        private Dictionary<int, Kurs> kursevi = new Dictionary<int, Kurs>();

        private Dictionary<int, Kategorija> kategorije = new Dictionary<int, Kategorija>();

        public Pisac(Dictionary<int, Kurs> k) 
        {
            this.kursevi = k;
        }

        public void upisi()
        {
            string upis = "";

            foreach(Kurs k in kursevi.Values) 
            {
                upis += k.toDat();            
            }

            File.WriteAllText(@"datoteke\kursevi.dat", upis.Trim());

        }


    }
}
