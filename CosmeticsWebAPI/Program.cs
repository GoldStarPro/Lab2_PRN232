using System.Text;
using System.Text.Json.Serialization;
using BusinessObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<CosmeticInformation>("CosmeticInformations");
modelBuilder.EntitySet<CosmeticCategory>("CosmeticCategories");

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
}).AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata", modelBuilder.GetEdmModel()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICosmeticInformationRepository, CosmeticInformationRepository>();
builder.Services.AddScoped<ICosmeticInformationService, CosmeticInformationService>();
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();

IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidIssuer = configuration["JWT:Issuer"],
        ValidAudience = configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!))
    };
});

// Add Swagger JWT configuration
builder.Services.AddSwaggerGen(c =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "JWT Authentication for Cosmetics Management",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] { }
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role") &&
            context.User?.FindFirst(claim => claim.Type == "Role")!.Value == "1"))
    .AddPolicy("AdminOrStaffOrMember", policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && (context.User?.FindFirst(claim => claim.Type == "Role")!.Value == "1"
            || context.User?.FindFirst(claim => claim.Type == "Role")!.Value == "3"
                        || context.User?.FindFirst(claim => claim.Type == "Role")!.Value == "4")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
