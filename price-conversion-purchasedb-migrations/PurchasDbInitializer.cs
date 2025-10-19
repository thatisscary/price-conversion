namespace price_conversion_purchasedb_migrations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using price_conversion_purchasedb;

    

    internal class PurchaseDbInitializer(IServiceProvider serviceProvider, ILogger<PurchaseDbInitializer> logger)
        : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
       

        private readonly ActivitySource _activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PurchaseDbContext>();

            using var activity = _activitySource.StartActivity("Initializing purchase database", ActivityKind.Client);
            await InitializeDatabaseAsync(dbContext, cancellationToken);
        }

        public async Task InitializeDatabaseAsync(PurchaseDbContext dbContext, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));
            dbContext.Database.ExecuteSqlAsync
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

            

            logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        }
    }
}