using PositionManagement.Service.Models;

namespace PositionManagement.Service.Core;

public interface IPositionService
{
    Task<IReadOnlyList<Position>> GetPositionsAsync();

    Task<IReadOnlyList<Trade>> GetTradesAsync();

    Task<IReadOnlyList<Tx>> GetTransactionsAsync();    

    Task Add(Tx tx);
}