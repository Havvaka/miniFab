using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniFab.Api.Data;
using MiniFab.Api.Hubs;
using MiniFab.Api.Services.Device;
using MiniFab.Api.Services.MessageBroker;
using MiniFab.Api.Services.SensorData;
using System;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder.WithOrigins("http://localhost:8080", "http://localhost:3000", "http://localhost") 
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});



var app = builder.Build();

// app.UseHttpsRedirection(); // Docker içinde HTTP kullanıyoruz

app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapHub<SensorHub>("/sensorHub");

var consumerService = app.Services.GetRequiredService<IConsumerService>();
consumerService.StartListening();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("Veritabanı bağlandı");

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

app.Run();

