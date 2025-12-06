using Microsoft.EntityFrameworkCore;
using PositionManagement.Service.Models;

namespace PositionManagement.Service.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
    {

    }

    public DbSet<Tx> Transactions => Set<Tx>();

    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureTx(modelBuilder);
        ConfigurePosition(modelBuilder);
    }

    private void ConfigureTx(ModelBuilder modelBuilder)
    {
        var tx = modelBuilder.Entity<Tx>();
        tx.HasKey(t => t.Id);
        tx.Property(t => t.Id).ValueGeneratedOnAdd();
        tx.Property(t => t.TradeId).IsRequired();
        tx.Property(t => t.Version).IsRequired();
        tx.Property(t => t.Quantity).IsRequired();
        tx.Property(t => t.Action).IsRequired();
        tx.Property(t => t.Side).IsRequired();
    }

    private void ConfigurePosition(ModelBuilder modelBuilder)
    {
        var position = modelBuilder.Entity<Position>();
        position.HasKey(t => t.TradeId);
        position.Property(t => t.SecurityCode).IsRequired();
        position.Property(t => t.NetQuantity).IsRequired();
    }
}