namespace ServiceBus.Filters.Consumer;

public class ConsumerUsingProcessor
{
	public static async Task ConsumingUsingProcessor(string serviceBusEndpoint, string topicName, string subscriptionName)
	{
		Console.WriteLine($"Processing {topicName} - {subscriptionName}");
		var client = new ServiceBusClient(serviceBusEndpoint);
		ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());
		try {
			processor.ProcessMessageAsync += MessageHandler;
			processor.ProcessErrorAsync += ErrorHandler;

			await processor.StartProcessingAsync();
			
			// Console.WriteLine("Wait for a minute and then press any key to end the process");
			Thread.Sleep(15000);

			// Console.WriteLine("\nStopping the receiver...");
			await processor.StopProcessingAsync();
			//Console.WriteLine("Stopped receiving Messages");

		}
		catch (Exception ex)
		{
				Console.WriteLine($"Exception: {ex.Message}");
		}		
		finally {
			await processor.DisposeAsync();
			await client.DisposeAsync();
		}

		async Task MessageHandler(ProcessMessageEventArgs args)
		{
			var body = args.Message.Body.ToString();
			var message = args.Message;
			Console.WriteLine($"Consuming data from {topicName} - {subscriptionName} : {body}");
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

			await args.CompleteMessageAsync(args.Message);
		}

		Task ErrorHandler(ProcessErrorEventArgs args)
		{
			Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		}
		await client.DisposeAsync();
		await processor.DisposeAsync();		
	}		
}
