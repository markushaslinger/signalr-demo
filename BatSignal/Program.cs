using BatSignal;
using BatSignal.Contract;
using Microsoft.AspNetCore.Mvc;

const string CorsPolicy = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, b =>
    {
        b.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);

// just a sample endpoint to check the service is up, not required
app.MapGet("/hello", ([FromQuery] string? name) => $"Hello {name ?? "Bergmann"}!")
   .WithName("Greet")
   .WithOpenApi();

app.MapHub<ChatHub>(ChatHubConfig.Route);

app.Run();
