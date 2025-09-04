using CasaBot.Components;
using CasaBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure to listen on port 8000 by default, but allow override via environment
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://0.0.0.0:8000");
}

// Configure forwarded headers for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HTTP client
builder.Services.AddHttpClient();

// Add custom services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMcpService, McpService>();

// Configure logging
builder.Logging.AddConsole();

var app = builder.Build();

// Use forwarded headers
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Don't use HSTS when behind a reverse proxy
    // app.UseHsts();
}

// Don't redirect to HTTPS when behind a reverse proxy
// app.UseHttpsRedirection();

// Add health check endpoint
app.MapGet("/health", () => "OK");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
