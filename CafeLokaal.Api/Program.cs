using CafeLokaal.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


// Configure authentication with Microsoft Identity (Entra ID/B2C)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("MSENTRA"));

// Add authorization
builder.Services.AddAuthorization();

// Add repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserAccessRepository, UserAccessRepository>();
builder.Services.AddScoped<IDBContextResolver, DBContextResolver>();

builder.Services.AddDbContextFactory<CafeLokaalContext>();


// Configure kestrel to listen on port 8080
// builder.WebHost.UseUrls("http://0.0.0.0:8080");
// 🔥 Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStaticWebApp", policy =>
    {
        policy.WithOrigins("https://orange-smoke-0bf9dda03.2.azurestaticapps.net") // your Angular app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddApplicationInsightsTelemetry();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAngularLocalhost");
}

app.UseCors("AllowStaticWebApp");

// app.UseHttpsRedirection();

// Add authentication & authorization to the pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();