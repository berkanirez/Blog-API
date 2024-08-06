using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        ApplicationContext _context;
        IdentityRole identityRole;
        User user;

        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
        _roleManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        _userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();

        _context.Database.Migrate();

        if (_roleManager.FindByNameAsync("Admin").Result == null)
        {
            identityRole = new IdentityRole("Admin");
            _roleManager.CreateAsync(identityRole).Wait();
        }
        if (_userManager.FindByNameAsync("Admin").Result == null)
        {
            user = new User();
            user.UserName = "Admin";
            _userManager.CreateAsync(user, "Admin123!").Wait();
            _userManager.AddToRoleAsync(user, "Admin").Wait();
            _context.SaveChanges();
        }

        app.Run();
    }
}

