using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Text;

class Worker
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
            // channel.QueueDeclare(queue: "hello",
            //                      durable: false, 
            //                      autoDelete: false,
            //                      arguments: null);

            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,    // Para que la cola sobreviva a un reinicio de servidor de RabbitMQ
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Esto es para que Rabbit no envíe un mensaje a un trabajador hasta que haya
            // procesado y reconocido el anterior.
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages");

            // Creamos el evento                     
            var consumer = new EventingBasicConsumer(channel);

            // Cuando consumer.Received sea invocado:
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Array de bytes
                var message = Encoding.UTF8.GetString(body); // Decodificamos el mensaje
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                // Simulamos una tarea falsa
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");
                
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); 
                // Los ACK deben enviarse por el mismo canal que recibió la entrega
            };

            // Consumimos la queue
            channel.BasicConsume(queue: "task_queue",
                                 autoAck: false, //Activamos el reconocimiento de mensajes de forma MANUAL
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
