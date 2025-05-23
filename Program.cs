﻿using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Repositories;
using migrapp_api.Services.Admin;
using migrapp_api.Seeding;
using FluentValidation;
using migrapp_api.Validators.Admin;
using migrapp_api.Validators.Admin;
using Microsoft.AspNetCore.Identity;
using migrapp_api.Models;
using migrapp_api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using migrapp_api.Helpers.Auth;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using migrapp_api.Services.HelpCenter;
using Microsoft.AspNetCore.Authentication.Cookies;
using migrapp_api.Services.User;
using migrapp_api.Hubs;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;// Add services to the container.


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
});

builder.Services.Configure<UploadSettings>(
    builder.Configuration.GetSection("UploadSettings"));

Console.WriteLine("Iniciando la app...");

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserDocumentService, UserDocumentService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IColumnVisibilityRepository, ColumnVisibilityRepository>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAssignedUserRepository, AssignedUserRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IMfaCodeRepository, MfaCodeRepository>();
builder.Services.AddSingleton<IEmailHelper, EmailHelper>();
builder.Services.AddSingleton<IDeviceHelper, DeviceHelper>();
builder.Services.AddSingleton<ISmsHelper, SmsHelper>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IColumnVisibilityService, ColumnVisibilityService>();
builder.Services.AddScoped<IUserLogRepository, UserLogRepository>();
builder.Services.AddScoped<ILegalProcessRepository, LegalProcessRepository>();

builder.Services.AddScoped<IProcedureDocumentService, ProcedureDocumentService>();
builder.Services.AddScoped<IHelpCenterService, HelpCenterService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILegalProcessService, LegalProcessService>();
builder.Services.AddScoped<IProcedureService, ProcedureService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAdminProcedureDocumentService, AdminProcedureDocumentService>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserByAdminDtoValidator>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Migrapp API", Version = "v1" });

    // 🔐 Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese: Bearer {su_token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();


var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!.Split(",");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200") 
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); 
    });
});

var app = builder.Build();

var uploadOpts = app.Services.GetRequiredService<IOptions<UploadSettings>>().Value;
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, uploadOpts.StoragePath));

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbSeeder.Seed(context);
    var ClientSeeder = new DataSeeder(context);
    ClientSeeder.Seed();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, uploadOpts.StoragePath)
    ),
    RequestPath = "/Uploads"
});

app.UseRouting();
app.UseCors("AllowFrontend"); // ← antes de auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.UseOutputCache();

app.Run();
