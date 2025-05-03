using mw_cw8_proj.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<ITripsService, TripsService>();
builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddOpenApi();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

