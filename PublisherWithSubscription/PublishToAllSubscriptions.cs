namespace ServiceBus.PublisherWithSubscription;

public static class PublishToAllSubscriptions
{
	public static async Task RunPublishToAllSubscriptions(string serviceBusEndpoint,string topic)
	{
		const int numOfMessages = 200;
		var client = new ServiceBusClient(serviceBusEndpoint);
		var sender = client.CreateSender(topic);

		var message = new ServiceBusMessage($"Message to By Correction Subscription")
		{
			CorrelationId = "id1",
			// MessageId = "msgid1",
			To = "xyz",
			ReplyTo = "someQueue",
			Subject = "subject1",	
			SessionId =	"session1",
			ReplyToSessionId = "sessionId",			
			ContentType = "application/text"
		};
		await sender.SendMessageAsync(message);

		var ByApplicationProperties = new ServiceBusMessage($"Message to By Properties Subscription");
		ByApplicationProperties.ApplicationProperties.Add("prop1", "value1");
		await sender.SendMessageAsync(ByApplicationProperties);

		var SqlFilter = new ServiceBusMessage($"Message to SqlFilter subscription")
		{
			ReplyTo = "Prometeo Adonis Eris Atenea"
		};
		SqlFilter.ApplicationProperties.Add("userProp1", "value1");
		await sender.SendMessageAsync(SqlFilter);


		using var messageBatch = await sender.CreateMessageBatchAsync();

		for (var i = 1; i <= numOfMessages; i++)
		{
			if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message to NoFilter subscription - {i}")))
			{
					throw new Exception($"The message {i} is too large to fit in the batch.");
			}
		}

		try
		{
			await sender.SendMessagesAsync(messageBatch);
			Console.WriteLine($"A batch of {numOfMessages} messages has been published to topic {topic} for NoFilter subscription.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred while publishing messages: {ex.Message}");
		}

		await sender.DisposeAsync();
		await client.DisposeAsync();
	}
}
