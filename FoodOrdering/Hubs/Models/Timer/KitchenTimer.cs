using System.Collections.Concurrent;
using FoodOrdering.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Hubs.Models.Timer;

public class KitchenTimer : System.Timers.Timer
{
    public static ConcurrentDictionary<int, KitchenTimer> Timers = new ConcurrentDictionary<int, KitchenTimer>();
}