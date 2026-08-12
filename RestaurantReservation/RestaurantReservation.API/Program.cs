using Elastic.Extensions.Logging;
using Elastic.Extensions.Logging.Options;
using Elastic.Ingest.Elasticsearch;
using FluentValidation;

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

builder.Services.AddSecurity(builder.Configuration);

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
