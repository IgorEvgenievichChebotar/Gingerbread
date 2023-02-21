using Microsoft.AspNetCore.Mvc;

namespace Gingerbread;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Order> _orders = new();

    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.0 },
        new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.0 },
        new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.0 }
    };

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(_products);
    }

    [HttpPost("order")]
    public IActionResult OrderProduct(OrderRequest orderRequest)
    {
        var product = _products.Find(p => p.Id == orderRequest.ProductId);

        if (product == null)
        {
            return NotFound();
        }

        if (product.Stock < orderRequest.Quantity)
        {
            return BadRequest("Not enough stock available");
        }

        product.Stock -= orderRequest.Quantity;

        var order = new Order
        {
            ProductId = product.Id,
            Quantity = orderRequest.Quantity
        };

        _orders.Add(order);

        return Ok();
    }

    [HttpDelete("order/{orderId}")]
    public IActionResult DeleteOrder(int orderId)
    {
        var order = _orders.Find(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        var product = _products.First(p => p.Id == order.ProductId);

        product.Stock += order.Quantity;

        _orders.Remove(order);

        return Ok();
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; } = 10;
}

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}