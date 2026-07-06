using MongoDB.Driver;
using Portfolio.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoSection = builder.Configuration.GetSection("MongoDbSettings");
var mongoConnection = mongoSection.GetValue<string>("ConnectionString");
var mongoDatabaseName = mongoSection.GetValue<string>("DatabaseName");
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnection));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = exceptionHandlerFeature?.Error;
        if (ex != null)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing request.");
        }

        var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
        await context.Response.WriteAsync(result);
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
