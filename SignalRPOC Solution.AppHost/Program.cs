var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FoodOrdering>("foodordering");

builder.Build().Run();
