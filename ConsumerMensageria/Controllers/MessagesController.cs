using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace ConsumerMensageria.Controllers
{

    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private const string QUEUE_NAME = "messages";
        private readonly ConnectionFactory _factory;

        public MessagesController()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        [HttpGet]
        public IActionResult GetMessage()
        {

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

                }
            }

            return Accepted();
        }

    }
}
