using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.http.Extentions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrationAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.WebhooksDbContext>();
            await dbContext.Database.MigrateAsync();
            return;
        }
    }
}
