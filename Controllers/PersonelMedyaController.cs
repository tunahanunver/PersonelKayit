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

        public async Task<IActionResult> Details(int id)
        {
            var pMedia = await _context.PersonelMedyalari.FindAsync(id);
            if (pMedia == null)
            {
                return NotFound();
            }
            return View(pMedia);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Lokasyon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonelMedya personelMedya)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personelMedya);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personelMedya);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pMedya = await _context.PersonelMedyalari.FindAsync(id);
            if (pMedya == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: Lokasyon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonelMedya personelMedya)
        {
            if (id != personelMedya.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personelMedya);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!pMedyaExists(personelMedya.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personelMedya);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pMedya = await _context.PersonelMedyalari.FindAsync(id);
            if (pMedya == null)
            {
                return NotFound();
            }
            return View(pMedya);
        }

        // POST: Lokasyon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pMedya = await _context.PersonelMedyalari.FindAsync(id);
            if (pMedya != null)
            {
                _context.PersonelMedyalari.Remove(pMedya);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool pMedyaExists(int id)
        {
            return _context.PersonelMedyalari.Any(pm => pm.Id == id);
        }
    }
}