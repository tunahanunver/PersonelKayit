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
            var model = await _context.MedyaKutuphaneleri.Include(m => m.PersonelMedyalar).ToListAsync();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedyaKutuphanesi medyaKutuphanesi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medyaKutuphanesi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medyaKutuphanesi);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medya = await _context.MedyaKutuphaneleri.FindAsync(id);
            if (medya == null)
            {
                return NotFound();
            }
            return View(medya);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedyaKutuphanesi medyaKutuphanesi)
        {
            if (id != medyaKutuphanesi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medyaKutuphanesi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedyaExists(medyaKutuphanesi.Id))
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
            return View(medyaKutuphanesi);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medya = await _context.MedyaKutuphaneleri
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medya == null)
            {
                return NotFound();
            }

            return View(medya);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medya = await _context.MedyaKutuphaneleri.FindAsync(id);
            if (medya != null)
            {
                _context.MedyaKutuphaneleri.Remove(medya);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MedyaExists(int id)
        {
            return _context.MedyaKutuphaneleri.Any(e => e.Id == id);
        }
    }
}
