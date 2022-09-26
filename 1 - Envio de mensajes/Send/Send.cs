using System;
using RabbitMQ.Client;
using System.Text;

class Send
{
    public static void Main()
    {
        // Primero nos conectamos al servidor de RabbitMQ
        var factory = new ConnectionFactory() { HostName = "localhost" };
        // Usamos un using para asegurarnos que luego la conexión será borrada
        using(var connection = factory.CreateConnection())
        // Creamos un canal de conexión para enviar los mensajes
        using(var channel = connection.CreateModel())
        {
            // Los mensajes se van a procesar en la cola de forma secuencial
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            // Si RabbitMQ no encuentra la cola, entonces la crea

            string message = "Prueba de mensajes";
            // El mensaje va a ser siempre un array de bytes
            var body = Encoding.UTF8.GetBytes(message); // Transformamos el mensaje a bytes

            // Transmitimos el mensaje por el canal
            channel.BasicPublish(exchange: "",  
                                 routingKey: "hello",   // Es el nombre de la Queue
                                 basicProperties: null,
                                 body: body);   // Enviamos el body que codificamos
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}