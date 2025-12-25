namespace ServiceBus.Filters.Consumer;

public class ConsumerSQLFilter
{
	public static async Task ConsumSQLFilter(string serviceBusEndpoint, string topic, string subscription)
	{
		var client = new ServiceBusClient(serviceBusEndpoint);
		var receiver = client.CreateReceiver(topic, subscription);

		var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
		if (message != null)
		{
			Console.WriteLine($"Message received from {topic} - {subscription}\n");

			// Inspect message properties
			Console.WriteLine($"Body: {message.Body}");
			Console.WriteLine($"MessageId: {message.MessageId}");
			Console.WriteLine($"userProp1: {message.ApplicationProperties["userProp1"]}");
			Console.WriteLine($"To (modified by Action): {message.To}");
			
			await receiver.CompleteMessageAsync(message); // Mark as processed
		}
		await client.DisposeAsync();
		await receiver.DisposeAsync();
	}
}