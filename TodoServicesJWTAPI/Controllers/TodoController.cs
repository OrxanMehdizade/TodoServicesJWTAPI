using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Providers;
using TodoServicesJWTAPI.Services.Todo;

namespace TodoServicesJWTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IRequestUserProvider _provider;
        public TodoController(ITodoService todoService, IRequestUserProvider provider)
        {
            _todoService = todoService;
            _provider = provider;
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<TodoItemDto>> Get(int id)
        {
            UserInfo userInfo = _provider.GetUserInfo();
            var item = await _todoService.GetTodoItem(id, userInfo!);


            return item is not null
                ? item
                : NotFound();
        }
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<TodoItemDto>> Delete(int id)
        {
            UserInfo userInfo = _provider.GetUserInfo();
            var item = await (_todoService.DeleteTodo(id, userInfo!));
            return item
                ? NoContent()
                : NotFound();
        }


        [HttpPost("create")]
        public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoItemRequest request)
        {
            UserInfo userInfo = _provider.GetUserInfo();
            var createdItem = await _todoService.CreateTodo(request,userInfo!);

            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        [HttpPost("change-status")]
        public async Task<ActionResult<TodoItemDto>> ChangeStatus(int id, bool isCompleted)
        {
            UserInfo userInfo = _provider.GetUserInfo();
            var updatedItem = await _todoService.ChangeTodoItemStatus(id, isCompleted, userInfo!);

            return updatedItem is not null
                ? updatedItem
                : NotFound();
        }

        [HttpPost("TodoAll")]
        public async Task<PagintionListDto<TodoItemDto>?> All(PagintionRequest request, bool? isCompleted)
        {
            UserInfo userInfo = _provider.GetUserInfo();
            var result = await _todoService.GetAll(request.Page, request.PageSize, isCompleted, userInfo!);
            return result is not null ? result : null;
        }
        [HttpGet("CheckEmails")]
        public async Task<IActionResult> CheckAndSendEmails()
        {
            try
            {
                await _todoService.CheckDeadlineAndSendEmailsAsync();
                return Ok("Emails sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
