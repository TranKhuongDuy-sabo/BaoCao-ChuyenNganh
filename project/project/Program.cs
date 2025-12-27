using Microsoft.EntityFrameworkCore;
using project.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Đăng ký dịch vụ để dùng được Session
builder.Services.AddDistributedMemoryCache(); // Lưu session trong RAM
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. QUAN TRỌNG: Đăng ký HttpContextAccessor (Để sửa lỗi trong ảnh của bạn)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Kích hoạt Session (Phải đặt trước UseRouting hoặc MapControllerRoute)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas", // ??t tên cho route c?a Area
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}" // C?u trúc URL
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
