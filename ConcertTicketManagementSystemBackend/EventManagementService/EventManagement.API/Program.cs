using EventManagement.Infrastructure;
using Serilog;
using Shared.Common;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IEventRepository>(sp =>
    new EventRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<IServiceHandler<IVenueCapacityService>, HttpServiceHandler<IVenueCapacityService>>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost:5003"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();