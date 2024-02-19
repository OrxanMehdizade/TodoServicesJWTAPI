namespace TodoServicesJWTAPI.Services.BackgroundServices
{
    public class TodoBackgroundService : IHostedService
    {
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("background Service Started");
            return Task.CompletedTask;
        }



        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("background Service Stoped");
            return Task.CompletedTask;
        }
    }
}
