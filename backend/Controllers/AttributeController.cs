using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using server.Data;
using server.Dto;
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

    [Authorize(Roles = Roles.AdminOrRecruiter)]
    [HttpPost]
    public async Task<IActionResult> CreateAttribute([FromBody] CreateAttributeDto dto)
    {
        var result = await attributeService.CreateAttributeAsync(dto);
        return CreatedAtAction(nameof(GetAttributeTypesAndCategories), new { id = result.Id }, result);
    }

    [Authorize(Roles = Roles.AdminOrRecruiter)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttribute(Guid id, [FromBody] UpdateAttributeDto dto)
    {
        var result = await attributeService.UpdateAttributeAsync(id, dto);
        return Ok(result);
    }

    [Authorize(Roles = Roles.AdminOrRecruiter)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttribute(Guid id)
    {
        await attributeService.DeleteAttributeAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttributeById(Guid id)
    {
        var result = await attributeService.GetAttributeDtoByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAttributes([FromQuery] AttributeSearchQueryDto dto)
    {
        var result = await attributeService.SearchAsync(dto);
        return Ok(result);
    }
}
