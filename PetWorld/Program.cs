using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using PetWorld.Application.Interfaces;
using PetWorld.Application.Services;
using PetWorld.Components;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Agents;
using PetWorld.Infrastructure.Data;
using PetWorld.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PetWorldDbContext>(options =>
    options.UseMySQL(connectionString!));

// Configure Google Gemini AI
var geminiApiKey = string.IsNullOrWhiteSpace(builder.Configuration["Gemini:ApiKey"])
    ? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
    : builder.Configuration["Gemini:ApiKey"];

if (string.IsNullOrWhiteSpace(geminiApiKey))
    throw new InvalidOperationException("Gemini API Key not configured");

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var client = new Client(apiKey: geminiApiKey);
    return client.AsIChatClient("gemini-2.5-flash-lite");
});

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();

// Register application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();

// Register agent factory (Infrastructure layer)
builder.Services.AddScoped<IWriterAgentFactory, WriterAgentFactory>();
builder.Services.AddScoped<IWriterCriticService, WriterCriticService>();

var app = builder.Build();

// Database will be initialized via init-db.sql script
// Automatic migrations disabled due to Pomelo/EF Core 10 compatibility

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// Only use HTTPS redirection when not in Development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
