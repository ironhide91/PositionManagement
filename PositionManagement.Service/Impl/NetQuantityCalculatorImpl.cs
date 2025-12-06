using PositionManagement.Service.Core;
using PositionManagement.Service.Models;

namespace PositionManagement.Service.Impl;

public class NetQuantityCalculatorImpl : INetQuantityCalculator
{
    public long Calculate(Position position, Tx tx)
    {
        if (tx.Action == TxAction.Cancel)
        {
            return 0L;
        }

        if (tx.Action == TxAction.Update)
        {
            return tx.Quantity;
        }

        if (tx.Action == TxAction.Insert && tx.Side == TxSide.Buy)
        {
            return position.NetQuantity + tx.Quantity;
        }

        if (tx.Action == TxAction.Insert && tx.Side == TxSide.Sell)
        {
            return position.NetQuantity - tx.Quantity;
        }

        return position.NetQuantity;
    }
}
