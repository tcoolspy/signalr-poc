namespace FoodOrdering.Hubs.Models;

public class TimerUpdate
{
    public int OrderId { get; set; }
    public int NumberOfSeconds { get; set; }
}