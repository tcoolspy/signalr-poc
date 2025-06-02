using FoodOrdering.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<Order> Orders { get; set; }
}