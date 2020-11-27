using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SpeechRecg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<BLL.Sql.Models.User> _userManager;
        private readonly SignInManager<BLL.Sql.Models.User> _signInManager;

        private readonly IConfiguration _configuration;
        private readonly DBcon _context;
        private readonly string UserId;

        public AccountController(UserManager<BLL.Sql.Models.User> userManager, SignInManager<BLL.Sql.Models.User> SignInManager, DBcon context, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = SignInManager;
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<object> Login([FromBody] BLL.Sql.Models.LoginAndRegister.Login model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return StatusCode((int)System.Net.HttpStatusCode.BadRequest, "Email can not be empty");
                }
                var result = await _signInManager.PasswordSignInAsync(model.Email, "123456", false, false);

                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                    return Ok(await BLL.Services.JWT.GenerateJwtToken(model.Email, appUser, _configuration));
                }
                else
                {
                    if (result.IsLockedOut) return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, "Is locked out");
                    if (result.IsNotAllowed) return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, "Is not allowed");
                    if (result.RequiresTwoFactor) return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, "Requires two factor");
                    return StatusCode((int)System.Net.HttpStatusCode.NotFound, "Email does not match");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.Message);
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<object> Register([FromBody] BLL.Sql.Models.LoginAndRegister.Register model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return StatusCode((int)System.Net.HttpStatusCode.BadRequest, "Email can not be empty");
                }
                if ((await _userManager.FindByEmailAsync(model.Email)) != null)
                {
                    return StatusCode((int)System.Net.HttpStatusCode.BadRequest, $"User name '{model.Email}' is already taken");
                }
                var user = new BLL.Sql.Models.User
                {
                    UserName = model.Email.Trim(),
                    Email = model.Email.Trim(),
                    Gender = model.Gender,
                    Age = DateTime.Now.Year - model.Birthday.Year
                };
                var result = await _userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Ok(await BLL.Services.JWT.GenerateJwtToken(model.Email, user, _configuration));
                }
                else
                    return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, result.Errors);

            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.Message);
            }
        }


        [HttpGet("GetTimeOfWork/{Email}")]
        //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
        [AllowAnonymous]
        public async Task<object> GetTimeOfWork(string Email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    return StatusCode((int)System.Net.HttpStatusCode.BadRequest, "Email can not be empty");
                }
                else
                {
                    var UserID = _context.Users.FirstOrDefault(user => string.Equals(Email, user.Email))?.Id;
                    if(UserID == null) return StatusCode((int) System.Net.HttpStatusCode.BadRequest, "This e-mail is not registered in the DataBase");
                    var UserInfoWork = _context.TransData.Where(column => column.UserID == UserID)?.Select(column => column.AudioFileID);
                    if(UserInfoWork.Count() < 1) return Ok(TimeSpan.FromSeconds(0));
                    var AudioWorkOn = _context.AudioFile.Where(item => UserInfoWork.Contains(item.ID)).ToList();
                    var TimeOfWork = AudioWorkOn.Select(column=>column.Length).Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

                    return Ok(TimeOfWork);
                }

            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.Message);
            }
        }
    }
}