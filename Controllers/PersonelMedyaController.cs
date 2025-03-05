using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    public class PersonelMedyaController : Controller
    {
        private readonly PersonelDbContext _context;

        public PersonelMedyaController(PersonelDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _context.PersonelMedyalari
                .Include(p => p.Personel)
                .ToListAsync();
            return View(model);
        }
    }
}