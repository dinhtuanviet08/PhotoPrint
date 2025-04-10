using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhotoPrintAPI.Models;
using PhotoPrintAPI.Services;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Text;


// Alias để tránh trùng tên
using MongoSettings = PhotoPrintAPI.Settings.MongoDbSettings;
using OrderDbSettings = PhotoPrintAPI.Settings.OrderStoreDatabaseSettings;

var builder = WebApplication.CreateBuilder(args);

// ---------- Load MongoDB Settings ----------
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.Configure<OrderDbSettings>(
    builder.Configuration.GetSection("OrderStoreDatabase"));

// ---------- Đăng ký MongoClient ----------
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

    if (string.IsNullOrEmpty(settings.ConnectionString))
    {
        throw new Exception("MongoDB ConnectionString is missing in appsettings.json!");
    }

    return new MongoClient(settings.ConnectionString);
});

// ---------- Đăng ký Services ----------
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<OrderService>();

// ---------- JWT Authentication ----------
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key missing in configuration");
}

if (Encoding.UTF8.GetBytes(jwtKey).Length < 32)
{
    throw new ArgumentOutOfRangeException("Jwt:Key", "JWT Key must be at least 256 bits (32 bytes) long");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Nhập token (định dạng: Bearer {token})",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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

// ---------- Khởi tạo app ----------
var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
