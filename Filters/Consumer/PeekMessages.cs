namespace ServiceBus.Filters.Consumer;

public class PeekMessages
{
	public static async Task PeekMessagesAsync(string serviceBusEndpoint,string topic, string subscription, int MaxMessages)
	{
		var client = new ServiceBusClient(serviceBusEndpoint);
		var receiver = client.CreateReceiver(topic, subscription);

		var messages = await receiver.PeekMessagesAsync(maxMessages: MaxMessages);
		foreach (var message in messages)
		{
			Console.WriteLine($"Peeked Message from {topic} - {subscription}:\n");
			Console.WriteLine($"Body: {message.Body}");
			Console.WriteLine($"MessageId: {message.MessageId}");
			Console.WriteLine($"To: {message.To}\n");
		}

		await receiver.DisposeAsync();
		await client.DisposeAsync();
	}
}
