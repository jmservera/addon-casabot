using CasaBot.Components;
using CasaBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure to listen on port 8000
builder.WebHost.UseUrls("http://0.0.0.0:8000");

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
