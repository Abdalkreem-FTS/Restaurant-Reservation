using Elastic.Extensions.Logging;
using Elastic.Extensions.Logging.Options;
using Elastic.Ingest.Elasticsearch;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("The 'DefaultConnection' connection string is required.");

builder.Services.AddRestaurantReservationDb(connectionString);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<RestaurantReservationDbContext>();

builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ProblemExtensions.Customize);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddStackExchangeRedisCache(redis =>
{
    redis.Configuration = builder.Configuration.GetConnectionString("Redis")
                          ?? throw new InvalidOperationException("The 'Redis' connection string is required for token revocation.");

    redis.InstanceName = "restaurant-reservation:";
});

builder.Services.AddSingleton<ITokenRevocationStore, DistributedCacheTokenRevocationStore>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptionsMonitor<JwtOptions>, ILoggerFactory>((bearer, jwt, loggers) =>
    {
        bearer.MapInboundClaims = false;

        bearer.TokenValidationParameters = JwtTokenGenerator.CreateValidationParameters(jwt.CurrentValue);
        bearer.Events = JwtBearerEventHandlers.Create(loggers.CreateLogger(JwtBearerEventHandlers.LoggerCategory));
    });

builder.Services.AddAuthorizationPolicies();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<ApiDocumentTransformer>();
    options.AddOperationTransformer<ApiOperationTransformer>();
});

var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"];

if (!string.IsNullOrWhiteSpace(elasticsearchUrl))
{
    builder.Logging.AddElasticsearch(options =>
    {
        options.ShipTo.NodePoolType = NodePoolType.SingleNode;
        options.ShipTo.NodeUris = [new Uri(elasticsearchUrl)];
        options.DataStream = new DataStreamNameOptions
        {
            Type = "logs",
            DataSet = "restaurant-reservation",
            Namespace = "api"
        };
        options.BootstrapMethod = BootstrapMethod.Silent;
        options.IncludeScopes = true;
    });
}

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Restaurant Reservation API v1"));
    app.MapGrpcReflectionService();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGrpcService<ReservationsGrpcService>();

app.MapTokenEndpoints();
app.MapReservationEndpoints();
app.MapEmployeeEndpoints();

app.Run();
