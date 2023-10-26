using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Controllers;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.UnitTests
{
    public class ToDoItemsControllerTest
    {
        [Fact]
        public async Task CreateItem()
        {
            var sut = new ToDoItemsController(new InMemoryToDoItemService());
            var id = Guid.NewGuid().ToString();
            var toDoItem = new ToDoItemDto
            {
                Id = id,
                Description = "Test",
                CreatedTime = DateTime.UtcNow,
                Done = false
            };

            var creationActionResult = await sut.PutAsync(id, toDoItem);
            
            
            Assert.IsType<CreatedResult>(creationActionResult.Result);
            var createdResult = creationActionResult.Result as CreatedResult;
            Assert.Equivalent(toDoItem, createdResult.Value);

            var getActionResult = await sut.GetAsync(id);
            Assert.IsType<OkObjectResult>(getActionResult.Result);
            var okObjectResult = getActionResult.Result as OkObjectResult;
            Assert.Equivalent(toDoItem, okObjectResult.Value);

            var deleteActionResult = await sut.DeleteAsync(id);
            Assert.IsType<NoContentResult>(deleteActionResult);
            var noContentResult = deleteActionResult as NoContentResult;
            Assert.Equal(204, noContentResult.StatusCode);

            var failedDeleteActionResult = await sut.DeleteAsync(id);
            Assert.IsType<NotFoundObjectResult>(failedDeleteActionResult);
            var notFoundResult = failedDeleteActionResult as NotFoundObjectResult;
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}