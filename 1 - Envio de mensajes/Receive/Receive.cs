using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    public static void Main()
    {
        // Primero nos conectamos al servidor de RabbitMQ a través de la factory
        var factory = new ConnectionFactory() { HostName = "localhost" };
        // Abrimos la conexión
        using (var connection = factory.CreateConnection())
        // Creamos un canal de conexión para recibir los mensajes
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Creamos el evento                     
            var consumer = new EventingBasicConsumer(channel);

            // Cuando consumer.Received sea invocado:
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Array de bytes
                var message = Encoding.UTF8.GetString(body); // Decodificamos el mensaje
                Console.WriteLine(" [x] Received {0}", message);
            };

            // Consumimos la queue
            channel.BasicConsume(queue: "hello",
                                 autoAck: true, //Auto ACK
                                // El sender será informado de que el mensaje fue recibido
                                // a nivel servidor
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
