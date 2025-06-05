using FoodOrdering.Data;
using FoodOrdering.Hubs;
using FoodOrdering.Hubs.Models.Timer;
using FoodOrdering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class KitchenController : ControllerBase
{
    private readonly DataContext _context;
    private KitchenTimer KitchenTimer;

    public KitchenController(DataContext context, IHubContext<FoodHub> foodHub)
    {
        _context = context;
        KitchenTimer = new KitchenTimer(foodHub);
    }

    [HttpGet]
    public List<Order> GetExistingOrders()
    {
        var orders = _context.Orders.Include(x => x.FoodItem).Where(x => x.OrderState != OrderState.Completed);
        return orders.ToList();
    }
}