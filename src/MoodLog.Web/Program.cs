using Microsoft.EntityFrameworkCore;
using MoodLog.Web.Components;
using MoodLog.Infrastructure;
using MoodLog.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMoodLogInfrastructure(builder.Configuration);

var app = builder.Build();

var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<MoodLogDbContext>>();
await using var dbContext = await dbContextFactory.CreateDbContextAsync();
await DatabaseInitializer.InitializeAsync(dbContext);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
