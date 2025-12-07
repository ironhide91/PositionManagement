using Microsoft.EntityFrameworkCore;
using PositionManagement.Service.Models;

namespace PositionManagement.Service.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
    {

    }

    public DbSet<Tx> Transactions => Set<Tx>();

    public DbSet<Trade> Trades => Set<Trade>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<TxOutOfOrder> TxOutOfOrders => Set<TxOutOfOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureTrade(modelBuilder);
        ConfigureTx(modelBuilder);
        ConfigurePosition(modelBuilder);
        ConfigureTxOutOfOrder(modelBuilder);
    }

    private void ConfigureTrade(ModelBuilder modelBuilder)
    {
        var trade = modelBuilder.Entity<Trade>();
        trade.HasKey(t => t.Id);
        trade.Property(t => t.Id).ValueGeneratedOnAdd();
        trade.Property(t => t.Security).IsRequired();
        trade.Property(t => t.Side).IsRequired();
        trade.Property(t => t.Version).IsRequired();
    }

    private void ConfigureTx(ModelBuilder modelBuilder)
    {
        var tx = modelBuilder.Entity<Tx>();
        tx.HasKey(t => t.Id);
        tx.HasIndex(t => new { t.Id, t.TradeId, t.Version }).IsUnique();
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
        position.HasKey(t => t.Security);
        position.Property(t => t.NetQuantity).IsRequired();
    }

    private void ConfigureTxOutOfOrder(ModelBuilder modelBuilder)
    {
        var txOutOfOrder = modelBuilder.Entity<TxOutOfOrder>();
        txOutOfOrder.HasKey(t => t.Id);
        txOutOfOrder.HasIndex(t => new { t.Id, t.TradeId }).IsUnique();
        txOutOfOrder.Property(t => t.TradeId).IsRequired();
    }
}