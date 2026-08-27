using Domain.DTOs;

namespace Task6Itransition_Server.Services.Database
{
    public class Map
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public List<CircuitItemDTO> AllItems { get; set; } = new();
    }
}
