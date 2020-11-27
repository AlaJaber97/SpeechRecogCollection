using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeechRecg.Api;
using BLL.Sql.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;

namespace SpeechRecg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class AudioFilesController : ControllerBase
    {
        private readonly DBcon _context;
        private readonly ClaimsPrincipal userclim;
        private readonly UserManager<User> _userManager;

        public AudioFilesController(DBcon context, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userclim = contextAccessor.HttpContext.User;
        }

        // GET: api/AudioFiles/
        [HttpGet()]
        public async Task<ActionResult<AudioFile>> GetAudioFile()
        {
            try
            {
                AudioFile audioFile = null;
                var user = await _userManager.GetUserAsync(userclim);
                if (user != null)
                {
                    if (BLL.Constants.InfoAdmin.EmailAdmin.Any(emailadmin => string.Equals(emailadmin.Trim().ToLower(), user.Email.Trim().ToLower())))
                        audioFile = await _context.AudioFile.
                        Where(c => c.Status == BLL.Enum.Status.Available && string.Equals(c.ReservedFor.Trim().ToLower(), user.Email.Trim().ToLower())).
                        OrderBy(c => c.Status == BLL.Enum.Status.Available).
                        FirstOrDefaultAsync();

                    if (audioFile == null)
                        audioFile = await _context.AudioFile.
                        Where(c => c.Status == BLL.Enum.Status.Available && string.IsNullOrEmpty(c.ReservedFor)).
                        OrderBy(c => c.Status == BLL.Enum.Status.Available).
                        FirstOrDefaultAsync();

                    if (audioFile == null)
                    {
                        return StatusCode((int)System.Net.HttpStatusCode.NotFound, "There are no more voices to work on, you can try at a later time");
                    }
                    audioFile.Status = BLL.Enum.Status.Taken;
                    _context.AudioFile.Update(audioFile);
                    await _context.SaveChangesAsync();
                    audioFile.Path = $"{BLL.Services.HttpClinetRespones.BaseUrl}/data/info@yulivetranslation.com/{audioFile.NameFile}";
                    return Ok(audioFile);
                }
                else
                    return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, "Email is not authorized in data");
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.ToString());
            }
        }

        // POST: api/AudioFiles
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<AudioFile>> PostAudioFile(AudioFile audioFile)
        {
            try
            {
                var audiofile = _context.AudioFile.Find(audioFile.ID);
                audiofile.CreateTime = DateTime.UtcNow;
                audiofile.Length = audioFile.Length;
                audiofile.SpeakerAge = audioFile.SpeakerAge;
                audiofile.SpeakerGender = audioFile.SpeakerGender;
                audiofile.Status = BLL.Enum.Status.Available;
                audiofile.ReservedFor = audioFile.ReservedFor;
                _context.AudioFile.Update(audiofile);
                await _context.SaveChangesAsync();

                return Ok(audioFile);
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.ToString());
            }
        }
    }
}
