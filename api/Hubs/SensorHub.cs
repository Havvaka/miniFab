using Microsoft.AspNetCore.SignalR;
using MiniFab.Api.Models;
using System.Threading.Tasks;

namespace MiniFab.Api.Hubs
{
    public class SensorHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("bağlantı kuruldu");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("bağlantı kesildi");
            await base.OnDisconnectedAsync(exception);
        }


        public async Task JoinDeviceGroup(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
        }


        public async Task LeaveDeviceGroup(string deviceId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
        }
    }
}