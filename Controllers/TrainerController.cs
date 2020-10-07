using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrainHero.DAL;
using TrainHero.Helpers;
using TrainHero.Model;
using TrainHero.Service;

namespace TrainHero.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("AnotherPolicy")]
    public class TrainerController : ControllerBase
    {
        private readonly TrainHeroContext _context;
        private ITrainService _train;
        private IHeroService _hero;
        private readonly AppSettings _appSettings;
        public TrainerController(ITrainService train, IHeroService hero, IOptions<AppSettings> appSettings)
        {
            _train = train;
            _hero = hero;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(string traiName, string password)
        {
            var train = _train.Autheticate(traiName, password);
            if (train == null)
                BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, train.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = train.id,
                Username = train.Name,
                Token = tokenString
            });
            // return View();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(string name, string password)
        {
            // map model to entity
            //  var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _train.Create(name, password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("setHero")]
        public IActionResult SetHero(tblHero model, int idTrain)
        {
            try
            {
                _hero.Create(model, idTrain);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("getHeros")]
        public IActionResult GetHeroes(int idTrainer)
        {
            try
            {
                return Ok(_hero.GetListHeroesByTrainer(idTrainer));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("setTraining")]
        public IActionResult SetTraining(int idHero, int idTrainer)
        {
            try
            {
                if (!_hero.SetTraining(idHero, idTrainer))
                    BadRequest(new { message = "Something does wrong!Try again!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
            return Ok();
        }
    }
}