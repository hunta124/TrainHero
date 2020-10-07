using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainHero.Entities;
using TrainHero.Model;

namespace TrainHero.DAL
{
    public class TrainHeroContext:DbContext
    {
        public TrainHeroContext(DbContextOptions<TrainHeroContext> options) : base(options)
        {

        }

        public DbSet<tblHero> Hero { get; set; }
        public DbSet<Trainer> Trainer { get; set; }
        public DbSet<tblAbility> Ability { get; set; }
        public DbSet<tblHeroTrainer> HeroTrainer { get; set; }
        public DbSet<tblHeroScheduler> HeroScheduler { get; set; }
        //public DbSet<tblTrainer> Trainer { get; set; }
    }
}
