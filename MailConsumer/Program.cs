using System;
using System.Text;
using System.Text.Json;
using MailConsumer.Configurations;
using MailConsumer.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using MimeKit;
using System.Threading.Tasks;
using MailKit.Security;

string FILE_NAME = @"C:\Users\Mehdi_ha01\source\repos\TodoServicesJWTAPI\TodoServicesJWTAPI\MailConsumer\config.json";
var builder = new ConfigurationBuilder().AddJsonFile(FILE_NAME).Build();

var rabbitMQConfig = new RabbitMQConfiguration();
builder.Bind("RabbitMQ", rabbitMQConfig);




var factory = new ConnectionFactory
{
    HostName = rabbitMQConfig.HostName,
    UserName = rabbitMQConfig.UserName,
    Password = rabbitMQConfig.Password,
    Port = rabbitMQConfig.Port,
};


using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: rabbitMQConfig.QueueName,
                     exclusive: false,
                     durable: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var registerRequest = JsonSerializer.Deserialize<RegisterRequest>(message);

    SendEmail(registerRequest.Email, "Registration Confirmation", "Thank you for registering!");

    Console.WriteLine($"Email sent to {registerRequest.Email}");
};

channel.BasicConsume(queue: rabbitMQConfig.QueueName,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine("Press [enter] to exit");
Console.ReadLine();

async Task SendEmail(string recipientEmail, string subject, string body)
{
    try
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Orxan", "mehdizadeorxan2000@gmail.com"));
        message.To.Add(new MailboxAddress("Recipient", recipientEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        client.Authenticate("mehdizadeorxan2000@gmail.com", "mnjmalhjvloxihcr");
        client.Send(message);
        client.Disconnect(true);

        Console.WriteLine("Email sent successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to send email: {ex.Message}");
    }
}
