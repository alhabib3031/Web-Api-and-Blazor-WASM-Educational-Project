using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.EndPoints;
using TaskFlow.Services;
using TaskFlow.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Blazor",
        policy =>
        {
            policy.WithOrigins("http://localhost:5047").AllowAnyHeader().AllowAnyMethod();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // Serves the generated JSON document
    app.UseSwaggerUI(); // Serves the interactive web UI
}

app.UseHttpsRedirection();

app.UseCors("Blazor");

app.MapTaskEndpoints();

app.Run();
