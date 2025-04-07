using HealthSystem.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Retrieve the DB_PASSWORD from environment variables
var dbPassword = builder.Configuration["DB_PASSWORD"];

// Update the connection string with the password from the environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection").Replace("{DB_PASSWORD}", dbPassword);

// Retrieve the DB_PASSWORD from environment variables
var dbPassword = builder.Configuration["DB_PASSWORD"];

// Update the connection string with the password from the environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection").Replace("{DB_PASSWORD}", dbPassword);

// Configure DbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularApp", policy =>
	{
		policy.WithOrigins("http://localhost:4200") // The URL of your Angular app
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});


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
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
		};
	});

builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
// Enable CORS (Cross-Origin Requests)
app.UseCors("AllowAngularApp");

//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
// Enable CORS (Cross-Origin Requests)
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();