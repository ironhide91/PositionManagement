using PositionManagement.Service.Models;

namespace PositionManagement.Service.Core;

public interface IPositionService
{
    Task<IReadOnlyList<Tx>> GetTransactionsAsync();

    Task<IReadOnlyList<Position>> GetPositionsAsync();

    Task Add(Tx tx);
}