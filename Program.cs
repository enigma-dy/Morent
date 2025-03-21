using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoRent_V2.Context;
using MoRent_V2.Models;
using MoRent_V2.Models.Mapper;
using MoRent_V2.Services;
using System.Text;
using System.Text.Json.Serialization;
using static MoRent_V2.Models.Mapper.CarMap;

var builder = WebApplication.CreateBuilder(args);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ProfileMapper), typeof(CarMap), typeof(CarProfile));

// Add ProblemDetails for global error handling
builder.Services.AddProblemDetails();

//Add DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MoRentDB"));


//Add identity
builder.Services.AddIdentity<MoRentUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


//add jsonwebtoken
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

//Cloudinary 
builder.Services.AddScoped<CloudinaryService>();

//Car DI
builder.Services.AddScoped<CarServices>();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CustomerOnly", policy => policy.RequireClaim("IsDealer", "false"))
    .AddPolicy("DealerOnly", policy => policy.RequireClaim("IsDealer", "true"))
    .AddPolicy("VerifiedDealerOnly", policy => policy.RequireClaim("IsVerifiedDealer", "true"))
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));


//Prevent Cyclic Error due to dual reference
builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();

//Configure Email Service
var emailConfig = builder.Configuration.GetSection("EmailSettings");

builder.Services.AddSingleton(new EmailService(
    emailConfig["SmtpServer"]!,
    int.Parse(emailConfig["SmtpPort"]!),
    emailConfig["SmtpUsername"]!,
    emailConfig["SmtpPassword"]!,
    bool.Parse(emailConfig["EnableSsl"]!)
));

var app = builder.Build();

//Seeding: Creating A superAdmin on database creation with all permission and previlage
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<MoRentUser>>();


    string[] roles = ["SuperAdmin", "Admin", "Dealer", "Customer"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    var superAdminEmail = "superadmin@morent.com";
    var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (superAdminUser == null)
    {
        superAdminUser = new MoRentUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            FullName = "Super Admin",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(superAdminUser, "SuperAdmin@123");
        await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
    }
}


if (app.Environment.IsDevelopment())
{

}

app.UseCors("AllowReactApp");

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();