using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebAPI.Extensions;
using WebAPI.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddNewtonsoftJson();

        //Add API document
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", info: new OpenApiInfo()
            {
                Title = "JWT Web API",
                Version = "1",
                Description = "Simple JWT Web API"

            });

            var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            options.IncludeXmlComments(xmlCommentFullPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearrer scheme.\r\n\r\n Enter 'Bearer' [space] and your token in the text input below. \r\n\r\n Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        //Add API versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        }).AddMvc();

        //Add auto mapper
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        string conn = builder.Configuration.GetConnectionString("WebAPIDbContext");
        builder.Services.AddDbContext<WebAPIDbContext>(options =>
        {
            options.UseSqlServer(conn);
        });

        //Mapping AppSettings json to AppSetting Class and get Secret string
        var appSettingsSection = builder.Configuration.GetSection("AppSettings");
        builder.Services.Configure<AppSettings>(appSettingsSection);
        var appSettings = appSettingsSection.Get<AppSettings>();
        var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        builder.Services.AddUserServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        //Use API document
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Web API");
            options.RoutePrefix = "";
        });

        app.UseHttpsRedirection();

        //Add Authentication
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}