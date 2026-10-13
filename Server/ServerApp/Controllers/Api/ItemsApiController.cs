using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Api;

namespace ServerApp.Controllers.Api;

/// <summary>REST API: read-only access to items.</summary>
[ApiController]
[Route("api/items")]
[Authorize]
public class ItemsApiController(ItemService items) : ControllerBase
{
    /// <summary>List of items, optionally filtered by name or category.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll([FromQuery] string? search)
    {
        var list = await items.GetAllAsync(search);
        return Ok(list.Select(ToDto));
    }

    /// <summary>One item by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> GetById(int id)
    {
        var item = await items.GetAsync(id);
        if (item is null) return NotFound();

        return Ok(ToDto(item));
    }

    private static ItemDto ToDto(Factorio.Domain.Entities.Item i) =>
        new(i.Id, i.InternalName, i.Name, i.Category, i.Description,
            i.StackSize, i.CraftTime, i.ProductAmount);
}
