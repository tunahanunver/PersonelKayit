using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
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
            try
            {
                // Tüm personelleri ve lokasyon bilgilerini tek sorguda çekiyoruz
                var personeller = await _context.Personeller
                    .Include(p => p.Lokasyon)
                    .AsNoTracking() // Performans için tracking'i kapatıyoruz
                    .OrderBy(p => p.Id) // ID'ye göre sıralama
                    .ToListAsync();

                // Tüm lokasyonları tek sorguda çekiyoruz
                var lokasyonlar = await _context.Lokasyonlar
                    .AsNoTracking() // Performans için tracking'i kapatıyoruz
                    .ToDictionaryAsync(l => l.Id, l => l);

                // Her personel için ülke ve şehir bilgilerini bellekten alıyoruz
                foreach (var personel in personeller)
                {
                    if (personel.Lokasyon != null)
                    {
                        // İlçenin bağlı olduğu şehri bellekten buluyoruz
                        if (lokasyonlar.TryGetValue(personel.Lokasyon.ParentId, out var sehir))
                        {
                            // Şehrin bağlı olduğu ülkeyi bellekten buluyoruz
                            if (lokasyonlar.TryGetValue(sehir.ParentId, out var ulke))
                            {
                                personel.SetLocationInfo(ulke.Name, sehir.Name);
                            }
                        }
                    }
                }
                return View(personeller);
            }
            catch (Exception ex)
            {
                return View(new List<Personel>());
            }
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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            if (personel.Lokasyon != null)
            {
                // İlçenin şehrini bul
                var sehir = await _context.Lokasyonlar.FindAsync(personel.Lokasyon.ParentId);
                if (sehir != null)
                {
                    // Şehrin ülkesini bul
                    var ulke = await _context.Lokasyonlar.FindAsync(sehir.ParentId);
                    if (ulke != null)
                    {
                        ViewBag.SecilenUlke = ulke.Name;
                        ViewBag.SecilenSehir = sehir.Name;
                        ViewBag.SecilenIlce = personel.Lokasyon.Name;
                    }
                }
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
        public async Task<IActionResult> Create([Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama")] Personel personel, IFormFile? Image, string ulke, string sehir, string ilce)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Ülke kontrolü
                    var ulkeLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == ulke && l.ParentId == 0);

                    if (ulkeLokasyon == null)
                    {
                        ulkeLokasyon = new Lokasyon { Name = ulke, ParentId = 0 };
                        _context.Lokasyonlar.Add(ulkeLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Şehir kontrolü
                    var sehirLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == sehir && l.ParentId == ulkeLokasyon.Id);

                    if (sehirLokasyon == null)
                    {
                        sehirLokasyon = new Lokasyon { Name = sehir, ParentId = ulkeLokasyon.Id };
                        _context.Lokasyonlar.Add(sehirLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // İlçe kontrolü
                    var ilceLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == ilce && l.ParentId == sehirLokasyon.Id);

                    if (ilceLokasyon == null)
                    {
                        ilceLokasyon = new Lokasyon { Name = ilce, ParentId = sehirLokasyon.Id };
                        _context.Lokasyonlar.Add(ilceLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    if (Image is not null)
                    {
                        string guidıd = Guid.NewGuid().ToString();
                        string fileextension = Path.GetExtension(Image.FileName); // .jpg, .png vb.
                        string newfilename = $"{guidıd}{fileextension}"; // örn: "b7f8e123-9c4d-4db1-a1ad-2d678a1e0345.jpg"
                        string klasor = Directory.GetCurrentDirectory() + "/wwwroot/MedyaKutuphanesi/" + newfilename;
                        using var stream = new FileStream(klasor, FileMode.Create);
                        Image.CopyTo(stream);
                        personel.Image = newfilename;
                    }

                    personel.LokasyonId = ilceLokasyon.Id;
                    _context.Add(personel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt eklenirken bir hata oluştu.");
                ModelState.AddModelError("", $"Hata Detayı: {ex.StackTrace}");
            }
            return View(personel);
        }

        // GET: Personel/Edit/5
        public async Task<IActionResult> Edit(int? id, IFormFile Image)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.Lokasyon)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            if (personel.Lokasyon != null)
            {
                // İlçenin şehrini bul
                var sehir = await _context.Lokasyonlar.FindAsync(personel.Lokasyon.ParentId);
                if (sehir != null)
                {
                    // Şehrin ülkesini bul
                    var ulke = await _context.Lokasyonlar.FindAsync(sehir.ParentId);
                    if (ulke != null)
                    {
                        // ViewBag ile bu bilgileri view'a gönder
                        ViewBag.SecilenUlke = ulke.Name;
                        ViewBag.SecilenSehir = sehir.Name;
                        ViewBag.SecilenIlce = personel.Lokasyon.Name;
                    }
                }
            }
            return View(personel);
        }

        // POST: Personel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama")] Personel personel, IFormFile? Image, string ulke, string sehir, string ilce)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Ülke kontrolü
                    var ulkeLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == ulke && l.ParentId == 0);

                    if (ulkeLokasyon == null)
                    {
                        ulkeLokasyon = new Lokasyon { Name = ulke, ParentId = 0 };
                        _context.Lokasyonlar.Add(ulkeLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // Şehir kontrolü
                    var sehirLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == sehir && l.ParentId == ulkeLokasyon.Id);

                    if (sehirLokasyon == null)
                    {
                        sehirLokasyon = new Lokasyon { Name = sehir, ParentId = ulkeLokasyon.Id };
                        _context.Lokasyonlar.Add(sehirLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    // İlçe kontrolü
                    var ilceLokasyon = await _context.Lokasyonlar
                        .FirstOrDefaultAsync(l => l.Name == ilce && l.ParentId == sehirLokasyon.Id);

                    if (ilceLokasyon == null)
                    {
                        ilceLokasyon = new Lokasyon { Name = ilce, ParentId = sehirLokasyon.Id };
                        _context.Lokasyonlar.Add(ilceLokasyon);
                        await _context.SaveChangesAsync();
                    }

                    if (Image is not null)
                    {
                        string guidıd = Guid.NewGuid().ToString();
                        string fileextension = Path.GetExtension(Image.FileName); // .jpg, .png vb.
                        string newfilename = $"{guidıd}{fileextension}"; // örn: "b7f8e123-9c4d-4db1-a1ad-2d678a1e0345.jpg"
                        string klasor = Directory.GetCurrentDirectory() + "/wwwroot/MedyaKutuphanesi/" + newfilename;
                        using var stream = new FileStream(klasor, FileMode.Create);
                        Image.CopyTo(stream);
                        personel.Image = newfilename;
                    }

                    personel.LokasyonId = ilceLokasyon.Id;
                    _context.Update(personel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt güncellenirken bir hata oluştu.");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            }
            return View(personel);
        }

        // GET: Personel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.Lokasyon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            if (personel.Lokasyon != null)
            {
                // İlçenin şehrini bul
                var sehir = await _context.Lokasyonlar.FindAsync(personel.Lokasyon.ParentId);
                if (sehir != null)
                {
                    // Şehrin ülkesini bul
                    var ulke = await _context.Lokasyonlar.FindAsync(sehir.ParentId);
                    if (ulke != null)
                    {
                        ViewBag.SecilenUlke = ulke.Name;
                        ViewBag.SecilenSehir = sehir.Name;
                        ViewBag.SecilenIlce = personel.Lokasyon.Name;
                    }
                }
            }

            return View(personel);
        }

        // POST: Personel/Delete/5
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
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
