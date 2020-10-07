using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainHero.Model
{
    public class tblHeroScheduler
    {
        public int id { get; set; }
        public int idHero { get; set; }
        public DateTime workDate { get; set; }
        public int amount { get; set; }
    }
}
