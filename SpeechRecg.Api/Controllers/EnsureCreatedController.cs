using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace SpeechRecg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnsureCreatedController : ControllerBase
    {
        private readonly DBcon _context; 
        public EnsureCreatedController(DBcon context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> EnsureCreated()
        {
            try
            {
                if ((_context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    _context.Database.EnsureCreated();
                }
            }
            catch (Exception ex) { return Ok(ex.ToString()); }
            return Ok("Success.");
        }
    }
}