using FoodOrdering.Hubs.Interfaces;
using FoodOrdering.Models;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Hubs.Extensions;

public static class HubExtensions
{
    public static void PendingFoodUpdate(this IHubContext<FoodHub, IFoodOrderClient> context, List<Order> orders)
    {
        context.Clients.All.PendingFoodUpdated(orders);
    }
}