using FoodOrdering.Data;
using FoodOrdering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FoodItemsController : ControllerBase
{
    private readonly DataContext _context;
    
    public FoodItemsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<FoodList>> GetFoodItems()
    {
        var foodItems = await _context.FoodItems.ToListAsync();
        return Ok(foodItems);
    }
}