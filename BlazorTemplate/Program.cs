using BlazorTemplate.Components;
using BlazorTemplate.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options => {
    options.UseSqlite(ApplicationDbContext.ConnectionString, o => o.MigrationsAssembly("BlazorTemplate.Data.Migrations"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    var dbFactory = app.Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    var db = dbFactory.CreateDbContext();
    db.Database.Migrate();
}
else { // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
