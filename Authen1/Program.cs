// dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.EntityFrameworkCore.Design


using Authen1.Data;
using Authen1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// default Swagger
//builder.Services.AddSwaggerGen();

// Swagger UI with authorization
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Authen1", Version = "v1" });
	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});

	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});
});

// Add Authentication
builder.Services.AddAuthentication()
	.AddBearerToken(IdentityConstants.BearerScheme);

// Add Authorization
builder.Services.AddAuthenticationCore();

// Configure DbContext
builder.Services.
	AddDbContext<AppDbContext>(opt => opt.UseSqlite("DataSource=appdata.db"));

builder.Services.AddIdentityCore<AppUser>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<AppUser>();


/*
 * 	 open Package Manager Console in VS2022 to enter these commands:
		Install - Package Microsoft.EntityFrameworkCore.Tools
		Add-Migration InitialCreate
		Update-Database
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
