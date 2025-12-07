namespace PositionManagement.Service.Models;

public class Trade
{
    public int Id { get; set; }

    public string Security { get; set; } = string.Empty;

    public TxSide Side { get; set; }

    public long Quantity { get; set; }

    public int Version { get; set; }
}