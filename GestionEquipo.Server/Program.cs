using GestionEquipo.DB.DATA;
using GestionEquipo.Server.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

//---------------------------------------------------------------------------------------------------------------
//CONFIGURACION DE LOS SERVICIOS EN EL CONSTRUCTOR DE LA APLICACION
var builder = WebApplication.CreateBuilder(args);


//---INCORPORAR CACHE EN UNA PETICIÓN HTTP---
builder.Services.AddOutputCache(opciones =>

{

    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(10);

});

builder.Services.AddControllers();
builder.Services.AddControllersWithViews().AddJsonOptions(
  x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(op =>op.UseSqlServer("name=conn"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["jwtkey"]))
        };
    });

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IJugadorRepositorio, JugadorRepositorio>();

//---------------------------------------------------------------------------------------------------------------
// CONSTRUCCION DE LA APLICACION
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache(); //--CACHE--//
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
