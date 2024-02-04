using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Providers;

namespace TodoServicesJWTAPI.Services.Todo
{
    public interface ITodoService
    {
        Task<TodoItemDto?> GetTodoItem(int id, UserInfo info);
        Task<TodoItemDto> CreateTodo(CreateTodoItemRequest request, UserInfo info);
        Task<TodoItemDto> ChangeTodoItemStatus(int id, bool isCompleted, UserInfo info);
        Task<bool> DeleteTodo(int id, UserInfo info);
        Task<PagintionListDto<TodoItemDto>> GetAll(int page, int pageSize, bool? isComleted, UserInfo info);
    }
}
