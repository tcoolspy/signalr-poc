using System.Collections.Concurrent;
using FoodOrdering.Data;
using FoodOrdering.Hubs.Interfaces;
using FoodOrdering.Hubs.Models;
using FoodOrdering.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Hubs;

public class FoodHub :  Hub<IFoodOrderClient>
{
    private readonly DataContext _context;
    protected static ConcurrentDictionary<int, Timer> _timers = new();
    protected static ConcurrentDictionary<int, int> _currentOrderTimes = new();
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
    //private int numberOfSeconds = 0;
    private int maxNumberOfSeconds = 600;
    
    public FoodHub(DataContext context)
    {
        _context = context;
    }

    public async Task OrderFoodItem(FoodRequest request)
    {
        var newOrder = new Order
        {
            FoodItemId = request.foodId,
            OrderDate = DateTimeOffset.Now,
            TableNumber = request.table,
            OrderState = OrderState.Ordered
        };
        
        _context.Orders.Add(newOrder);
        
        await _context.SaveChangesAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, newOrder.Id.ToString());
        await StartTimer(newOrder);
        await EmitActiveOrders();
    }
    
    public async Task UpdateFoodItem(int orderId, OrderState state)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.OrderState = state;
        }

        await _context.SaveChangesAsync();
        await EmitActiveOrders();
        if (order?.OrderState == OrderState.Completed)
        {
            await StopTimer(order);
        }
    }
    public async Task EmitActiveOrders()
    {
        var orders = _context.Orders.Include(x => x.FoodItem).Where(x => x.OrderState != OrderState.Completed).ToList();
        await Clients.All.PendingFoodUpdated(orders);
    }
    
    public async Task StartTimer(Order order)
    {
        await BroadcastTimerStarted(order.Id);
        var timer = new Timer(UpdateTimer, order, _interval, _interval);
        _timers.TryAdd(order.Id, timer);
        _currentOrderTimes.TryAdd(order.Id, 0);
    }

    public async Task StopTimer(Order order)
    {
        var timer = _timers[order.Id];
        timer.Change(Timeout.Infinite, Timeout.Infinite);
        await BroadcastTimerEnded(order.Id);
        _timers.TryRemove(order.Id, out timer);
        _currentOrderTimes.TryRemove(order.Id, out _);
    }
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        Console.WriteLine(Context.ConnectionId);
        await base.OnDisconnectedAsync(ex);
    }
    
    #region Private Methods

    private async void UpdateTimer(object state)
    {
        var order = state as Order;
        var currentTimeExists = _currentOrderTimes.TryGetValue(order.Id, out var value);
        if (currentTimeExists)
        {
            var currentOrderTime = _currentOrderTimes[order.Id];
            if (currentOrderTime <= maxNumberOfSeconds)
            {
                currentOrderTime += 1;
                _currentOrderTimes[order.Id] = currentOrderTime;
                await BroadcastTimeChanged();
            }
            else
            {
                await StopTimer(order);
            }
        }
    }

    private async Task BroadcastTimeChanged()
    {
        var currentTimers = _currentOrderTimes.ToDictionary();
        List<TimerUpdate> timers = new();
        foreach (var currentTimer in currentTimers)
        {
            var orderId = currentTimer.Key;
            var orderTime = _currentOrderTimes[currentTimer.Key];
            timers.Add(new TimerUpdate {OrderId = orderId, NumberOfSeconds = orderTime });
        }
        await Clients.All.UpdateOrderTime(timers);
        //await Clients.All.SendAsync("TimeChanged", numberOfSeconds.ToString(), OrderId.ToString());
    }

    private async Task BroadcastTimerStarted(int orderId)
    {
        await Clients.All.TimerStarted();
        //await Clients.All.SendAsync("TimeStarted", CancellationToken.None);
    }

    private async Task BroadcastTimerEnded(int orderId)
    {
        await Clients.All.TimerEnded();
        //await Clients.All.SendAsync("TimeEnded", CancellationToken.None);
    }
    
    #endregion
}