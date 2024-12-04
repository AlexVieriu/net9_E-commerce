using System.Net.Cache;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> productRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        var products = await productRepo.GetAllAsync(spec);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await productRepo.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        productRepo.Add(product);
        if (await productRepo.SaveAllAsync())
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        else
            return BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        var exists = await productRepo.ExistsAsync(id);
        if (id != product.Id || !exists)
            return BadRequest("Can't update product");

        productRepo.Update(product);

        if (await productRepo.SaveAllAsync())
            return Ok(product);
        else
            return BadRequest("Problem updating the product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await productRepo.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        productRepo.Delete(product);
        if (await productRepo.SaveAllAsync())
            return NoContent();
        else
            return BadRequest();
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();

        return Ok(await productRepo.GetAllAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();

        return Ok(await productRepo.GetAllAsync(spec));
    }
}