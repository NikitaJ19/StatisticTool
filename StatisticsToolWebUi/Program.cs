using StatisticsToolDbDevOpsLibrary.Data.Data;
using StatisticsToolDbDevOpsLibrary.DbServices;
using StatisticsToolDbDevOpsLibrary.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Db library services
builder.Services.AddDbContext<StatsDbContext>();
builder.Services.AddScoped<IWorkItemDbService, WorkItemDbService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WorkItemStatistics}/{action=Analytics}/{id?}");

app.Run();
