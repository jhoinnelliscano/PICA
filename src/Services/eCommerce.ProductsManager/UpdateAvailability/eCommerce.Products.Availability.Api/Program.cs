using eCommerce.Products.Availability.Api.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Reflection;
using System.Text;
using eCommerce.Products.Availability.Core.Config;
using eCommerce.Products.Availability.Core.Contracts.Repositories;
using eCommerce.Products.Availability.Core.Contracts.Services;
using eCommerce.Products.Availability.Core.Helpers.BadRequests;
using eCommerce.Products.Availability.Core.Helpers.Log;
using eCommerce.Products.Availability.Core.Helpers.Mappers;
using eCommerce.Products.Availability.Infraestructure.Models.UnitOfWorks;
using eCommerce.Products.Availability.Infraestructure.Repositories;
using eCommerce.Products.Availability.Infraestructure.Services;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.HealthChecks;
using eCommerce.Commons.Objects.Responses.HealthCheck;
using eCommerce.Products.Availability.Infraestructure.Contexts.DbProduct;
using eCommerce.Products.Availability.Core.PublisherConsumer;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.Commons.Objects.Messaging;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Commons.Security;
using eCommerce.PublisherSubscriber.MessagingRabbit;
using eCommerce.PublisherSubscriber.Messaging;
using eCommerce.PublisherSubscriber.Object;
//using eCommerce.PublisherSubscriber.MessagingRabbit;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                    .AllowAnyHeader().
                                        AllowAnyMethod();
                      });
});

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = AppConfiguration.Configuration["AppConfiguration:ApiSwaggerName"].ToString(), Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[]{}
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Use bearer token to authorize (enter into field the word 'Bearer' following by space and JWT)",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
    });
});
#endregion

builder.Services.AddMvc()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var modelState = actionContext.ModelState;
            return new BadRequestObjectResult(new ServiceResponse<Dictionary<string, string[]>>(
                "Bad request", BadRequestHelper.GetValidationResult(modelState)));
        };
    });

#region Log config
var levelSwitch = new LoggingLevelSwitch();
levelSwitch.MinimumLevel = LogEventLevel.Information;

var basePath = AppConfiguration.Configuration["AppConfiguration:Log:SeqFilePath"].ToString() + "\\" + AppConfiguration.Configuration["AppConfiguration:ApiCode"].ToString();
if (!System.IO.Directory.Exists(basePath))
    System.IO.Directory.CreateDirectory(basePath);

var filePath = "[BASEPATH]\\" + "Log-[DATE].txt";
filePath = filePath.Replace("[BASEPATH]", basePath);
filePath = filePath.Replace("[DATE]", DateTime.Now.ToString("yyyy-MM-dd"));

builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("Application", "API-" + AppConfiguration.Configuration["AppConfiguration:ApiCode"].ToString())
    .WriteTo.Seq(AppConfiguration.Configuration["AppConfiguration:Log:SeqHost"].ToString(),
        apiKey: AppConfiguration.Configuration["AppConfiguration:Log:SeqApiKey"].ToString(),
        bufferBaseFilename: filePath,
        controlLevelSwitch: levelSwitch));
#endregion


builder.Services.AddDbContext<DbProductContext>(
    options => options.UseSqlServer(AppConfiguration.Configuration["AppConfiguration:DataBases:DbProduct:ConnectionString"].ToString()));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


#region Publisher config
//builder.Services.AddScoped<IPublisher<IEnumerable<ProductMsg>>, PublisherProductMsg>();
//builder.Services.AddHostedService<ConsumerOrderMsg>();
builder.Services.AddSingleton<IMessagingFactory, RabbitMQFactoryCreator>();
//builder.Services.AddScoped<IPublisher3, TareasPublisher>();
builder.Services.AddScoped<IPublisher, PublisherMessage>();
//builder.Services.AddHostedService<TareasConsumer>();
builder.Services.AddHostedService<ConsumerPurchaseOrderMsg>();
#endregion


builder.Services.AddSingleton<ILogHelper, LogHelper>();
builder.Services.AddScoped<IMapperHelper, MapperHelper>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: AppConfiguration.Configuration["AppConfiguration:DataBases:DbProduct:ConnectionString"].ToString(),
        healthQuery: "SELECT 1;",
        name: "SqlServerContext")
    .AddCheck<MemoryHealthCheck>("Memory");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = AppConfiguration.Configuration["AppConfiguration:Authentication:Authority"].ToString();
    options.Audience = AppConfiguration.Configuration["AppConfiguration:Authentication:Audience"].ToString();
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("update:product", policy => policy.Requirements.Add(new HasScopeRequirement("update:product", AppConfiguration.Configuration["AppConfiguration:Authentication:Authority"].ToString())));
});
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();



var app = builder.Build();

//app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", AppConfiguration.Configuration["AppConfiguration:ApiSwaggerName"].ToString());
    });
}

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();

    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 1024, true))
    {
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Seek(0, SeekOrigin.Begin);
    }

    await next.Invoke();
});

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            HealtChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description
            }),
            HealthCheckDuration = report.TotalDuration
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

ILogHelper logHelper = app.Services.GetRequiredService<ILogHelper>();
app.ConfigureExceptionHandler(logHelper);

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
