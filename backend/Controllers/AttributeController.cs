using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Services.AttributeLibraryServices;

namespace server.Controllers;

[Authorize]
[ApiController]
[Route("api/attributes")]
public class AttributeController(IAttributeService attributeService) : ControllerBase
{
    [HttpGet("types-and-categories")]
    public async Task<IActionResult> GetAttributeTypesAndCategories()
    {
        var categories = await attributeService.GetCategoriesAsync();
        var types = await attributeService.GetAttributeTypesAsync();
        return Ok(new
        {
            Categories = categories,
            Types = types
        });
    }
}
