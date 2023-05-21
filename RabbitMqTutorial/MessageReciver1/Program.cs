using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672"); //This is the connection string, it shouldn't be put directly into code but into a secrets file.
factory.ClientProvidedName = "RabbitMQ Reciver 1 Demo App"; //Uniek identefier of this console app

//Interfaces for interacting with the message queue
IConnection connection = factory.CreateConnection();
IModel channel = connection.CreateModel();

//Define the echange and queue names
string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

//Bind the correct exchange to the correct queue
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false); //0 implies we don't care about the message size, 1 implies the amount of messages you want to recive at once, false implies we want this settings to this instance and not globaly

//Defines consuming the message and how it will be handled
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Received: {message}");

    channel.BasicAck(args.DeliveryTag, false); //Tells the RabbitMQ server that the event was correctly consumed and can be removed
};

//shuts down the consumer correctly
string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine(); //Prevents application from closing so that the listener can actualy listen to arriving messages

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();