using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainHero.Model
{
    public class tblHero
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ability { get; set; }
        public DateTime startDate { get; set; }
        public int suitColor {get;set;}
        public int startingPower { get; set; }
        public int currentPower { get; set; }
    }
}
