using Microsoft.AspNetCore.Mvc;

namespace Gingerbread;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Order> _orders = new();

    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.0, Stock = 10 },
        new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.0, Stock = 20 },
        new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.0, Stock = 30 }
    };

    [HttpPost]
    public IActionResult AddProduct(ProductRequest productRequest)
    {
        var product = new Product
        {
            Id = _products.Count + 1,
            Name = productRequest.Name,
            Description = productRequest.Description,
            Stock = productRequest.Stock,
            Price = productRequest.Price
        };

        _products.Add(product);

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(_products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _products.Remove(product);

        return Ok();
    }

    [HttpPost("orders")]
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
            Id = _orders.Count + 1,
            ProductId = product.Id,
            Quantity = orderRequest.Quantity,
            OrderDate = DateTime.Now
        };

        _orders.Add(order);

        return Ok();
    }

    [HttpGet("orders")]
    public IActionResult GetOrders(int? productId)
    {
        IEnumerable<Order> orders = _orders;

        if (productId.HasValue)
        {
            orders = orders.Where(o => o.ProductId == productId.Value);
        }

        return Ok(orders);
    }

    [HttpGet("orders/{id}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpDelete("orders/{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var product = _products.First(p => p.Id == order.ProductId);
        product.Stock++;

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
    public int Stock { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class ProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
}