using TodoServicesJWTAPI.Services.Todo;

namespace TodoServicesJWTAPI.Services.BackgroundServices
{
    public class TodoBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public TodoBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Background Service Started");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public async void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var todoService = scope.ServiceProvider.GetRequiredService<TodoService>();
                await todoService.CheckDeadlineAndSendEmailsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Background Service Stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
