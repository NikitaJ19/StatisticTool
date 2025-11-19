using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatisticsToolDbDevOpsLibrary.Data.Data;
using StatisticsToolDbDevOpsLibrary.DbServices;
using StatisticsToolDbDevOpsLibrary.Interfaces;
using StatisticsToolTimerConsole.BusinessServices;
using StatisticsToolTimerConsole.Interfaces;
using System;
using System.Threading.Tasks;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Add services to the container
//Db library services
builder.Services.AddDbContext<StatsDbContext>();
builder.Services.AddScoped<IWorkItemDbService, WorkItemDbService>();
//Timer console services
builder.Services.AddScoped<IAzureAuthService, AzureAuthService>();
builder.Services.AddScoped<IWorkItemAzureService, WorkItemAzureService>();

using var host = builder.Build();

var workItemService = host.Services.GetRequiredService<IWorkItemAzureService>();
//var dbService = host.Services.GetRequiredService<IWorkItemDbService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    var timer = new System.Timers.Timer(5 * 60 * 1000);
    timer.Elapsed += async (s, e) => await workItemService.AddWorkItem();
    timer.AutoReset = true;
    timer.Enabled = true;

    // Run once immediately
    await workItemService.AddWorkItem();
 

    Console.WriteLine("Running... Press ENTER to exit");
    Console.ReadLine();
    timer.Stop();
    timer.Dispose();
}
catch (Exception ex)
{
    logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
}