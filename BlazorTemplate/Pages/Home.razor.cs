using BlazorTemplate.Data;
using BlazorTemplate.Data.Entities;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemplate.Pages;
public partial class Home(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<Home> logger) {

    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
    private readonly ILogger<Home> _logger = logger;

    private GridItemsProvider<Worker> _provider = default!;
    private int _totalItemsCount;

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        _provider = async request => {
            var providerResult = new GridItemsProviderResult<Worker>() {
                Items = [],
                TotalItemCount = 0
            };
            try {
                _logger.LogInformation("Loading workers from database...");
                _logger.LogInformation("Applying filters, StartIndex: {}, Count: {}, Sorting On: {}", request.StartIndex, request.Count, string.Join(", ", request.GetSortByProperties()));
                await using var db = await _dbFactory.CreateDbContextAsync(request.CancellationToken);
                var workersQuery = db.Workers
                    .AsNoTracking()
                    .Skip(request.StartIndex);

                if (request.Count.HasValue) {
                    workersQuery = workersQuery.Take(request.Count.Value);
                }
                var count = await workersQuery.CountAsync(request.CancellationToken);

                if (count != _totalItemsCount && !request.CancellationToken.IsCancellationRequested) {
                    _totalItemsCount = count;
                    StateHasChanged();
                }

                providerResult = new GridItemsProviderResult<Worker>() {
                    Items = await request.ApplySorting(workersQuery).ToArrayAsync(request.CancellationToken),
                    TotalItemCount = count
                };
            }
            catch when (request.CancellationToken.IsCancellationRequested) { } // Ignore
            return providerResult;
        };
    }
}
