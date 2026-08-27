using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace Task6Itransition_Server.Services.Database
{
    public class AppDbContext : DbContext
    {
        private DbSet<Map> maps;

        public AppDbContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            maps = Set<Map>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Map>().HasKey(x => x.Id);

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            modelBuilder.Entity<Map>()
                .Property(x => x.AllItems)
                //.HasColumnType("json")
                .HasConversion(
                    x => JsonSerializer.Serialize(x, jsonOptions),
                    x => JsonSerializer.Deserialize<List<CircuitItemDTO>>(x, jsonOptions) ?? new List<CircuitItemDTO>()
                );
        }

        public async Task RewriteMapAsync(List<CircuitItemDTO> items, string mapName)
        {
            var map = maps.FirstOrDefault(x => x.Name == mapName);
            if (map is null) throw new Exception("Map not found");

            map.AllItems = items;   

            await SaveChangesAsync();
        }

        public async Task AddItems(List<CircuitItemDTO> items, string mapName)
        {
            var map = maps.FirstOrDefault(x => x.Name == mapName);
            if (map is null) throw new Exception("Map not found");

            map.AllItems.AddRange(items);
            Entry(map).Property(x => x.AllItems).IsModified = true;
            await SaveChangesAsync();
        }

        public async Task<List<CircuitItemDTO>> LoadAsync(string mapName)
        {
            var map = maps.FirstOrDefault(x=>x.Name == mapName);
            if (map is null)
            {
                maps.Add(new Map { Id = Guid.NewGuid(), Name = mapName, AllItems = new List<CircuitItemDTO>() });
                await SaveChangesAsync();
                return new List<CircuitItemDTO>();
            }
            else
            {
                return map.AllItems.ToList();
            }
        }
    }
}
