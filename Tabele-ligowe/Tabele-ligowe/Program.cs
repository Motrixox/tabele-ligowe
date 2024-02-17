using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tabele_ligowe.Data;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	//options.UseSqlServer(connectionString));
	options.UseInMemoryDatabase("Scoreboard"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IRepositoryService<Team>, RepositoryService<Team>>();
builder.Services.AddScoped<IRepositoryService<Match>, RepositoryService<Match>>();
builder.Services.AddScoped<IRepositoryService<League>, RepositoryService<League>>();
builder.Services.AddScoped<IRepositoryService<Season>, RepositoryService<Season>>();
builder.Services.AddSingleton<ScoreBoardService, ScoreBoardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
