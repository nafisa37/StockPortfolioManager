using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);

// try{
//     var root = Directory.GetCurrentDirectory();
//     var dotenv = Path.Combine(root, ".env");
//     DotEnv.Load(dotenv);
// }
// catch (Exception ex){
//     Console.WriteLine($"error with .env file: {ex.Message}");
// }

// var config = new ConfigurationBuilder()
//     .AddEnvironmentVariables()
//     .Build();

// string apiKey = config["API_KEY"];
Env.Load();
// string apiKey = Environment.GetEnvironmentVariable("API_Key");
var connectionString = Environment.GetEnvironmentVariable("MSSQL_TCP_URL");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

//builder.Services.AddSingleton<StockService>(); //for API calls
builder.Services.AddHttpClient();
builder.Services.AddScoped<APIHelper>();  // Register StockApiService

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Ensure logs appear in the terminal

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
