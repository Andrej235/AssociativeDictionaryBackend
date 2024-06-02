namespace DictionaryBackend.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public IEnumerable<Association> Associations { get; set; } = new List<Association>();
    }
}
