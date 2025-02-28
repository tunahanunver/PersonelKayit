namespace PersonelKayit.Models
{
    public class Lokasyon
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<Personel>? Personels { get; set; }
    }
}
