namespace PositionManagement.Service.Models;

public class Position
{
    public int TradeId { get; set; }

    public string SecurityCode { get; set; } = string.Empty;

    public long NetQuantity { get; set; }
}
