using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainHero.DAL;
using TrainHero.Helpers;
using TrainHero.Model;

namespace TrainHero.Service
{
    public interface IHeroService
    {
        tblHero Create(tblHero hero,int idTrainer);
        tblHero GetHeroById(int idHero);
        List<tblHero> GetListHeroesByTrainer(int idTrainer);
        bool SetTraining(int idHero, int idTrainer);
    }
    public class HeroService : IHeroService
    {
        private readonly TrainHeroContext _context;

        public HeroService(TrainHeroContext context)
        {
            _context = context;
        }
        public tblHero Create(tblHero hero, int idTrainer)
        {
            if(_context.Hero.Any(x=>x.name == hero.name))
                throw new AppException("Hero with name: \"" + hero.name + "\" is already exist");

            int pwr = GetSomePower();
            hero.startingPower = pwr;
            hero.currentPower = pwr;

            _context.Hero.Add(hero);
            _context.SaveChanges();
            return hero;
        }

        public tblHero GetHeroById(int idHero)
        {
           return _context.Hero.SingleOrDefault(x => x.id == idHero);
        }

        public List<tblHero> GetListHeroesByTrainer(int idTrainer)
        {
            if (!_context.Trainer.Any(x => x.id == idTrainer))
                throw new AppException("The trainer with given id doe not exist");

            if (!_context.HeroTrainer.Any(x => x.idTrainer == idTrainer))
                throw new AppException("The given trainer does not have a heroes for training!");

            var result = from a in _context.Trainer
                         join b in _context.HeroTrainer on a.id equals b.idTrainer
                         join c in _context.Hero on b.idHero equals c.id
                         orderby c.currentPower descending
                         select new tblHero { id = c.id, name = c.name, ability = c.ability,startDate = c.startDate, currentPower = c.currentPower,suitColor=c.suitColor,startingPower=c.startingPower };

            return result.ToList<tblHero>();
        }

        private int GetSomePower()
        {
            Random rnd = new Random();
            return rnd.Next(1, 10);
        }

        public bool SetTraining(int idHero, int idTrainer)
        {
            string today = DateTime.Today.ToString("dd/MM/yyyy");
            if(!_context.HeroScheduler.Any(x=>x.idHero==idHero && x.workDate.ToString("dd/MM/yyyy")==today))
            {
                tblHeroScheduler hs = new tblHeroScheduler();
                hs.idHero = idHero;
                hs.workDate = Convert.ToDateTime(today);
                hs.amount = 1;
                _context.HeroScheduler.Add(hs);
                _context.SaveChanges();    
            }
            else
            {
                tblHeroScheduler hss = _context.HeroScheduler.SingleOrDefault(x => x.idHero == idHero && x.workDate.ToString("dd/MM/yyyy") == today);
                if (hss.amount < 5)
                {
                    hss.amount += 1;
                    _context.HeroScheduler.Update(hss);
                    _context.SaveChanges();
                }
                else
                    throw new AppException("Amount of you training for today is exceeded");

            }

            tblHero h = GetHeroById(idHero);
            h.currentPower += GetSomePower();
            _context.Hero.Update(h);
            _context.SaveChanges();

            return true;
        }
    }
}
