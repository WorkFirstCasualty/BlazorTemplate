using BlazorTemplate.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTemplate.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) {
    public const string ConnectionString = "Data Source=app.db";

    public virtual DbSet<Worker> Workers { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
}
