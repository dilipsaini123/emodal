using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;
using PaymentAPI.Data;
using PaymentAPI.Services;
using PaymentAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(options =>
{
    options.AddConsole();  
    options.AddDebug();    
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PaymentService>();
builder.Services.AddSingleton<CosmosDbService>();
builder.Services.AddSingleton(new ServiceBusPublisher(
    builder.Configuration["AzureServiceBus:ConnectionString"],
    builder.Configuration["AzureServiceBus:QueueName"]
));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://your-auth-provider";
//         options.Audience = "PaymentAPI";
//     });
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
