using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using PersonelKayit.Migrations;
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

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            ViewBag.PersonelMedyalar = personelMedyalar;

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
        public async Task<IActionResult> Create([Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama")] Personel personel, List<IFormFile?> Images, string ulke, string sehir, string ilce)
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

                    personel.LokasyonId = ilceLokasyon.Id;
                    _context.Add(personel);
                    await _context.SaveChangesAsync();

                    // Resimleri işle
                    if (Images != null && Images.Count > 0)
                    {
                        foreach (var image in Images)
                        {
                            if (image.Length > 0)
                            {
                                string guidId = Guid.NewGuid().ToString();
                                string fileExtension = Path.GetExtension(image.FileName);
                                string newFileName = $"{guidId}{fileExtension}";
                                string klasor = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", newFileName);

                                using (var stream = new FileStream(klasor, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                // PersonelMedya oluştur
                                var personelMedya = new PersonelMedya
                                {
                                    Name = newFileName,
                                    PersonelId = personel.Id
                                };

                                _context.PersonelMedyalari.Add(personelMedya);
                                await _context.SaveChangesAsync();

                                // MedyaKutuphanesi oluştur
                                var medyaKutuphanesi = new MedyaKutuphanesi
                                {
                                    MedyaAdi = image.FileName,
                                    MedyaGuid = guidId,
                                    PersonelMedyaId = personelMedya.Id
                                };

                                _context.MedyaKutuphaneleri.Add(medyaKutuphanesi);
                                await _context.SaveChangesAsync();

                                // İlk resmi personel.Image olarak ayarla (eğer henüz ayarlanmamışsa)
                                if (string.IsNullOrEmpty(personel.Image))
                                {
                                    personel.Image = newFileName;
                                }
                            }
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt eklenirken bir hata oluştu.");

            }
            return View(personel);
        }

        // GET: Personel/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            ViewBag.PersonelMedyalar = personelMedyalar;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama,Image")] Personel personel, List<IFormFile> Images, string ulke, string sehir, string ilce)
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

                    var existingPersonel = await _context.Personeller.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Resimleri işle
                    if (Images != null && Images.Count > 0)
                    {
                        foreach (var image in Images)
                        {
                            if (image.Length > 0)
                            {
                                string guidId = Guid.NewGuid().ToString();
                                string fileExtension = Path.GetExtension(image.FileName);
                                string newFileName = $"{guidId}{fileExtension}";
                                string klasor = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", newFileName);

                                using (var stream = new FileStream(klasor, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                // PersonelMedya oluştur
                                var personelMedya = new PersonelMedya
                                {
                                    Name = newFileName,
                                    PersonelId = personel.Id
                                };

                                _context.PersonelMedyalari.Add(personelMedya);
                                await _context.SaveChangesAsync();

                                // MedyaKutuphanesi oluştur
                                var medyaKutuphanesi = new MedyaKutuphanesi
                                {
                                    MedyaAdi = image.FileName,
                                    MedyaGuid = guidId,
                                    PersonelMedyaId = personelMedya.Id
                                };

                                _context.MedyaKutuphaneleri.Add(medyaKutuphanesi);
                                await _context.SaveChangesAsync();

                                // İlk resmi personel.Image olarak ayarla (eğer henüz ayarlanmamışsa)
                                if (string.IsNullOrEmpty(personel.Image))
                                {
                                    personel.Image = newFileName;
                                }
                            }
                        }
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

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            ViewBag.PersonelMedyalar = personelMedyalar;

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
            // Personel bilgilerini getir
            var personel = await _context.Personeller.FindAsync(id);

            if (personel == null)
            {
                return NotFound();
            }

            // Personelin TÜM medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            foreach (var personelMedya in personelMedyalar)
            {
                // MedyaKutuphanesi bilgilerini getir
                var medyaKutuphanesi = await _context.MedyaKutuphaneleri
                    .FirstOrDefaultAsync(mk => mk.PersonelMedyaId == personelMedya.Id);

                if (medyaKutuphanesi != null)
                {
                    // MedyaKutuphanesi kaydını sil
                    _context.MedyaKutuphaneleri.Remove(medyaKutuphanesi);
                }

                // Dosya sisteminden resmi sil
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", personelMedya.Name);
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        // Dosya silinirken hata oluşursa devam et
                    }
                }

                // PersonelMedya kaydını sil
                _context.PersonelMedyalari.Remove(personelMedya);
            }
            
            // Tüm medya kayıtlarını sildikten sonra değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // Personel kaydını sil
            _context.Personeller.Remove(personel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        // POST: Personel/DeleteImage
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                // PersonelMedya bilgilerini getir
                var personelMedya = await _context.PersonelMedyalari.FindAsync(id);
                
                if (personelMedya == null)
                {
                    return Json(new { success = false, message = "Resim bulunamadı." });
                }
                
                // Personel bilgilerini getir
                var personel = await _context.Personeller.FindAsync(personelMedya.PersonelId);
                
                // MedyaKutuphanesi bilgilerini getir
                var medyaKutuphanesi = await _context.MedyaKutuphaneleri
                    .FirstOrDefaultAsync(mk => mk.PersonelMedyaId == personelMedya.Id);
                
                if (medyaKutuphanesi != null)
                {
                    // MedyaKutuphanesi kaydını sil
                    _context.MedyaKutuphaneleri.Remove(medyaKutuphanesi);
                }
                
                // Eğer silinen resim, personelin ana resmi ise başka bir resmi ana resim yap
                if (personel != null && personel.Image == personelMedya.Name)
                {
                    // Başka bir resim varsa onu ana resim yap, yoksa null yap
                    var otherMedya = await _context.PersonelMedyalari
                        .Where(pm => pm.PersonelId == personel.Id && pm.Id != personelMedya.Id)
                        .FirstOrDefaultAsync();
                    
                    personel.Image = otherMedya?.Name;
                    _context.Update(personel);
                }
                
                // Dosya sisteminden resmi sil
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", personelMedya.Name);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                
                // PersonelMedya kaydını sil
                _context.PersonelMedyalari.Remove(personelMedya);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
