using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainHero.Entities
{
    public class Trainer
    {
        public int id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
