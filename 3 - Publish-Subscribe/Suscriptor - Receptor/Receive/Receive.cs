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
            channel.ExchangeDeclare(exchange:"logs", type:ExchangeType.Fanout);
            // Hasta ahora no tenemos ninguna cola, pero vamos a usar una sola
            // Creamos una cola dinámica (para cada cliente es diferente)
            var queuName = channel.QueueDeclare().QueueName;
            
            // Podríamos asociar más de una cola, un exchange soporta múltiples colas

            // Asociamos la cola con el exchange
            channel.QueueBind(queue: queuName, exchange: "logs", routingKey:"");

            Console.WriteLine("Waiting for logs...");

            // Creamos el evento                     
            var consumer = new EventingBasicConsumer(channel);

            // Cuando consumer.Received sea invocado:
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Array de bytes
                var message = Encoding.UTF8.GetString(body); // Decodificamos el mensaje y transformamos en string
                Console.WriteLine(" [x] Received {0}", message);
            };

            // Consumimos la queue
            channel.BasicConsume(queue: queuName,
                                 autoAck: true, //Auto ACK
                                // El sender será informado de que el mensaje fue recibido
                                // a nivel servidor
                                 consumer: consumer);   // Enviamos el consumer

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
