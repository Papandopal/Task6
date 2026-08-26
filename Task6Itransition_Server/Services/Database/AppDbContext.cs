using System.Threading.Tasks;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Task6Itransition_Server.Services.Database
{
    public class AppDbContext : DbContext
    {
        private DbSet<CircuitItemDTO> items;

        public AppDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            items = Set<CircuitItemDTO>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CircuitItemDTO>().HasKey(x => x.Id);
            modelBuilder.Entity<CircuitItemDTO>().Property(x=>x.Id)
                .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
        }

        public async Task SaveAsync(List<CircuitItemDTO> items)
        {
            List<CircuitItemDTO> newItems = new();
            List<CircuitItemDTO> updatedItems = new();
            foreach(var item in items)
            {
                if(items.Contains(item)) updatedItems.Add(item);
                else newItems.Add(item);
            }
            UpdateRange(updatedItems);
            AddRange(newItems);
            await SaveChangesAsync();
        }

        public List<CircuitItemDTO> Load()
        {
            return items.ToList();
        }
    }
}
