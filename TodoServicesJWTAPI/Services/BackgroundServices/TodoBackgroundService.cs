using MimeKit;
using MailKit.Net.Smtp;
using TodoServicesJWTAPI.Data;


namespace TodoServicesJWTAPI.Services.BackgroundServices
{
    public class TodoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public TodoBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                        var tasks = dbContext.TodoItems.ToList();

                        foreach (var task in tasks)
                        {
                            string subject = "Task Deadline Reminder";
                            string message = $"The deadline for task '{task.Text}' is tomorrow. Please complete it on time.";
                            await SendEmailAsync(dbContext.Users.FirstOrDefault(u => u.Id == task.UserId)?.Email, subject, message);
                        }
                    }
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
            }
        }

        public async Task SendEmailAsync(string? email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Your Name", "mehdizadeorxan2000@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("Recipient", email)); 
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("plain") { Text = message };

                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587,true);
                client.Authenticate("mehdizadeorxan2000@gmail.com", "aekuvdvvjfgoofpw");
                client.Send(emailMessage);
                client.Disconnect(true);

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
