using Dapper;
using FAQApp_test.Repositories;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IFaqRepository, FaqRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        logger.LogWarning("ConnectionStrings:DefaultConnection is not configured.");
    }
    else
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            _ = await connection.ExecuteScalarAsync<int>("SELECT 1");
            logger.LogInformation("Database connection check succeeded.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database connection check failed.");
        }
    }
}

app.Run();
