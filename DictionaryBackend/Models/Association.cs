namespace DictionaryBackend.Models
{
    public class Association
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Count { get; set; }

        public int WordId { get; set; }
        public Word Word { get; set; } = null!;
    }
}
