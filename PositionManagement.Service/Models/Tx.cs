namespace PositionManagement.Service.Models;

public class Tx
{
    public int Id { get; set; }

    public int TradeId { get; set; }

    public int Version { get; set; }

    public string Security { get; set; } = string.Empty;

    public long Quantity { get; set; }

    public TxAction Action { get; set; }

    public TxSide Side { get; set; }
}
