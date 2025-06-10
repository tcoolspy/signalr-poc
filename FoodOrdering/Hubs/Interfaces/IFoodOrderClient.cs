using FoodOrdering.Hubs.Models;
using FoodOrdering.Models;

namespace FoodOrdering.Hubs.Interfaces;

public interface IFoodOrderClient
{
    Task PendingFoodUpdated(List<Order> orders);
    Task UpdateOrderTime(List<TimerUpdate> currentTimers);
    Task TimerStarted();
    Task TimerEnded();
}