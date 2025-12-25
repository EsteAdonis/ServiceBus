namespace ServiceBus.Filters.Consumer;

public static class ConsumerCorrelation
{
	public static async Task ConsumerWithCorrection(string serviceBusEndpoint, string topic, string subscription)
	{
		var client = new ServiceBusClient(serviceBusEndpoint);
		var receiver = client.CreateReceiver(topic, subscription, new ServiceBusReceiverOptions());

		var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));
		if (message != null)
		{
			Console.WriteLine($"Message received from {topic} - {subscription}!\n");
			
			// Inspect message properties before processing
			Console.WriteLine($"Body: {message.Body}");
			Console.WriteLine($"ContentType: {message.ContentType}");
			Console.WriteLine($"CorrelationId: {message.CorrelationId}");
			Console.WriteLine($"Subject: {message.Subject}");
			Console.WriteLine($"MessageId: {message.MessageId}");
			Console.WriteLine($"ReplyTo: {message.ReplyTo}");
			Console.WriteLine($"ReplyToSessionId: {message.ReplyToSessionId}");
			Console.WriteLine($"SessionId: {message.SessionId}");
			Console.WriteLine($"To: {message.To}");
			await receiver.CompleteMessageAsync(message);
		}
		await receiver.DisposeAsync();
		await client.DisposeAsync();
	}
}