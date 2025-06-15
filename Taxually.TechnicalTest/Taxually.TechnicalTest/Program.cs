using Taxually.TechnicalTest;
using Taxually.TechnicalTest.Application.Interfaces;
using Taxually.TechnicalTest.Application.Services;
using Taxually.TechnicalTest.Infrastructure.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IQueuePublisher, TaxuallyQueueClient>();
builder.Services.AddScoped<IHttpPoster, TaxuallyHttpClient>();

builder.Services.AddScoped<IRegistrationStrategy, GermanRegistrationStrategy>();
builder.Services.AddScoped<IRegistrationStrategy, FranceRegistrationStrategy>();
builder.Services.AddScoped<IRegistrationStrategy, UkRegistrationStrategy>();

builder.Services.AddScoped<RegistrationResolver>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
