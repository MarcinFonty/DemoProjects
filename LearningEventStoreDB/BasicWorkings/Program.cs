using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BasicWorkings.Events;
using EventStore.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace quick_start
{
    class InventoryItemInfo
    {
        public int Quantity { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            #region create client
            var settings = EventStoreClientSettings
                .Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
            var client = new EventStoreClient(settings);
            #endregion
            #region events
            var addEventG = new InventoryItemAddedEvent
            {
                ItemId = 1,
                Name = "Gloves",
                Quantity = 10,
                UpdateDate = DateTime.Now
            };

            var removeEventG = new InventoryItemRemovedEvent
            {
                ItemId = 1,
                Name = "Gloves",
                Quantity = 5,
                UpdateDate = DateTime.Now
            };

            var addEventH = new InventoryItemAddedEvent
            {
                ItemId = 2,
                Name = "Hats",
                Quantity = 7,
                UpdateDate = DateTime.Now
            };

            var removeEventH = new InventoryItemRemovedEvent
            {
                ItemId = 2,
                Name = "Hats",
                Quantity = 2,
                UpdateDate = DateTime.Now
            };
            #endregion
            #region operations
            bool resultChosen = false;

            while (!resultChosen)
            {
                Console.WriteLine("Enter a command ('addG', 'addH', 'removeG', 'removeH', 'result'): ");
                string command = Console.ReadLine();

                switch (command)
                {
                    case "addG":
                        Console.WriteLine("Add command selected.");
                        AppendEvent(client, addEventG);
                        break;
                    case "removeG":
                        Console.WriteLine("Remove command selected.");
                        AppendEvent(client, removeEventG);
                        break;
                    case "addH":
                        Console.WriteLine("Add command selected.");
                        AppendEvent(client, addEventH);
                        break;
                    case "removeH":
                        Console.WriteLine("Remove command selected.");
                        AppendEvent(client, removeEventH);
                        break;
                    case "result":
                        Console.WriteLine("Result command selected.");
                        resultChosen = true; // Set the flag to exit the loop
                        ReturnInventoryState(client);
                        break;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }

            Console.ReadKey();
            #endregion
        }

        public static async Task AppendEvent(EventStoreClient client, object @event)
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                @event.GetType().Name,
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            #region appendEvents
            await client.AppendToStreamAsync(
                "inventory-stream",
                StreamState.Any,
                new[] { eventData }
                //cancellationToken: cancellationToken
            );
            #endregion appendEvents
        }

        public static async Task ReturnInventoryState(EventStoreClient client)
        {
            var result = client.ReadStreamAsync(
                Direction.Forwards,
                "inventory-stream",
                StreamPosition.Start);

            var events = await result.ToListAsync();

            var inventoryState = new Dictionary<int, InventoryItemInfo>(); // Dictionary to hold the inventory state

            foreach (var resolvedEvent in events)
            {
                var eventBytes = resolvedEvent.Event.Data.Span.ToArray();
                var eventJson = Encoding.UTF8.GetString(eventBytes);

                // Deserialize the event JSON to determine the event type
                var eventType = resolvedEvent.Event.EventType;

                // Apply the event to the inventory state
                switch (eventType)
                {
                    case "InventoryItemAddedEvent":
                        var addedEvent = JsonConvert.DeserializeObject<InventoryItemAddedEvent>(eventJson);
                        if (!inventoryState.ContainsKey(addedEvent.ItemId))
                        {
                            inventoryState[addedEvent.ItemId] = new InventoryItemInfo
                            {
                                Quantity = 0,
                                Name = addedEvent.Name
                            };
                        }
                        inventoryState[addedEvent.ItemId].Quantity += addedEvent.Quantity;
                        break;
                    case "InventoryItemRemovedEvent":
                        var removedEvent = JsonConvert.DeserializeObject<InventoryItemRemovedEvent>(eventJson);
                        if (inventoryState.ContainsKey(removedEvent.ItemId))
                        {
                            inventoryState[removedEvent.ItemId].Quantity -= removedEvent.Quantity;
                            if (inventoryState[removedEvent.ItemId].Quantity < 0)
                                inventoryState[removedEvent.ItemId].Quantity = 0;
                        }
                        break;
                    // Add cases for other event types if needed

                    default:
                        // Handle unknown event type or ignore it
                        break;
                }
            }

            // Print the current inventory state
            Console.WriteLine("Current Inventory State:");
            foreach (var item in inventoryState)
            {
                Console.WriteLine($"{item.Key}: {item.Value.Quantity} - {item.Value.Name}");
            }
        }

    }
}