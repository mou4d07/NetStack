using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Models;

namespace NetMapManager.API.Data
{
    public class NetMapDbContext : DbContext
    {
        public NetMapDbContext(DbContextOptions<NetMapDbContext> options) : base(options) { }

        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<NetworkSwitch> NetworkSwitches { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<SwitchModel> SwitchModels { get; set; }
        public DbSet<VlanDefinition> VlanDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Switch Hierarchy (Pyramidal structure)
            modelBuilder.Entity<NetworkSwitch>()
                .HasOne(s => s.ParentSwitch)
                .WithMany(s => s.ChildSwitches)
                .HasForeignKey(s => s.ParentSwitchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Link relationships
            modelBuilder.Entity<Link>()
                .HasOne(l => l.SourceSwitch)
                .WithMany()
                .HasForeignKey(l => l.SourceSwitchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Link>()
                .HasOne(l => l.DestinationSwitch)
                .WithMany()
                .HasForeignKey(l => l.DestinationSwitchId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Ensure DomainUsername is unique
            modelBuilder.Entity<AuthorizedUser>()
                .HasIndex(u => u.DomainUsername)
                .IsUnique();
        }
    }
}
