using BLL.Services;

using DAL.EF;
using DAL.Repositories;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookShopDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookShopDBContext")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<EmailVerificationRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();


builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<PaymentRepository>();

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AdminDashboardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();