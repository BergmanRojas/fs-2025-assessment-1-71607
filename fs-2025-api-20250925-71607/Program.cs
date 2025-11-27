using fs_2025_api_20250925_71607.Endpoints;
using fs_2025_api_20250925_71607.Startup;

var builder = WebApplication.CreateBuilder(args);

// Swagger (ya lo tenías)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ⬇️ Habilitar Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⬇️ Requerido por Blazor
app.UseStaticFiles();
app.UseAntiforgery();

// Tus endpoints de API
app.AddWeatherEndPoints();
app.AddCourseEndPoints();
app.AddRootEndPoints();
app.AddBookEndPoints();

// ⬇️ Montar la App de Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();