using FoodOrdering.Models;

namespace FoodOrdering.Hubs.Interfaces;

public interface IFoodOrderClient
{
    Task PendingFoodUpdated(List<Order> orders);
}