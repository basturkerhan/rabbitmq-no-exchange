using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.subscriber // consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var uri = Environment.GetEnvironmentVariable("URI", EnvironmentVariableTarget.Process);
            var factory = new ConnectionFactory();
            factory.Uri = new Uri( (uri);

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.QueueDeclare("hello-queue", true, false, false); // yeni kuyruk oluşturma
            // publisher ın kuyruğu oluşturduğundan kesin eminsek üstteki satırı yazmaya gerek yok
            // ama eğer oluşturmamışsa hata alırız. Almamak için burada da kuyruk oluşturulabilir

            channel.BasicQos(0, 1, false); // mesajlar kuyruktan kaçar kaçar gelecek? videodan parametreleri dinle
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", false, consumer);
            // ikinci parametre true verilirse rabbitmq mesaj yanlış da alınsa doğru da alınsa kuyruktan siler
            // false yaparsak ben sana mesajı doğru gönderdiğini haber edeceğim o zaman sil diyoruz

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(500);
                Console.WriteLine("Kuyruktan Gelen Mesaj: " + message);

                channel.BasicAck(e.DeliveryTag, false); // mesajı artık kuyruktan silebilirsin diye belirttik (yukarıyı false yaptığımız için)
            };

            Console.ReadLine();
        }

    }
}
