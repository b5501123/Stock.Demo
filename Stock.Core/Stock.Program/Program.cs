using Microsoft.EntityFrameworkCore;
using Stock.Common.Extension;
using Stock.DB;
using Stock.Model.Dao;
using Stock.Program.Extensions;
using Stock.Repository;
using Stock.Service.Job;
using Stock.Service.Schedule;
using Stock.Service.Scrubbing;
using System.Diagnostics;

Console.WriteLine(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStockServices(builder.Configuration);
Console.WriteLine("app work");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var job = new StcokJob();
job.Start();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
