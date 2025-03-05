using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    public class LokasyonController : Controller
    {
        private readonly PersonelDbContext _context;

        public LokasyonController(PersonelDbContext context)
        {
            _context = context;
        }

        // GET: Lokasyon
        public async Task<IActionResult> Index()
        {
            var model = await _context.Lokasyonlar
                .OrderBy(l => l.Id)
                .ToListAsync();
            return View(model);
        }

        // GET: Lokasyon/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var lokasyon = await _context.Lokasyonlar.FindAsync(id);
            if (lokasyon == null)
            {
                return NotFound();
            }
            return View(lokasyon);
        }

        // GET: Lokasyon/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lokasyon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ParentId")] Lokasyon lokasyon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lokasyon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lokasyon);
        }

        // GET: Lokasyon/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var lokasyon = await _context.Lokasyonlar.FindAsync(id);
            if (lokasyon == null)
            {
                return NotFound();
            }
            return View(lokasyon);
        }

        // POST: Lokasyon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ParentId")] Lokasyon lokasyon)
        {
            if (id != lokasyon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lokasyon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LokasyonExists(lokasyon.Id))
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
            return View(lokasyon);
        }

        // GET: Lokasyon/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var lokasyon = await _context.Lokasyonlar.FindAsync(id);
            if (lokasyon == null)
            {
                return NotFound();
            }
            return View(lokasyon);
        }

        // POST: Lokasyon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lokasyon = await _context.Lokasyonlar.FindAsync(id);
            if (lokasyon != null)
            {
                _context.Lokasyonlar.Remove(lokasyon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LokasyonExists(int id)
        {
            return _context.Lokasyonlar.Any(e => e.Id == id);
        }
    }
}
