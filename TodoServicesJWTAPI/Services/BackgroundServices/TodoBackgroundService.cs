using MimeKit;
using MailKit.Net.Smtp;
using TodoServicesJWTAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using MailKit.Security;

namespace TodoServicesJWTAPI.Services.BackgroundServices
{
    public class TodoBackgroundService : IHostedService
    {
        private readonly IServiceProvider _services;
        Timer _timer;
        public TodoBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(Run, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private void Run(object? state)
        {
            var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            var todos = context.TodoItems.Include(t => t.User).Where(todo => todo.Deadline.Date == DateTime.Now.AddDays(1).Date).ToList();
            foreach (var todo in todos)
            {
                Console.WriteLine($"{todo.Text} {todo.User.UserName}");
                SendEmailReminder(todo.User.Email, todo.Text);
            }

        }
        public async Task SendEmailReminder(string email,string taskText)
        {
            try
            {
                var subject = "Task DeadLine Reminder";
                var message = $"The deadline for task '{taskText}' is tomorrow. Please complete it on time.";
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Orxan", "mehdizadeorxan2000@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient",email));
                emailMessage.Subject = taskText;
                emailMessage.Body = new TextPart("plain") { Text = message };

                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("mehdizadeorxan2000@gmail.com", "mnjmalhjvloxihcr");
                await client.SendAsync(emailMessage);
                client.Disconnect(true);


                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            _timer = null;
            return Task.CompletedTask;
        }

        
    }
}
