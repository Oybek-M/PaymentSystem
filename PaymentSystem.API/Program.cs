using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentSystem.API.Middleware;
using PaymentSystem.BLL.Services;
using PaymentSystem.BLL.Validators;
using PaymentSystem.DAL.Data;
using PaymentSystem.DAL.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with XML comments
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment System API",
        Version = "v1.0",
        Description = @"
# To'lov Tizimi API

O'zbekiston uchun to'lov tizimi - Ro'yxatdan o'tish va to'lovlarni qayd etish.

## Asosiy Xususiyatlar
- ✅ Foydalanuvchini ro'yxatdan o'tkazish
- ✅ To'lovlarni qayd etish (Check fayli bilan)
- ✅ Pagination bilan ro'yxatlarni olish
- ✅ O'zbekcha xato xabarlari va LOGlar
- ✅ Telefon raqam validatsiyasi

## Telefon Format
**Qabul qilinadigan format:** `+998 XX XXX XX XX`

**Misollar:**
- ✅ +998 90 123 45 67
- ✅ +998 91 234 56 78
- ✅ +998901234567 (bo'sh joysiz)

## File Upload
**Qabul qilinadigan formatlar:** PDF, PNG, JPG, JPEG
**Maksimum hajm:** 5MB

## Pagination
**Default sahifa hajmi:** 10
**Maksimum sahifa hajmi:** 100

---
**Versiya:** 1.0
**Sana:** 16-02-2026
",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "muxtaraliyevoybek@gmail.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();

// Register PaymentService with upload path
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
builder.Services.AddScoped<IPaymentService>(provider =>
{
    var paymentRepo = provider.GetRequiredService<IPaymentRepository>();
    var userRepo = provider.GetRequiredService<IUserRepository>();
    return new PaymentService(paymentRepo, userRepo, uploadPath);
});

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<SignUpDtoValidator>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment System API v1");
        c.DocumentTitle = "Payment System API - Swagger UI";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    });
}

// Use Global Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Serve static files (uploaded files)
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
