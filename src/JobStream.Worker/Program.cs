using JobStream.Application.Interfaces;
using JobStream.Infrastructure.Persistence;
using JobStream.Infrastructure.Repositories;
using JobStream.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJobRepository, JobRepository>();

Console.WriteLine("Worker DB: " + builder.Configuration.GetConnectionString("DefaultConnection"));

var host = builder.Build();
host.Run();