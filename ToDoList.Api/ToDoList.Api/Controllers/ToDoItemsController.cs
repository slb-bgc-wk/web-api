using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    //[Authorize]
    [AllowAnonymous]

    public class ToDoItemsController : ControllerBase
    {

        private readonly IToDoItemService _toDoItemService;

        public ToDoItemsController(IToDoItemService toDoItemService)
        {
            _toDoItemService = toDoItemService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ToDoItemDto>), 200)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Get All",
            Description ="Get All ToDo Items"
            )]
        public async Task<ActionResult<List<ToDoItemDto>>> GetAsync()
        {
            var result = await _toDoItemService.GetAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(404)]
        //[AllowAnonymous]
        public async Task<ActionResult<ToDoItemDto>> GetAsync(string id)
        {
            var result = await _toDoItemService.GetAsync(id);
            if (result == null)
            {
                return NotFound($"The item with id {id} does not exist.");
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ToDoItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ToDoItemDto>> PostAsync([FromBody] ToDoItemCreateRequest toDoItemCreateRequest)
        {
            var toDoItemDto = new ToDoItemDto
            {
                Description = toDoItemCreateRequest.Description,
                Done = toDoItemCreateRequest.Done,
                Favorite = toDoItemCreateRequest.Favorite,
            };
            await _toDoItemService.CreateAsync(toDoItemDto);
            return Created("", toDoItemDto);
        }

        // PUT api/<ToDoItemsController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ToDoItemDto), 200)]
        [ProducesResponseType(typeof(ToDoItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ToDoItemDto>> PutAsync(string id, [FromBody] ToDoItemDto toDoItemDto)
        {
            bool isCreate = false;
            var existingItem = await _toDoItemService.GetAsync(id);
            if (existingItem is null)
            {
                isCreate = true;
                await _toDoItemService.CreateAsync(toDoItemDto);
            }
            else
            {
                await _toDoItemService.ReplaceAsync(id, toDoItemDto);
            }

            return isCreate ? Created("", toDoItemDto) : Ok(toDoItemDto);
        }

        // DELETE api/<ToDoItemsController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var isSuccessful = await _toDoItemService.RemoveAsync(id);
            if (!isSuccessful)
            {
                return NotFound($"The item with id {id} does not exist.");
            }
            return NoContent();
        }
    }
}
