using LearningEventsSourcingDemo.DTO_s;
using LearningEventsSourcingDemo.Entities;
using LearningEventsSourcingDemo.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace LearningEventsSourcingDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemController : ControllerBase
    {
        private readonly IConnection _connection;

        public ShopItemController(IConnection connection)
        {
            _connection = connection;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShopItem([FromBody] ShopItemDto shopItemDto)
        {
            // Code to create the shop item
            var shopItem = new ShopItem
            {
                Title = shopItemDto.Title,
                Description = shopItemDto.Description,
                CreatedBy = shopItemDto.CreatedBy
            };

            // Publish an event indicating that a new shop item has been created
            var shopItemCreatedEvent = new ShopItemCreatedEvent(shopItem);
            var channel = _connection.CreateModel();
            var exchangeName = "shopitem-created-exchange";
            var routingKey = "shopitem.created";
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(shopItemCreatedEvent));
            channel.BasicPublish(exchangeName, routingKey, null, body);

            // Return a success response to the client
            return Ok();
        }
    }
}
