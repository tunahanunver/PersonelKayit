using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    public class PersonelController : Controller
    {
        private readonly PersonelDbContext _context;

        public PersonelController(PersonelDbContext context)
        {
            _context = context;
        }

        // GET: Personel
        public async Task<IActionResult> Index()
        {
            return View(await _context.Personeller.AsNoTracking().OrderBy(p => p.Id).ToListAsync());
        }

        // GET: Personel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad,Soyad,DogumTarihi,Cinsiyet,Ulke,Sehir,Aciklama")] Personel personel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(personel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Kayıt eklenirken bir hata oluştu.");
            }
            return View(personel);
        }

        // GET: Personel/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller.FindAsync(id);
            if (personel == null)
            {
                return NotFound();
            }
            return View(personel);
        }

        // POST: Personel/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Ulke,Sehir,Aciklama")] Personel personel)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(personel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonelExists(personel.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            }
            return View(personel);
        }

        // GET: Personel/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // POST: Personel/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var personel = await _context.Personeller.FindAsync(id);
                if (personel != null)
                {
                    _context.Personeller.Remove(personel);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Silme işlemi sırasında bir hata oluştu.");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }
    }
}
