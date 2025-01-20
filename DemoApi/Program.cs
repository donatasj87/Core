using Donatas.Core.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.AddCoreWebApplication(); // Setting up core builder things
var app = builder.Build();

app.UseCoreWebApplication(); // Setting up core application things

await app.RunAsync();
