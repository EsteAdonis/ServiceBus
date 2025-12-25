using System.Reflection.Metadata;

namespace ServiceBus.PublisherWithSubscription;

public class CreateTopicsSubscriptionsAndRules
{
	public static async Task RunCreateTopicsSubscriptionsAndRules(string serviceBusEndpoint, string topic)
	{
		string[] subscriptions = ["ByProperties", "ByCorrelation", "BySqlFiler", "NoFilter"];

		var adminClient = new ServiceBusAdministrationClient(serviceBusEndpoint);

		var existsTopic = await adminClient.TopicExistsAsync(topic);
		if (!existsTopic)
		{	
			var options = new CreateTopicOptions(topic)			
			{
				RequiresDuplicateDetection = true,
				DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(5),
				DefaultMessageTimeToLive = TimeSpan.FromDays(7),
				MaxSizeInMegabytes = 4096 // 4GB
			};			
			await adminClient.CreateTopicAsync(options);

			Console.WriteLine($"The topic {topic} has been created");
		}
		else
		{
			Console.WriteLine($"The topic {topic} already exists");
		}

		foreach (var subscription in subscriptions)
		{
			var subscriptionExistsForSub = adminClient.SubscriptionExistsAsync(topic, subscription).GetAwaiter().GetResult();	

			if (!subscriptionExistsForSub)
			{
				var subscriptionOptions = new CreateSubscriptionOptions(topic, subscription)
				{
					LockDuration = TimeSpan.FromMinutes(2),
					DefaultMessageTimeToLive = TimeSpan.FromDays(14),
					MaxDeliveryCount = 50
				};
				// await adminClient.CreateSubscriptionAsync(subscriptionOptions);		

				switch(subscription)
				{
					case "ByProperties":
						if (!adminClient.RuleExistsAsync(topic, subscription, "ByApplicationProperties").GetAwaiter().GetResult())
						{
							var ruleOption = new CreateRuleOptions 
							{
								Name = "ByApplicationProperties",
								Filter = new CorrelationRuleFilter
								{
									ApplicationProperties = { { "prop1", "value1" }	}
								}
							};							
							await adminClient.CreateSubscriptionAsync(subscriptionOptions, ruleOption);		
						}
						break;

					case "ByCorrelation":
						if (!adminClient.RuleExistsAsync(topic, subscription, "ByCorrelationProperties").GetAwaiter().GetResult())
						{
							var ruleOption = new CreateRuleOptions 
							{
								Name = "ByCorrelationProperties",
								Filter = new CorrelationRuleFilter
								{
									CorrelationId = "id1",
									// MessageId = "msgid1",
									To = "xyz",
									ReplyTo = "someQueue",	
									Subject = "subject1",
									SessionId =	"session1",
									ReplyToSessionId = "sessionId",			
									ContentType = "application/text"
								}
							};
							await adminClient.CreateSubscriptionAsync(subscriptionOptions, ruleOption);		
						}
						break;

					case "BySqlFiler":
						if (!adminClient.RuleExistsAsync(topic, subscription, "BySqlFilterProperties").GetAwaiter().GetResult())
						{
							var ruleOption = new CreateRuleOptions 
							{
								Name = "BySqlFilterProperties",
								Filter = new SqlRuleFilter("sys.replyTo like '%Adonis%' AND userProp1 = 'value1'"),
								Action = new SqlRuleAction("SET sys.To = 'Entity'")
							};
							await adminClient.CreateSubscriptionAsync(subscriptionOptions, ruleOption);				
						}
						break;
					case "NoFilter":
						// No filter, all messages will be received	
						await adminClient.CreateSubscriptionAsync(subscriptionOptions);
						break;
				}
			}
		}

	}
}