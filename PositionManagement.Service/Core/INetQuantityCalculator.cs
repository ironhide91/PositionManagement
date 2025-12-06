using PositionManagement.Service.Models;

namespace PositionManagement.Service.Core;

public interface INetQuantityCalculator
{
    long Calculate(Position position, Tx tx);
}