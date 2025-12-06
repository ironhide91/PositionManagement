using Microsoft.EntityFrameworkCore;
using PositionManagement.Service.Core;
using PositionManagement.Service.Data;
using PositionManagement.Service.Models;

namespace PositionManagement.Service.Impl;

public class PositionServiceImpl : IPositionService
{
    private readonly TradingDbContext dbContext;
    private readonly INetQuantityCalculator netQuantityCalculator;

    public PositionServiceImpl(TradingDbContext dbContext, INetQuantityCalculator netQuantityCalculator)
    {
        this.dbContext = dbContext;
        this.netQuantityCalculator = netQuantityCalculator;
    }

    public async Task<IReadOnlyList<Tx>> GetTransactionsAsync()
    {
        var txs = await dbContext.Transactions
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();

        return txs;
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync()
    {
        var positions = await dbContext.Positions
            .AsNoTracking()
            .OrderBy(t => t.SecurityCode)
            .ToListAsync();

        return positions;
    }

    public async Task Add(Tx tx)
    {
        await dbContext.Transactions.AddAsync(tx);

        var position = await dbContext.Positions
            .FirstOrDefaultAsync(p => p.SecurityCode.Equals(tx.TradeId));

        if (position == null)
        {
            var newPosition = new Position
            {
                SecurityCode = tx.SecurityCode,
                NetQuantity = tx.Side == TxSide.Buy ? tx.Quantity : -tx.Quantity
            };

            await dbContext.Positions.AddAsync(newPosition);
            await dbContext.SaveChangesAsync();
            return;
        }

        position.NetQuantity = netQuantityCalculator.Calculate(position, tx);
        await dbContext.SaveChangesAsync();
    }
}
