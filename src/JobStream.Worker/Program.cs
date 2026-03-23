using JobStream.Application.Interfaces;
using JobStream.Infrastructure.Persistence;
using JobStream.Infrastructure.Repositories;
using JobStream.Infrastructure.Services;
using JobStream.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

Console.WriteLine("Worker DB: " + builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

host.Run();