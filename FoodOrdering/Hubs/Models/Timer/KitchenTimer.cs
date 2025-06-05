using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Hubs.Models.Timer;

public class KitchenTimer
{
    private System.Threading.Timer _timer;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
    private int numberOfSeconds = 0;
    private int maxNumberOfSeconds = 600;

    public IHubContext<FoodHub> FoodHub { get; set; }

    public KitchenTimer(IHubContext<FoodHub> foodHub)
    {
        FoodHub = foodHub ?? throw new ArgumentNullException(nameof(foodHub));
    }

    public async Task StartTimer()
    {
        await BroadcastTimerStarted();
        _timer = new System.Threading.Timer(UpdateTimer, null, _interval, _interval);
    }

    public async Task StopTimer()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        await BroadcastTimerEnded();
    }

#region Private Methods

private async void UpdateTimer(object state)
{
    if (numberOfSeconds <= maxNumberOfSeconds)
    {
        numberOfSeconds += 1;
    }
    else
    {
        await BroadcastTimerEnded();
        await _timer.DisposeAsync();
    }
}

    private async Task BroadcastTimeChanged()
    {
        await FoodHub.Clients.All.SendAsync("TimeChanged", numberOfSeconds.ToString());
    }

    private async Task BroadcastTimerStarted()
    {
        await FoodHub.Clients.All.SendAsync("TimeStarted", CancellationToken.None);
    }

    private async Task BroadcastTimerEnded()
    {
        await FoodHub.Clients.All.SendAsync("TimeEnded", CancellationToken.None);
    }
    
    #endregion
    
}