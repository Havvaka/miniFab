using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using minifab.api.Templates.Data;
using minifab.api.Templates.Hubs;
using minifab.api.Templates.Service.Device;
using minifab.api.Templates.Service.MessageBroker;
using minifab.api.Templates.Service.SensorData;
using System;


var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı ve DbContext servisi ekleniyor
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servisler ekleniyor - Modüler yapı için service katmanını ekliyoruz
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();



// CORS politikası ekleniyor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder.WithOrigins("http://localhost:8080", "http://localhost:3000") // Vue.js dev server'ları
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()); // SignalR için gerekli
});

// Swagger ekleniyor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTPS yönlendirmesi
app.UseHttpsRedirection();

// CORS politikası uygulanıyor
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SensorHub>("/sensorHub"); // SignalR Hub endpoint'i
});
// Controller rotalarını eşleştir
app.MapControllers();

// ConsumerService'i doğrudan ana service provider'dan al, scope'a ihtiyacı yok
var consumerService = app.Services.GetRequiredService<IConsumerService>();
consumerService.StartListening();
Console.WriteLine("✅ RabbitMQ consumer başlatıldı.");

// Veritabanı bağlantısını kontrol et ve migrate et
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("✅ Veritabanı bağlantısı başarılı.");

    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Veritabanı bağlantısı başarısız:");
        Console.WriteLine(ex.Message);
    }
}

app.Run();

