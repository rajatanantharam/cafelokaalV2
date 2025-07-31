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
    options.AddPolicy("AllowAngularLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // your Angular app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
    // app.UseHttpsRedirection();
    IdentityModelEventSource.ShowPII = true;
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    app.UseCors("AllowAngularLocalhost");
}

// app.UseHttpsRedirection();

// Add authentication & authorization to the pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();