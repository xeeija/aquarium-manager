using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Services;
using Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<GlobalService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument(doc =>
{
  doc.Title = "Aquarium Management API";
  // doc.Description = "";
  doc.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme()
  {
    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
    Name = "Authorization",
    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
    Description = "Enter token below: Bearer ey1234567890abcdef...",
  });
});

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => options.TokenValidationParameters = Authentication.ValidationParams);

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
app.UseOpenApi();
app.UseSwaggerUi3();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
