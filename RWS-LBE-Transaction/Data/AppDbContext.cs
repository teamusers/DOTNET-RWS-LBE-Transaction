using Microsoft.EntityFrameworkCore; 
using RWS_LBE_Transaction.Models;

namespace RWS_LBE_Transaction.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
         
        public DbSet<SysChannel> SysChannel { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<TransactionIDRecord> TransactionIDRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
