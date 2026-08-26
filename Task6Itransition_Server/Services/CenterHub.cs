using System.Threading.Tasks;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Task6Itransition_Server.Services.Database;

namespace Task6Itransition.Services
{
    public class CenterHub(AppDbContext dbContext) : Hub
    {
        public async Task AddItem(CircuitItemDTO dto)
        {
            await Clients.Others.SendAsync("AddItem", dto);
        }

        public async Task SaveItems(List<CircuitItemDTO> items)
        {
            await dbContext.SaveAsync(items);
        }

        public async Task LoadItems()
        {
            await Clients.Caller.SendAsync("LoadItems", dbContext.Load());
        }
    }
}
