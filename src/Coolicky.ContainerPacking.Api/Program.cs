using Coolicky.ContainerPacking.Api.Services;
using Coolicky.ContainerPacking;
using Coolicky.ContainerPacking.Algorithms;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddOpenApi();
services.AddFastEndpoints();
services.AddScoped<IPackingService, PackingService>();
services.AddScoped<IPackingAlgorithm, EB_AFIT>();
services.AddScoped<IPackingSolver, GeneticPackingSolver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseFastEndpoints();
app.Run();