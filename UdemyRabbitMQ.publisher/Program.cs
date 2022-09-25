using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var uri = Environment.GetEnvironmentVariable("URI", EnvironmentVariableTarget.Process);
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(uri);

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.QueueDeclare("hello-queue", true, false, false); // yeni kuyruk oluşturma

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Message: {x}";
                var messageBody = Encoding.UTF8.GetBytes(message); // kuyruğa bayt olarak gönderilir

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
                // henüz exchange kullanmıyoruz ondan string.Empty yolladık
                // ayrıca exchange kullanmıyorsak ikinci parametre olarak kuyruğun ismi yollanır

                Console.WriteLine($"Mesaj Gönderildi: {x}");
            });
  
            Console.ReadLine();
        }
    }
}
