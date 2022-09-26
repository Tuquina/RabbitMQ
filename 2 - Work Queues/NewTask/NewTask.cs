using RabbitMQ.Client;
using System.Text;

class NewTask
{
    public static void Main(string[] args)
    {
        // Primero nos conectamos al servidor de RabbitMQ
        var factory = new ConnectionFactory() { HostName = "localhost" };
        // Usamos un using para asegurarnos que luego la conexión será borrada
        using(var connection = factory.CreateConnection())
        // Creamos un canal de conexión para enviar los mensajes
        using(var channel = connection.CreateModel())
        {

            // // Los mensajes se van a procesar en la cola de forma secuencial
            // channel.QueueDeclare(queue: "hello",
            //                      durable: false,  
            //                      exclusive: false,
            //                      autoDelete: false,
            //                      arguments: null);
            // // Si RabbitMQ no encuentra la cola, entonces la crea

            // RabbitMQ no permite redefinir una cola existente con distintos parámetros
            // Por eso declaramos una cola nueva
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,    // Para que la cola sobreviva a un reinicio de servidor de RabbitMQ
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = GetMessage(args);
            // El mensaje va a ser siempre un array de bytes
            var body = Encoding.UTF8.GetBytes(message); // Transformamos el mensaje a bytes

            // Marcamos nuestros mensajes como persistentes
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            // Marcar mensajes como persistentes no garantiza que no se perderán
            // Para más garantía usar publisher confirms

            // Transmitimos el mensaje por el canal
            channel.BasicPublish(exchange: "",  
                                 routingKey: "task_queue",   // Es el nombre de la Queue
                                 basicProperties: null,
                                 body: body);   // Enviamos el body que codificamos
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return((args.Length > 0)
        ? string.Join("",args)
        : "info: Se envió el mensaje");
    }
}
