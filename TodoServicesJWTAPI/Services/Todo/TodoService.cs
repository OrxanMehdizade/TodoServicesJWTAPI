using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using TodoServicesJWTAPI.Data;
using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Models.Entities;
using TodoServicesJWTAPI.Providers;

namespace TodoServicesJWTAPI.Services.Todo
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;
        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<TodoItemDto> ChangeTodoItemStatus(int id, bool isCompleted, UserInfo info)
        {
            try
            {
                var todoItem = await _context.TodoItems.Where(i => i.UserId == info.id).FirstOrDefaultAsync(e => e.Id == id);
                if (todoItem == null)
                {
                    return null;
                }

                todoItem.IsCompleted = isCompleted;
                await _context.SaveChangesAsync();


                return new TodoItemDto(
                        id: todoItem.Id,
                        text: todoItem.Text,
                        isCompleted: todoItem.IsCompleted,
                        createdTime: todoItem.CreatedTime,
                        deadline: todoItem.Deadline);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ChangeTodoItemStatus:{ex.Message}");
                throw;
            }

        }

        public async Task<TodoItemDto> CreateTodo(CreateTodoItemRequest request, UserInfo info)
        {
            try
            {
                var todoItem = new TodoItem
                {
                    Text = request.Text,
                    IsCompleted = false,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    UserId = info.id
                };

                await _context.TodoItems.AddAsync(todoItem);
                await _context.SaveChangesAsync();
                var Item = await _context.TodoItems.Where(i => i.UserId == info.id).OrderBy(i => i.Id).LastAsync();

                return new TodoItemDto(
                        id: Item.Id,
                        text: Item.Text,
                        isCompleted: Item.IsCompleted,
                        createdTime: Item.CreatedTime,
                        deadline: Item.Deadline);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateTodo:{ex.Message}");
                throw;
            }

        }

        public async Task<bool> DeleteTodo(int id, UserInfo info)
        {
            try
            {
                var todoItem = await _context.TodoItems.Where(i => i.UserId == info.id).FirstOrDefaultAsync(e => e.Id == id);
                if (todoItem == null)
                {
                    return false;
                }

                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteTodo:{ex.Message}");
                throw;
            }


        }

        public async Task<PagintionListDto<TodoItemDto>> GetAll(int page, int pageSize, bool? isComleted, UserInfo info)
        {
            try
            {
                IQueryable<TodoItem> todoQuery = _context.TodoItems.Where(i => i.UserId == info.id).AsQueryable();
                if (isComleted.HasValue)
                {
                    todoQuery = todoQuery.Where(e => e.IsCompleted == isComleted);
                }
                var items = await todoQuery.Skip((page - 1) - pageSize).Take(pageSize).ToListAsync();
                var totalCount = await todoQuery.CountAsync();
                return new PagintionListDto<TodoItemDto>(

                    items.Select(e => new TodoItemDto(
                        id: e.Id,
                        text: e.Text,
                        isCompleted: e.IsCompleted,
                        createdTime: e.CreatedTime,
                        deadline: e.Deadline

                        )),
                    new PagintionMeta(page, pageSize, totalCount)

                    );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll:{ex.Message}");
                throw;
            }
        }


        public async Task<TodoItemDto?> GetTodoItem(int id, UserInfo info)
        {
            try
            {
                var todoItem = await _context.TodoItems.Where(i => i.UserId == info.id).FirstOrDefaultAsync(e => e.Id == id);

                return todoItem is not null
                    ? new TodoItemDto(
                        id: todoItem.Id,
                        text: todoItem.Text,
                        isCompleted: todoItem.IsCompleted,
                        createdTime: todoItem.CreatedTime,
                        deadline: todoItem.Deadline)
                    : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTodoItem:{ex.Message}");
                throw;
            }
        }

        public async Task CheckDeadlineAndSendEmailsAsync()
        {
            try
            {
                var todos = await _context.TodoItems
                    .Where(i => i.Deadline.Date == DateTime.Today.AddDays(1))
                    .ToListAsync();

                foreach (var todo in todos)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == todo.UserId);
                    var userEmail = user.Email;
                    var emailBody = $"Hello {user.UserName},\nYour task's due date is tomorrow ({todo.Deadline.ToShortDateString()}) will expire. Don't forget!";

                    using (var message = new MailMessage())
                    {
                        message.To.Add(userEmail);
                        message.Subject = "Your Task Deadline is Approaching";
                        message.Body = emailBody;

                        using (var client = new SmtpClient("smtp.example.com"))
                        {
                            client.Port = 587;
                            client.Credentials = new NetworkCredential("username", "password");
                            client.EnableSsl = true;
                            await client.SendMailAsync(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckDeadlineAndSendEmailsAsync: {ex.Message}");
                throw;
            }

        }

    }
}
