using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using UT5_TFU.Repositories;
using UT5_TFU.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo
    {
        Title = "UT5_TFU",
        Version = "V1",
        Description = "Tarea final de la unidad numero 5 de Analisis y diseno de aplicaciones (ADA/ANDIS)"
    });

    options.EnableAnnotations();
});

builder.Services.AddSingleton<IFoodTruckRepository, FoodTruckEnMemoriaRepository>();
builder.Services.AddScoped<IPuestoComidaService, PuestoComidaService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IPedidoEstadoObserver, PedidoListoNotificacionObserver>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyEfectivo>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyTarjeta>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyMercadoPagoStrategy>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/V1/swagger.json", "UT5_TFU");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
