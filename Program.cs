using dotnetMVP.Middleware;
using dotnetMVP.Models;
using dotnetMVP.Models.DTO.ChallengeDto;
using dotnetMVP.Models.DTO.PlatformDto;
using dotnetMVP.Models.DTO.User;
using dotnetMVP.Models.DTO.Writeup;
using dotnetMVP.Models.Entities;
using dotnetMVP.Services.Interface;
using dotnetMVP.Services.Realization;
using dotnetMVP.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSerilog();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("WriteUpDemo")) //later change to container id or some unique value to identify service in 3rd party collector (loki, prometheus, etc.)
                                                         //.UseOtlpExporter(OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf,)     [ app -> 3rd party collector (loki,prometheus,etc.)]
    .WithTracing(r =>
    {
        r.AddAspNetCoreInstrumentation(); // использование интструментов SDK(?) под трейсы и спаны ( ниже то же, но с метриками ) 
        r.AddHttpClientInstrumentation();
        //r.AddConsoleExporter();
    })
    .WithMetrics(r =>
    {
        r.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter();       // [ prometheus -> app ]

    });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDBcontext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresqlDefault"))
);

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDBcontext>();

builder.Services.AddScoped<IWriteUpService, WriteUpService>();
builder.Services.AddScoped<WriteupMapper>();

builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<ChallengeMapper>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserMapper>();

builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<PlatformMapper>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(options =>
                {
                    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = jwtOptions.ValidateIssuer,
                        ValidateLifetime = jwtOptions.ValidateLifetime,
                        ValidateIssuerSigningKey = jwtOptions.ValidateKey,
                        ValidateAudience = jwtOptions.ValidateAudience,

                        ValidIssuer = jwtOptions.ValidIssuer,
                        ValidAudience = jwtOptions.ValidAudience,

                        //Change later to more secure approach , like using a key vault or environment variable ( mb docker env variables ) 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SymmetricKey)),
                        ValidAlgorithms = jwtOptions.ValidAlgotrithms
                    };
                });

builder.Services.AddScoped<IJwtHandler, JwtHandler>();

builder.Services.ConfigurePolicies();

//Later change to Configure().Bind().ValidateOnStart() [optional + data annotations to meet "requirements" with keys]


builder.Services.AddSwaggerGen();



var app = builder.Build();

using (var serv = app.Services.CreateScope())
{
    var db = serv.ServiceProvider.GetRequiredService<ApplicationDBcontext>();

    // Migrate to the latest version of the database schema
    db.Database.Migrate();

    if (app.Environment.IsDevelopment())
    {
        // seed the database with initial data for development environment
        SeedDatabase seed = new SeedDatabase(db, serv.ServiceProvider.GetRequiredService<UserManager<AppUser>>());

        await seed.EnsureRolesAndAdmins();
    }
}
app.UseHsts();


//Enabling Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();
app.Run();