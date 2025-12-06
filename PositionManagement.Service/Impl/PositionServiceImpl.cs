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
        await dbContext.SaveChangesAsync();

        var position = await dbContext.Positions
            .FirstOrDefaultAsync(p => p.SecurityCode.Equals(tx.SecurityCode));

        if (position == null)
        {
            await Create(tx);
            return;
        }

        if (tx.Action == TxAction.Cancel)
        {
            await Cancel(position);
            return;
        }

        await Update(position, tx);
    }    

    private async Task Create(Tx tx)
    {
        var quantity = tx.Side == TxSide.Buy ? tx.Quantity : -tx.Quantity;

        if (tx.Action != TxAction.Insert)
        {
            await dbContext.TxOutOfOrders.AddAsync(new TxOutOfOrder
            {
                Id = tx.Id,
                TradeId = tx.TradeId
            });

            quantity = 0;
        }

        await dbContext.Positions.AddAsync(new Position
        {
            SecurityCode = tx.SecurityCode,
            NetQuantity = quantity
        });

        await dbContext.SaveChangesAsync();
    }

    private async Task Update(Position position, Tx tx)
    {
        var outOfOrderTxs = await dbContext.TxOutOfOrders
            .Join(dbContext.Transactions,
                txo => new { txo.Id, txo.TradeId },
                t => new { t.Id, t.TradeId },
                (txo, t) => new { tx = t, txo })
            .ToListAsync();

        if (outOfOrderTxs.Any() && tx.Action == TxAction.Insert)
        {
            position.NetQuantity += outOfOrderTxs.Sum(x => netQuantityCalculator.Calculate(position, x.tx));
            dbContext.TxOutOfOrders.RemoveRange(outOfOrderTxs.Select(x => x.txo));
            await dbContext.SaveChangesAsync();
            return;
        }

        if (outOfOrderTxs.Any() && tx.Action == TxAction.Update)
        {
            return;
        }

        position.NetQuantity = netQuantityCalculator.Calculate(position, tx);
        await dbContext.SaveChangesAsync();
    }

    private async Task Cancel(Position position)
    {
        position.NetQuantity = 0;
        await dbContext.SaveChangesAsync();
    }
}
