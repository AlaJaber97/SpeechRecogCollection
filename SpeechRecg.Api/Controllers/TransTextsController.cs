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

namespace SpeechRecg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class TransTextsController : ControllerBase
    {
        private readonly DBcon _context;

        public TransTextsController(DBcon context)
        {
            _context = context;
        }

        // GET: api/TransTexts/
        [HttpGet()]
        public async Task<ActionResult<TransText>> GetTransText()
        {try
            {
                var transText = await _context.TransText.Where(c => c.Status == BLL.Enum.Status.Available).OrderBy(c => c.Status == BLL.Enum.Status.Available).FirstOrDefaultAsync();
                if (transText == null)
                {
                    return StatusCode((int)System.Net.HttpStatusCode.NotFound, "There are no more text to work on, you can try at a later time");
                }
                transText.Status = BLL.Enum.Status.Taken;
                _context.TransText.Update(transText);
                await _context.SaveChangesAsync();
                return transText;
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.ToString());
            }
        }


        // POST: api/TransTexts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> PostTransText(List<TransText> transTexts)
        {
            try
            {
                transTexts.ForEach(item => item.CreateTime = DateTime.UtcNow);
                _context.TransText.AddRange(transTexts);
                var result = await _context.SaveChangesAsync();

                return StatusCode((int)System.Net.HttpStatusCode.Created,$"{result}");
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.ExpectationFailed, ex.ToString());
            }
        }
    }
}
