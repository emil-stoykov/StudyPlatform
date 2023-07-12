using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyPlatform.Data;
using StudyPlatform.Data.Models;
using StudyPlatform.Services.Category;
using StudyPlatform.Services.Category.Interfaces;
using StudyPlatform.Services.Course;
using StudyPlatform.Services.Course.Interfaces;
using StudyPlatform.Services.Users;
using StudyPlatform.Services.Users.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudyPlatformDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        db => db.MigrationsAssembly("StudyPlatform.Data")) // or just "StudyPlatform.Data"
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount 
        = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
    options.Password.RequireDigit
        = builder.Configuration.GetValue<bool>("Identity:Password:RequireDigit");
    options.Password.RequireLowercase
        = builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
    options.Password.RequireUppercase
        = builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
    options.Password.RequiredLength
        = builder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<StudyPlatformDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICategoryViewService, CategoryViewService>();
builder.Services.AddScoped<ICourseViewService, CourseViewService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("Home/Error/500");
    app.UseStatusCodePagesWithRedirects("Home/Error?statusCode={0}");
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();