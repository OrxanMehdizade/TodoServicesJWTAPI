namespace TodoServicesJWTAPI.Models.DTOs.Todo
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedTime { get; set; }
        public TodoItemDto(int id, string text, bool isCompleted, DateTime createdTime )
        {
            Id = id;
            Text = text;
            IsCompleted = isCompleted;
            CreatedTime = createdTime;
        }
    }
}
