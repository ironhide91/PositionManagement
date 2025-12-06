namespace PositionManagement.Service.Models;

public class Position
{
    public string TradeId { get; set; } = string.Empty;

    public string SecurityCode { get; set; } = string.Empty;

    public long NetQuantity { get; set; }
}
