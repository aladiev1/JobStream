using JobStream.Application.Interfaces;
using JobStream.Infrastructure.Persistence;
using JobStream.Infrastructure.Repositories;
using JobStream.Infrastructure.Services;
using JobStream.Infrastructure.Options;
using JobStream.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

Console.WriteLine("Worker DB: " + builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();
builder.Services.Configure<WeatherstackOptions>(
    builder.Configuration.GetSection(WeatherstackOptions.SectionName));
builder.Services.AddHttpClient<IWeatherClient, WeatherstackClient>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

host.Run();