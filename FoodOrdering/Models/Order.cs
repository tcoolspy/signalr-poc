namespace FoodOrdering.Models;

public class Order
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public int FoodItemId { get; set; }
    public FoodItem FoodItem { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public OrderState OrderState { get; set; }
}