using Microsoft.AspNetCore.SignalR;
using minifab.api.Templates.Models;
using System.Threading.Tasks;

namespace minifab.api.Templates.Hubs
{
    public class SensorHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"İstemci bağlandı: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"İstemci bağlantısı kesildi: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // Belirli bir cihazın verilerini izlemek için gruba katılma
        public async Task JoinDeviceGroup(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            Console.WriteLine($"İstemci {Context.ConnectionId}, cihaz grubuna katıldı: {deviceId}");
        }

        // Cihaz grubundan ayrılma
        public async Task LeaveDeviceGroup(string deviceId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
            Console.WriteLine($"İstemci {Context.ConnectionId}, cihaz grubundan ayrıldı: {deviceId}");
        }
    }
}