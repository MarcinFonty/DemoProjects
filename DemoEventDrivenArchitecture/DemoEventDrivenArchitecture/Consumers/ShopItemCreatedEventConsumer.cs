using DemoEventDrivenArchitecture.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace DemoEventDrivenArchitecture.Consumers
{
    public class ShopItemCreatedEventConsumer : AsyncEventingBasicConsumer /*DefaultBasicConsumer*/
    {
        public ShopItemCreatedEventConsumer(IModel model) : base(model)
        {

        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var json = Encoding.UTF8.GetString(body.ToArray());
            var shopItemCreatedEvent = JsonSerializer.Deserialize<ShopItemCreatedEvent>(json);

            // TODO: Handle the received message here
            Console.WriteLine($"Received message: {json}");

            Model.BasicAck(deliveryTag, false);
        }

        //private readonly IModel _channel;

        //public ShopItemCreatedEventConsumer(IModel channel)
        //{
        //    _channel = channel;
        //}

        //public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        //{
        //    var json = Encoding.UTF8.GetString(body.ToArray());
        //    var shopItemCreatedEvent = JsonSerializer.Deserialize<ShopItemCreatedEvent>(json);

        //    // TODO: Handle the received message here
        //    throw new NotImplementedException(json);
        //}
    }
}
