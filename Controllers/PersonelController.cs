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
            return View(await _context.Personeller
                .Include(p => p.Lokasyon)
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync());
        }

        // GET: Personel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.Lokasyon)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // GET: Personel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad,Soyad,DogumTarihi,Cinsiyet,Ulke,Sehir,Ilce,Aciklama")] Personel personel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Önce ülke lokasyonunu kontrol et veya oluştur
                    var ulkeLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Ulke && l.ParentId == 0);
                    
                    if (ulkeLokasyon == null)
                    {
                        ulkeLokasyon = new Lokasyon
                        {
                            Name = personel.Ulke,
                            ParentId = 0 // Ülkeler için ParentId = 0
                        };
                        _context.Lokasyonlar.Add(ulkeLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Şehir lokasyonunu kontrol et veya oluştur
                    var sehirLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Sehir && l.ParentId == ulkeLokasyon.Id);
                    
                    if (sehirLokasyon == null)
                    {
                        sehirLokasyon = new Lokasyon
                        {
                            Name = personel.Sehir,
                            ParentId = ulkeLokasyon.Id // Şehrin ParentId'si ülkenin Id'si
                        };
                        _context.Lokasyonlar.Add(sehirLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // İlçe lokasyonunu kontrol et veya oluştur
                    var ilceLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Ilce && l.ParentId == sehirLokasyon.Id);
                    
                    if (ilceLokasyon == null)
                    {
                        ilceLokasyon = new Lokasyon
                        {
                            Name = personel.Ilce,
                            ParentId = sehirLokasyon.Id // İlçenin ParentId'si şehrin Id'si
                        };
                        _context.Lokasyonlar.Add(ilceLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Personeli ilçe lokasyonuna bağla
                    personel.LokasyonId = ilceLokasyon.Id;
                    
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Ulke,Sehir,Ilce,Aciklama")] Personel personel)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Önce ülke lokasyonunu kontrol et veya oluştur
                    var ulkeLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Ulke && l.ParentId == 0);
                    
                    if (ulkeLokasyon == null)
                    {
                        ulkeLokasyon = new Lokasyon
                        {
                            Name = personel.Ulke,
                            ParentId = 0
                        };
                        _context.Lokasyonlar.Add(ulkeLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Şehir lokasyonunu kontrol et veya oluştur
                    var sehirLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Sehir && l.ParentId == ulkeLokasyon.Id);
                    
                    if (sehirLokasyon == null)
                    {
                        sehirLokasyon = new Lokasyon
                        {
                            Name = personel.Sehir,
                            ParentId = ulkeLokasyon.Id
                        };
                        _context.Lokasyonlar.Add(sehirLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // İlçe lokasyonunu kontrol et veya oluştur
                    var ilceLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == personel.Ilce && l.ParentId == sehirLokasyon.Id);
                    
                    if (ilceLokasyon == null)
                    {
                        ilceLokasyon = new Lokasyon
                        {
                            Name = personel.Ilce,
                            ParentId = sehirLokasyon.Id
                        };
                        _context.Lokasyonlar.Add(ilceLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Personeli ilçe lokasyonuna bağla
                    personel.LokasyonId = ilceLokasyon.Id;

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
                .Include(p => p.Lokasyon)
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
