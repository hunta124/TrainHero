using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrainHero.DAL;
using TrainHero.Entities;
using TrainHero.Helpers;

namespace TrainHero.Service
{
    public interface ITrainService
    {
        Trainer Autheticate(string name, string password);
        Trainer Create(string name, string password);
        Trainer GetById(int id);
    }

    public class TrainService:ITrainService
    {

        private readonly TrainHeroContext _context;

        public TrainService(TrainHeroContext context)
        {
            _context = context;
        }
        public Trainer Autheticate(string name, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
               return null;

            var trainer = _context.Trainer.SingleOrDefault(e => e.Name == name);
            if (trainer == null)
                return null; ;

            if (!VerifyPasswordHash(password, trainer.PasswordHash, trainer.PasswordSalt))
                return null;
                
            return trainer;
        }

        public Trainer Create(string name, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Trainer.Any(x => x.Name == name))
                throw new AppException("Username \"" + name + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            Trainer train = new Trainer();
            train.Name = name;
            train.PasswordHash = passwordHash;
            train.PasswordSalt = passwordSalt;

            _context.Trainer.Add(train);
            _context.SaveChanges();

            return train;
        }

        public Trainer GetById(int id)
        {
            return _context.Trainer.Find(id);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (password.Length < 8) throw new ArgumentException("Yout password have to be at least 8 characters");
            Regex reg = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");
            if (!reg.IsMatch(password)) throw new ArgumentException("Your password does not meet minimum security requierment!","password");
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
