using DictionaryBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DictionaryBackend.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Word> Words { get; set; } = null!;
        public DbSet<Association> Associations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AssociativeDictionary;Integrated Security=True;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Word>()
                .HasMany(x => x.Associations)
                .WithOne(x => x.Word)
                .HasForeignKey(x => x.WordId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
