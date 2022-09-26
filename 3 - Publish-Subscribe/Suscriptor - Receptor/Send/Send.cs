using System;
using RabbitMQ.Client;
using System.Text;

class Send
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

            // El Exchange envía el mensaje a todos los que están escuchando
            channel.ExchangeDeclare(exchange:"logs", type:ExchangeType.Fanout);
            

            string message = GetMessage(args);
            // El mensaje va a ser siempre un array de bytes
            var body = Encoding.UTF8.GetBytes(message); // Transformamos el mensaje a bytes

            // Transmitimos el mensaje por el canal
            channel.BasicPublish(exchange: "logs",  
                                 routingKey: "",   // El exchange se encarga del routingKey
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