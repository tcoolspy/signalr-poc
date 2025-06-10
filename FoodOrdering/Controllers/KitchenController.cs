using FoodOrdering.Data;
using FoodOrdering.Hubs;
using FoodOrdering.Hubs.Extensions;
using FoodOrdering.Hubs.Interfaces;
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
    private IHubContext<FoodHub, IFoodOrderClient> _hubContext;

    public KitchenController(DataContext context, IHubContext<FoodHub> foodHub, IHubContext<FoodHub, IFoodOrderClient> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public List<Order> GetExistingOrders()
    {
        var orders = _context.Orders.Include(x => x.FoodItem).Where(x => x.OrderState != OrderState.Completed);
        return orders.ToList();
    }
}