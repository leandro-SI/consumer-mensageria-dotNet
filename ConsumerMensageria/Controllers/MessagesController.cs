using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;
using System;

namespace ConsumerMensageria.Controllers
{

    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private const string QUEUE_NAME = "messages";
        private readonly ConnectionFactory _factory;

        public MessagesController()
        {
            _factory = new ConnectionFactory // Definindo uma conexão com um nó RabbitMQ em localhost
            {
                HostName = "localhost"
            };
        }

        [HttpGet]
        public IActionResult GetMessage()
        {

            var msg = "";

            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // Declara a fila a partir da qual irei consumir as mensagens.
                    channel.QueueDeclare(
                            queue: QUEUE_NAME,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                            );

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, args) =>
                    {
                        var body = args.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        msg = message;

                        Console.WriteLine($" [x] Recebida: {message}");
                    };

                    channel.BasicConsume(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

                }
            }

            return Ok(msg);
        }

    }
}
