using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    public class PersonelEgitimController : Controller
    {
        private readonly PersonelDbContext _context;

        public PersonelEgitimController(PersonelDbContext context)
        {
            _context = context;
        }

        // GET: PersonelEgitim
        public async Task<IActionResult> Index()
        {
            var personelDbContext = _context.PersonelEgitimleri.Include(p => p.Personel);
            return View(await personelDbContext.ToListAsync());
        }
    }
}
