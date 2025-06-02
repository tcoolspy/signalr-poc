using System.Text.Json.Serialization;

namespace FoodOrdering.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderState
{
    Ordered,
    Preparing,
    AwaitingDelivery,
    Completed
}