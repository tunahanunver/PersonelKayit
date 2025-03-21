using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelKayit.Models;

namespace PersonelKayit.Controllers
{
    [Authorize]
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
                .Include(p => p.Egitimler)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Include(pm => pm.MedyaKutuphanesi)
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
        public async Task<IActionResult> Create([Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama")] Personel personel, List<IFormFile?> Images, string ulke, string sehir, string ilce, IFormCollection form)
        {
            // Eğitim verilerini takip etmek için liste oluştur
            List<PersonelEgitim> Egitimler = new List<PersonelEgitim>();
            
            try
            {
                // Form verilerinin detaylı dökümü (debugging için)
                System.Diagnostics.Debug.WriteLine($"Form verileri alındı: Personel={personel.Ad} {personel.Soyad}");
                foreach (var key in form.Keys)
                {
                    System.Diagnostics.Debug.WriteLine($"Form Key: {key} = {form[key]}");
                }
                
                // Eğitim bilgilerini form verilerinden manuel olarak çıkar
                var egitimKeys = form.Keys.Where(k => k.StartsWith("Egitimler[") && k.EndsWith("].OkulAdi")).ToList();
                System.Diagnostics.Debug.WriteLine($"Eğitim form anahtarları: {egitimKeys.Count} adet bulundu");
                
                foreach (var key in egitimKeys)
                {
                    try {
                        // İndeksi çıkar - "Egitimler[0].OkulAdi" formatından "0" değerini al
                        string indexStr = key.Substring(key.IndexOf('[') + 1, key.IndexOf(']') - key.IndexOf('[') - 1);
                        int index = int.Parse(indexStr);
                        
                        string okulAdi = form[$"Egitimler[{index}].OkulAdi"].ToString();
                        string bolum = form[$"Egitimler[{index}].Bolum"].ToString();
                        bool devamEdiyor = form[$"Egitimler[{index}].DevamEdiyor"] == "true";
                        
                        if (!string.IsNullOrEmpty(okulAdi) && !string.IsNullOrEmpty(bolum))
                        {
                            var egitim = new PersonelEgitim
                            {
                                OkulAdi = okulAdi,
                                Bolum = bolum,
                                DevamEdiyor = devamEdiyor
                            };
                            
                            // Devam etmiyorsa mezuniyet tarihini işle
                            if (!devamEdiyor && form.ContainsKey($"Egitimler[{index}].MezuniyetTarihi"))
                            {
                                string tarihStr = form[$"Egitimler[{index}].MezuniyetTarihi"].ToString();
                                if (!string.IsNullOrEmpty(tarihStr))
                                {
                                    if (DateTime.TryParse(tarihStr, out DateTime mezuniyetTarihi))
                                    {
                                        egitim.MezuniyetTarihi = mezuniyetTarihi;
                                        System.Diagnostics.Debug.WriteLine($"Mezuniyet tarihi işlendi: {mezuniyetTarihi}");
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Geçersiz tarih formatı: {tarihStr}");
                                    }
                                }
                            }
                            
                            Egitimler.Add(egitim);
                            System.Diagnostics.Debug.WriteLine($"Eğitim eklendi: İndeks={index}, Okul={egitim.OkulAdi}, Bölüm={egitim.Bolum}, Devam={egitim.DevamEdiyor}, Tarih={egitim.MezuniyetTarihi}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"İndeks {index} için eğitim bilgisi atlandı: Eksik alanlar var (OkulAdi={okulAdi}, Bolum={bolum})");
                        }
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine($"Eğitim işlenirken hata oluştu: {ex.Message} - {key}");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Toplam işlenen eğitim sayısı: {Egitimler.Count}");
                
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
                    System.Diagnostics.Debug.WriteLine($"Personel kaydedildi: ID={personel.Id}");

                    // Resimleri işle
                    if (Images != null && Images.Count > 0)
                    {
                        foreach (var image in Images)
                        {
                            if (image != null && image.Length > 0)
                            {
                                string guidId = Guid.NewGuid().ToString();
                                string fileExtension = Path.GetFileNameWithoutExtension(image.FileName);
                                string newFileName = $"{guidId}{image.FileName}";
                                string klasor = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", newFileName);

                                using (var stream = new FileStream(klasor, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                // MedyaKutuphanesi oluştur
                                var medyaKutuphanesi = new MedyaKutuphanesi
                                {
                                    MedyaAdi = fileExtension,
                                    MedyaURL = newFileName
                                };

                                _context.MedyaKutuphaneleri.Add(medyaKutuphanesi);
                                await _context.SaveChangesAsync();

                                // PersonelMedya oluştur
                                var personelMedya = new PersonelMedya
                                {
                                    PersonelId = personel.Id,
                                    MedyaId = medyaKutuphanesi.Id
                                };

                                _context.PersonelMedyalari.Add(personelMedya);
                                await _context.SaveChangesAsync();

                                // Her zaman son eklenen resmi personel.Image olarak ayarla
                                personel.Image = newFileName;
                                await _context.SaveChangesAsync();  // Personel.Image değişikliğini kaydet
                            }
                        }
                    }

                    // Eğitim bilgilerini ekle
                    if (Egitimler.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Veritabanına eklenecek eğitim sayısı: {Egitimler.Count}");
                        
                        foreach (var egitim in Egitimler)
                        {
                            egitim.PersonelId = personel.Id;
                            _context.PersonelEgitimleri.Add(egitim);
                            System.Diagnostics.Debug.WriteLine($"Veritabanına eklenen eğitim: {egitim.OkulAdi}, {egitim.Bolum}");
                        }
                        
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine("Eğitim bilgileri başarıyla kaydedildi");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Eklenecek eğitim bilgisi bulunamadı! (Liste boş)");
                    }

                    System.Diagnostics.Debug.WriteLine("Tüm işlemler başarıyla tamamlandı.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // ModelState hatalarını logla
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"ModelState Hatası: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Kayıt eklenirken bir hata oluştu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Personel eklenirken hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            
            // Eğer buraya kadar geldiyse, bir hata oluşmuş demektir
            ViewData["Egitimler"] = Egitimler;
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
                .Include(p => p.Egitimler) // Eğitim bilgilerini dahil et
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Include(pm => pm.MedyaKutuphanesi)
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            ViewBag.PersonelMedyalar = personelMedyalar;

            // Lokasyon bilgilerini getir
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

        // POST: Personel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,DogumTarihi,Cinsiyet,Aciklama,Image")] Personel personel, List<IFormFile> Images, string ulke, string sehir, string ilce, IFormCollection form)
        {
            if (id != personel.Id)
            {
                return NotFound();
            }

            // Eğitim verilerini form'dan manuel olarak çıkar
            List<PersonelEgitim> Egitimler = new List<PersonelEgitim>();
            
            try
            {
                // Form verilerinin detaylı dökümü (debugging için)
                System.Diagnostics.Debug.WriteLine($"Form verileri alındı: Personel={personel.Ad} {personel.Soyad}");
                foreach (var key in form.Keys)
                {
                    System.Diagnostics.Debug.WriteLine($"Form Key: {key} = {form[key]}");
                }
                
                // Eğitim bilgilerini form verilerinden manuel olarak çıkar
                var egitimKeys = form.Keys.Where(k => k.StartsWith("Egitimler[") && k.EndsWith("].OkulAdi")).ToList();
                System.Diagnostics.Debug.WriteLine($"Eğitim form anahtarları: {egitimKeys.Count} adet bulundu");
                
                foreach (var key in egitimKeys)
                {
                    try {
                        // İndeksi çıkar - "Egitimler[0].OkulAdi" formatından "0" değerini al
                        string indexStr = key.Substring(key.IndexOf('[') + 1, key.IndexOf(']') - key.IndexOf('[') - 1);
                        int index = int.Parse(indexStr);
                        
                        string okulAdi = form[$"Egitimler[{index}].OkulAdi"].ToString();
                        string bolum = form[$"Egitimler[{index}].Bolum"].ToString();
                        bool devamEdiyor = form[$"Egitimler[{index}].DevamEdiyor"] == "true";
                        
                        if (!string.IsNullOrEmpty(okulAdi) && !string.IsNullOrEmpty(bolum))
                        {
                            var egitim = new PersonelEgitim
                            {
                                OkulAdi = okulAdi,
                                Bolum = bolum,
                                DevamEdiyor = devamEdiyor
                            };
                            
                            // Devam etmiyorsa mezuniyet tarihini işle
                            if (!devamEdiyor && form.ContainsKey($"Egitimler[{index}].MezuniyetTarihi"))
                            {
                                string tarihStr = form[$"Egitimler[{index}].MezuniyetTarihi"].ToString();
                                if (!string.IsNullOrEmpty(tarihStr))
                                {
                                    if (DateTime.TryParse(tarihStr, out DateTime mezuniyetTarihi))
                                    {
                                        egitim.MezuniyetTarihi = mezuniyetTarihi;
                                        System.Diagnostics.Debug.WriteLine($"Mezuniyet tarihi işlendi: {mezuniyetTarihi}");
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Geçersiz tarih formatı: {tarihStr}");
                                    }
                                }
                            }
                            
                            Egitimler.Add(egitim);
                            System.Diagnostics.Debug.WriteLine($"Eğitim eklendi: İndeks={index}, Okul={egitim.OkulAdi}, Bölüm={egitim.Bolum}, Devam={egitim.DevamEdiyor}, Tarih={egitim.MezuniyetTarihi}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"İndeks {index} için eğitim bilgisi atlandı: Eksik alanlar var (OkulAdi={okulAdi}, Bolum={bolum})");
                        }
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine($"Eğitim işlenirken hata oluştu: {ex.Message} - {key}");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Toplam işlenen eğitim sayısı: {Egitimler.Count}");

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

                    // Mevcut personeli veritabanından al (AsNoTracking ile)
                    var existingPersonel = await _context.Personeller.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPersonel == null)
                    {
                        return NotFound();
                    }

                    // Personeli güncelle
                    personel.LokasyonId = ilceLokasyon.Id;
                    // Eğer resim yüklenmediyse eski resim adını koru
                    if (string.IsNullOrEmpty(personel.Image) && !string.IsNullOrEmpty(existingPersonel.Image))
                    {
                        personel.Image = existingPersonel.Image;
                    }

                    // Resimleri işle
                    if (Images != null && Images.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Yeni yüklenen resim sayısı: {Images.Count}");
                        
                        foreach (var image in Images)
                        {
                            if (image != null && image.Length > 0)
                            {
                                string guidId = Guid.NewGuid().ToString();
                                string fileExtension = Path.GetFileNameWithoutExtension(image.FileName);
                                string newFileName = $"{guidId}{image.FileName}";
                                string klasor = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", newFileName);

                                using (var stream = new FileStream(klasor, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                // MedyaKutuphanesi oluştur
                                var medyaKutuphanesi = new MedyaKutuphanesi
                                {
                                    MedyaAdi = fileExtension,
                                    MedyaURL = newFileName
                                };

                                _context.MedyaKutuphaneleri.Add(medyaKutuphanesi);
                                await _context.SaveChangesAsync();

                                // PersonelMedya oluştur
                                var personelMedya = new PersonelMedya
                                {
                                    PersonelId = personel.Id,
                                    MedyaId = medyaKutuphanesi.Id
                                };

                                _context.PersonelMedyalari.Add(personelMedya);
                                await _context.SaveChangesAsync();

                                // Her zaman son eklenen resmi personel.Image olarak ayarla
                                personel.Image = newFileName;
                                System.Diagnostics.Debug.WriteLine($"Yeni resim eklendi: {newFileName}");
                            }
                        }
                    }

                    try
                    {
                        // Personeli güncelle
                        _context.Update(personel);
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"Personel güncellendi: ID={personel.Id}");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!PersonelExists(personel.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Personel güncellenirken veri tutarsızlığı: {ex.Message}");
                            throw;
                        }
                    }

                    // Eğitim bilgilerini güncelle
                    if (Egitimler.Count > 0)
                    {
                        // Önce mevcut eğitim bilgilerini sil
                        var mevcutEgitimler = await _context.PersonelEgitimleri
                            .Where(e => e.PersonelId == id)
                            .ToListAsync();

                        _context.PersonelEgitimleri.RemoveRange(mevcutEgitimler);
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"Mevcut {mevcutEgitimler.Count} eğitim kaydı silindi");

                        // Sonra yeni eğitim bilgilerini ekle
                        foreach (var egitim in Egitimler)
                        {
                            egitim.PersonelId = personel.Id;
                            _context.PersonelEgitimleri.Add(egitim);
                            System.Diagnostics.Debug.WriteLine($"Eğitim eklendi: {egitim.OkulAdi}, {egitim.Bolum}");
                        }
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"{Egitimler.Count} adet eğitim bilgisi eklendi");
                    }
                    else
                    {
                        // Eğitim bilgisi yoksa varolan eğitimleri sil
                        var mevcutEgitimler = await _context.PersonelEgitimleri
                            .Where(e => e.PersonelId == id)
                            .ToListAsync();

                        if (mevcutEgitimler.Any())
                        {
                            _context.PersonelEgitimleri.RemoveRange(mevcutEgitimler);
                            await _context.SaveChangesAsync();
                            System.Diagnostics.Debug.WriteLine($"Tüm eğitim bilgileri silindi");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Tüm güncelleme işlemleri başarıyla tamamlandı");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // ModelState hatalarını logla
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"ModelState Hatası: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Kayıt güncellenirken bir hata oluştu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Personel güncellenirken hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            // Hata durumunda medya bilgilerini hazırla
            var personelMedyalar = await _context.PersonelMedyalari
                .Include(pm => pm.MedyaKutuphanesi)
                .Where(pm => pm.PersonelId == id)
                .ToListAsync();

            ViewBag.PersonelMedyalar = personelMedyalar;
            
            // Lokasyon bilgilerini hazırla
            ViewBag.SecilenUlke = ulke;
            ViewBag.SecilenSehir = sehir;
            ViewBag.SecilenIlce = ilce;
            
            // Form verilerini view'a geri döndür
            ViewData["Egitimler"] = Egitimler;
            
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
                .Include(p => p.Egitimler)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            // Personelin medya bilgilerini getir
            var personelMedyalar = await _context.PersonelMedyalari
                .Include(pm => pm.MedyaKutuphanesi)
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
            try
            {
                System.Diagnostics.Debug.WriteLine($"Personel silme başlatıldı: ID={id}");
                
                // Personel bilgilerini getir
                var personel = await _context.Personeller.FindAsync(id);

                if (personel == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Silinecek personel bulunamadı: ID={id}");
                    return NotFound();
                }

                // Personelin eğitim bilgilerini getir ve sil
                var personelEgitimler = await _context.PersonelEgitimleri
                    .Where(pe => pe.PersonelId == id)
                    .ToListAsync();
                
                if (personelEgitimler.Any())
                {
                    _context.PersonelEgitimleri.RemoveRange(personelEgitimler);
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"{personelEgitimler.Count} adet eğitim bilgisi silindi.");
                }

                // Personelin TÜM medya bilgilerini getir
                var personelMedyalar = await _context.PersonelMedyalari
                    .Where(pm => pm.PersonelId == id)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"{personelMedyalar.Count} adet medya bilgisi bulundu.");

                foreach (var personelMedya in personelMedyalar)
                {
                    try
                    {
                        // MedyaKutuphanesi bilgilerini getir
                        var medya = await _context.MedyaKutuphaneleri.FindAsync(personelMedya.MedyaId);
                        if (medya != null)
                        {
                            // Dosya sisteminden resmi sil
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", medya.MedyaURL);
                            if (System.IO.File.Exists(filePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(filePath);
                                    System.Diagnostics.Debug.WriteLine($"Dosya silindi: {filePath}");
                                }
                                catch (Exception ex)
                                {
                                    // Dosya silinirken hata oluşursa devam et
                                    System.Diagnostics.Debug.WriteLine($"Dosya silinemedi: {filePath}, Hata: {ex.Message}");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Dosya bulunamadı: {filePath}");
                            }

                            // MedyaKutuphanesi kaydını sil
                            _context.MedyaKutuphaneleri.Remove(medya);
                            System.Diagnostics.Debug.WriteLine($"MedyaKutuphanesi kaydı silindi: ID={medya.Id}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"MedyaKutuphanesi bulunamadı: ID={personelMedya.MedyaId}");
                        }

                        // PersonelMedya kaydını sil
                        _context.PersonelMedyalari.Remove(personelMedya);
                        System.Diagnostics.Debug.WriteLine($"PersonelMedya kaydı silindi: ID={personelMedya.Id}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Medya silinirken hata: {ex.Message}");
                        // Hata oluşsa bile diğer medyaları silmeye devam et
                    }
                }

                // Tüm medya kayıtlarını sildikten sonra değişiklikleri kaydet
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine("Tüm medya kayıtları silindi.");

                // Personel kaydını sil
                _context.Personeller.Remove(personel);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Personel başarıyla silindi: ID={id}");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Personel silinirken hata oluştu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                // Hata durumunda ana sayfaya yönlendir ve hata mesajı göster
                TempData["ErrorMessage"] = $"Personel silinirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Personel/DeleteImage
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                // PersonelMedya bilgilerini getir
                var personelMedya = await _context.PersonelMedyalari
                    .Include(pm => pm.MedyaKutuphanesi)
                    .FirstOrDefaultAsync(pm => pm.Id == id);

                if (personelMedya == null)
                {
                    return Json(new { success = false, message = "Resim bulunamadı." });
                }

                // Personel bilgilerini getir
                var personel = await _context.Personeller.FindAsync(personelMedya.PersonelId);

                // MedyaKutuphanesi bilgilerini getir
                var medyaKutuphanesi = personelMedya.MedyaKutuphanesi;

                if (medyaKutuphanesi != null)
                {
                    // Dosya sisteminden resmi sil
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MedyaKutuphanesi", medyaKutuphanesi.MedyaURL);
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

                    // MedyaKutuphanesi kaydını sil
                    _context.MedyaKutuphaneleri.Remove(medyaKutuphanesi);
                }

                // Eğer silinen resim, personelin ana resmi ise başka bir resmi ana resim yap
                if (personel != null && personel.Image == medyaKutuphanesi?.MedyaURL)
                {
                    // Başka bir resim varsa onu ana resim yap, yoksa null yap
                    var otherMedya = await _context.PersonelMedyalari
                        .Include(pm => pm.MedyaKutuphanesi)
                        .Where(pm => pm.PersonelId == personel.Id && pm.Id != personelMedya.Id)
                        .FirstOrDefaultAsync();

                    personel.Image = otherMedya?.MedyaKutuphanesi?.MedyaURL;
                    _context.Update(personel);
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

        private bool PersonelExists(int id)
        {
            return _context.Personeller.Any(e => e.Id == id);
        }
    }
}
