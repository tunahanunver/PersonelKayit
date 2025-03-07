using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    public class MkController : Controller
    {
        private readonly PersonelDbContext _context;

        public MkController(PersonelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model =  await _context.MedyaKutuphaneleri.Include(m => m.PersonelMedya).ThenInclude(pm => pm.Personel).ToListAsync();
            return View(model);
        }
    }
}
