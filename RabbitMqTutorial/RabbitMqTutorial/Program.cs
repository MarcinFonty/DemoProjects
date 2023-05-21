using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672"); //This is the connection string, it shouldn't be put directly into code but into a secrets file.
factory.ClientProvidedName = "RabbitMQ Sender Demo App"; //Uniek identefier of this console app

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

//Send the message
byte[] messageBodyBytes = Encoding.UTF8.GetBytes("My first message to the queue");
channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

//Close connection after use
channel.Close();
connection.Close();