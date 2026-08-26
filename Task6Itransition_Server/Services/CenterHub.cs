using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Task6Itransition.Services
{
    public class CenterHub : Hub
    {
        public async Task AddItem(CircuitItemDTO dto)
        {
            await Clients.Others.SendAsync("AddItem", dto);
        }
    }
}
