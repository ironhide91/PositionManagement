using Microsoft.EntityFrameworkCore;
using PositionManagement.Service.Core;
using PositionManagement.Service.Data;
using PositionManagement.Service.Impl;
using PositionManagement.Service.Models;

namespace PositionManagement.Test;

public class PositionServiceImplTests
{
    private static TradingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TradingDbContext(options);
    }

    [Fact]
    public async void Sanity()
    {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new PositionServiceImpl(dbContext, new NetQuantityCalculatorImpl());
        var txs = new[]
        {
            new Tx { Id = 1, TradeId = 1, Version = 1, SecurityCode = "REL", Quantity = 50, Action = TxAction.Insert,  Side = TxSide.Buy  },
            new Tx { Id = 2, TradeId = 2, Version = 1, SecurityCode = "ITC", Quantity = 40, Action = TxAction.Insert,  Side = TxSide.Sell },
            new Tx { Id = 3, TradeId = 3, Version = 1, SecurityCode = "INF", Quantity = 70, Action = TxAction.Insert,  Side = TxSide.Buy  },
            new Tx { Id = 4, TradeId = 1, Version = 2, SecurityCode = "REL", Quantity = 60, Action = TxAction.Update,  Side = TxSide.Buy  },
            new Tx { Id = 5, TradeId = 2, Version = 2, SecurityCode = "ITC", Quantity = 30, Action = TxAction.Cancel,  Side = TxSide.Buy  },
            new Tx { Id = 6, TradeId = 4, Version = 1, SecurityCode = "INF", Quantity = 20, Action = TxAction.Insert,  Side = TxSide.Sell },
        };

        // act
        foreach (var tx in txs)
        {
            await sut.Add(tx);
        }

        // assert
        var positions = await sut.GetPositionsAsync();
        Assert.Equal(3, positions.Count);

        var rel = positions.Single(p => p.SecurityCode == "REL");
        var itc = positions.Single(p => p.SecurityCode == "ITC");
        var inf = positions.Single(p => p.SecurityCode == "INF");

        Assert.Equal(60, rel.NetQuantity);   // REL +60
        Assert.Equal(0, itc.NetQuantity);   // ITC  0
        Assert.Equal(50, inf.NetQuantity);   // INF +50
    }

    [Fact]
    public async void SingleStockOutOfOrder()
    {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new PositionServiceImpl(dbContext, new NetQuantityCalculatorImpl());
        var txs = new[]
        {
            new Tx { Id = 4, TradeId = 1, Version = 2, SecurityCode = "REL", Quantity = 20, Action = TxAction.Update,  Side = TxSide.Buy  },
            new Tx { Id = 5, TradeId = 1, Version = 3, SecurityCode = "REL", Quantity = 90, Action = TxAction.Cancel,  Side = TxSide.Buy  },
            new Tx { Id = 6, TradeId = 1, Version = 1, SecurityCode = "REL", Quantity = 20, Action = TxAction.Insert,  Side = TxSide.Buy },
        };

        // act
        foreach (var tx in txs)
        {
            await sut.Add(tx);
        }

        // assert
        var positions = await sut.GetPositionsAsync();
        Assert.Equal(1, positions.Count);
        var rel = positions.Single(p => p.SecurityCode == "REL");
        Assert.Equal(0, rel.NetQuantity);
    }
}
