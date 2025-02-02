using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WFHSocial.Api.Application;
using WFHSocial.Api.Application.ExceptionHandling;
using WFHSocial.Api.Application.MappingProfiles;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Infrastructure;
using WFHSocial.Api.Infrastructure.Data;
using WFHSocial.Api.Infrastructure.Data.Repositories;
using WFHSocial.Api.Infrastructure.Data.SeedingDbs;
using WFHSocial.Api.Middleware;
using WFHSocial.Utility.LoggingOpenTelemetry;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenTelemetry(builder.Configuration, builder.Environment.EnvironmentName);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<IdentityRole>();

var key = builder.Configuration.GetSection("JWT:Key").Value!;
var issuer = builder.Configuration.GetSection("JWT:Issuer").Value!;
var audience = builder.Configuration.GetSection("JWT:Audience").Value!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateActor = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience
    };
});


builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(UserMappingProfiles));
builder.Services.AddExceptionHandler<ProbelmDetailsExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.ValidateAndSeedDatabases(builder.Configuration, builder.Environment.EnvironmentName);
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
});
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();


app.UseSwagger();
    app.UseSwaggerUI();

//ENABLE CORS - purely for testing on development deployment
app.UseCors(x => x
   .AllowAnyMethod()
   .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true) // allow any origin  
   .AllowCredentials()
   );

app.UseCustomJwtMiddleware();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseClientApplication();
app.MapControllers();

app.Run();


