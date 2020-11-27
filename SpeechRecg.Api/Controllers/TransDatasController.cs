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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.RegularExpressions;

namespace SpeechRecg.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class TransDatasController : ControllerBase
    {
        private readonly DBcon _context;
        private readonly ClaimsPrincipal userclim;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _environment;
        public TransDatasController(DBcon context, IHttpContextAccessor contextAccessor, IHostingEnvironment environment, UserManager<User> userManager)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
            _userManager = userManager;
            userclim = contextAccessor.HttpContext.User;
        }


        // POST: api/TransDatas
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<BLL.Sql.ViewModels.TransData>> PostTransData(BLL.Sql.ViewModels.TransData transData)
        {
            try
            {
                var user = await _userManager.GetUserAsync(userclim);
                if (user != null)
                {
                    transData.UserID = user.Id;
                    if (transData.TypePost == BLL.Enum.TypePost.Audio)
                    {
                        if (transData.TransText.Status == BLL.Enum.Status.Rejected)
                        {
                            var textinfo = _context.TransText.Find(transData.TransText.ID);
                            textinfo.Status = BLL.Enum.Status.Rejected;
                            _context.TransText.Update(textinfo);
                        }
                        else
                        {
                            var audioInfo = _context.AudioFile.Find(transData.AudioFile.ID);
                            audioInfo.Length = transData.AudioFile.Length;
                            audioInfo.SpeakerAge = user.Age;
                            audioInfo.SpeakerGender = user.Gender;
                            audioInfo.Status = BLL.Enum.Status.Completed;
                            audioInfo.CreateTime = DateTime.UtcNow;
                            _context.AudioFile.Update(audioInfo);

                            var textinfo = _context.TransText.Find(transData.TransText.ID);
                            textinfo.Status = BLL.Enum.Status.Completed;
                            if (!string.Equals(textinfo.Text, transData.TransText.Text))
                            {
                                var pattern = new Regex("[\t\r]|[ ]{2,}");
                                transData.TransText.Text = pattern.Replace(transData.TransText.Text, " ");
                                transData.TransText.Text = transData.TransText.Text.Trim();

                                textinfo.Text = transData.TransText.Text;
                                _context.TransText.Update(textinfo);
                            }
                        }
                    }
                    if (transData.TypePost == BLL.Enum.TypePost.Text)
                    {
                        if (transData.AudioFile.Status == BLL.Enum.Status.Rejected)
                        {
                            var audioInfo = _context.AudioFile.Find(transData.AudioFile.ID);
                            audioInfo.Status = BLL.Enum.Status.Rejected;
                            _context.AudioFile.Update(audioInfo);
                        }
                        else
                        {
                            var audioInfo = _context.AudioFile.Find(transData.AudioFile.ID);
                            audioInfo.Status = BLL.Enum.Status.Completed;
                            _context.AudioFile.Update(audioInfo);

                            var pattern = new Regex("[\t\r]|[ ]{2,}");
                            transData.TransText.Text = pattern.Replace(transData.TransText.Text, " ");
                            transData.TransText.Text = transData.TransText.Text.Trim();
                            transData.TransText.Status = BLL.Enum.Status.Completed;
                            transData.TransText.CreateTime = DateTime.UtcNow;
                            _context.TransText.Add(transData.TransText);
                        }
                    }
                    if (transData.AudioFile.Status != BLL.Enum.Status.Rejected &&
                        transData.TransText.Status != BLL.Enum.Status.Rejected)
                    {
                        transData.ID = Guid.NewGuid();
                        _context.TransData.Add(new TransData
                        {
                            ID = transData.ID,
                            AudioFileID = transData.AudioFile.ID,
                            TransTextID = transData.TransText.ID,
                            UserID = transData.UserID,
                            IsValid = false,
                            ReviewBy = null,
                            CreateTime = DateTime.UtcNow
                        });
                        await _context.SaveChangesAsync();
                        return Ok(transData);
                    }
                    else
                        return Ok();
                }
                else
                    return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, "Email is not authorized in data");
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.Message);
            }
        }
        [HttpPost("audio/{id}")]
        public async Task<ActionResult<AudioFile>> PostAudiofile(Guid id, IFormFile file)
        {
            try
            {
                if (file == null) return StatusCode(StatusCodes.Status400BadRequest, "File is NLL");
                var user = await _userManager.GetUserAsync(userclim);
                if (id != Guid.Empty)
                {
                    if (file != null)
                    {
                        var filename = Guid.NewGuid().ToString() + ".wav";
                        if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                        {
                            _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        }

                        var _path = Path.Combine(_environment.WebRootPath, "Data", user.Email.ToLower());

                        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

                        if (file.Length > 0)
                        {
                            using (var fileStream = new FileStream(Path.Combine(_path, filename), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                        }
                        var dbPath = Path.Combine(_path, filename);
                        var audiofile = new AudioFile
                        {
                            ID = id,
                            Path = dbPath,
                            NameFile = filename,

                        };
                        _context.AudioFile.Add(audiofile);
                    }
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction("PostAudiofile", new { id = id }, null);
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.ToString());
            }

        }
    }
}
