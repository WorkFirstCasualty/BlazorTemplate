using BlazorTemplate.Data;
using BlazorTemplate.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorTemplate.Pages;
public partial class AddWorker(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<AddWorker> logger) {
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
    private readonly ILogger<AddWorker> _logger = logger;

    private Company[] _companies = default!;
    private readonly Model _model = new();

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();
        await using var db = await _dbFactory.CreateDbContextAsync();
        _companies = await db.Companies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToArrayAsync();
        _logger.LogInformation("Discovered {} companies available", _companies.Length);
    }

    private async Task SubmitWorkerAsync() {
        _logger.LogInformation("Saving new worker {}", _model.FirstName + " " + _model.LastName);
        var worker = new Worker {
            AssignedCompanyId = _model.AssignedCompanyId,
            FirstName = _model.FirstName,
            LastName = _model.LastName,
            Email = _model.Email,
            PhoneNumber = _model.PhoneNumber,
            BirthDate = _model.BirthDate
        };

        await using var db = await _dbFactory.CreateDbContextAsync();
        await db.AddAsync(worker);
        await db.SaveChangesAsync();
    }


    private sealed class Model {
        public int? AssignedCompanyId { get; set; }
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        [Required, Phone]
        public string PhoneNumber { get; set; } = default!;
        [Required, Range(typeof(DateOnly), "01/01/2015", "01/01/2015")]
        public DateOnly BirthDate { get; set; }
    }
}
