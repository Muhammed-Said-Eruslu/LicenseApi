using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.Controllers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Infrastructure (DbContext + Services)
builder.Services.AddInfrastructure(builder.Configuration);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers()
    .AddApplicationPart(typeof(LicenseController).Assembly);

// Swagger + JWT Auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "License API",
        Version = "v1",
        Description = "Lisans oluşturma ve doğrulama API'si"
    });

    // Swagger için Bearer JWT Auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Örnek: 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = new[] { "USER", "ADMIN" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // İlk admin hesabı: AdminSeed:Email ve AdminSeed:Password ortam değişkeni / user-secrets ile verilir.
    // Kod içinde sabit parola tutulmaz; ayar yoksa admin oluşturulmaz.
    var adminEmail = builder.Configuration["AdminSeed:Email"];
    var adminPassword = builder.Configuration["AdminSeed:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser { UserName = adminEmail, Email = adminEmail };
            var created = await userManager.CreateAsync(admin, adminPassword);
            if (!created.Succeeded)
                throw new InvalidOperationException("Admin oluşturulamadı: " + string.Join(", ", created.Errors.Select(e => e.Description)));
        }
        if (!await userManager.IsInRoleAsync(admin, "ADMIN"))
            await userManager.AddToRoleAsync(admin, "ADMIN");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


/*
 "Server=localhost;Database=LisansDb3;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
 
 */