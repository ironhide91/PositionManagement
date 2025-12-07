using Microsoft.AspNetCore.SignalR;
using PositionManagement.Service.Core;
using PositionManagement.Service.Models;

namespace PositionManagement.Service.Api;

public class SignalRHub : Hub
{
    private readonly IPositionService positionService;

    public SignalRHub(IPositionService positionService)
    {
        this.positionService = positionService;
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync()
    {
        return await positionService.GetPositionsAsync();
    }

    public async Task<IReadOnlyList<Trade>> GetTradesAsync()
    {
        return await positionService.GetTradesAsync();
    }

    public async Task<IReadOnlyList<Tx>> GetTransactionsAsync()
    {
        return await positionService.GetTransactionsAsync();
    }    

    public async Task InsertAsync(Tx tx)
    {
        tx.Action = TxAction.Insert;
        await positionService.Add(tx);

        var trades = await positionService.GetTradesAsync();
        var txs = await positionService.GetTransactionsAsync();
        var positions = await positionService.GetPositionsAsync();

        await Clients.All.SendAsync("TradesUpdated", trades);
        await Clients.All.SendAsync("TransactionsUpdated", txs);
        await Clients.All.SendAsync("PositionsUpdated", positions);
    }
    
    public async Task UpdateAsync(Tx tx)
    {
        tx.Action = TxAction.Update;
        await positionService.Add(tx);

        var trades = await positionService.GetTradesAsync();
        var txs = await positionService.GetTransactionsAsync();
        var positions = await positionService.GetPositionsAsync();

        await Clients.All.SendAsync("TradesUpdated", trades);
        await Clients.All.SendAsync("TransactionsUpdated", txs);
        await Clients.All.SendAsync("PositionsUpdated", positions);
    }

    public async Task CancelAsync(Tx tx)
    {
        tx.Id = 0;
        tx.Action = TxAction.Cancel;
        await positionService.Add(tx);

        var trades = await positionService.GetTradesAsync();
        var txs = await positionService.GetTransactionsAsync();
        var positions = await positionService.GetPositionsAsync();

        await Clients.All.SendAsync("TradesUpdated", trades);
        await Clients.All.SendAsync("TransactionsUpdated", txs);
        await Clients.All.SendAsync("PositionsUpdated", positions);
    }
}
