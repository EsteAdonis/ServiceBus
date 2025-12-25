// Topic filters and actions
// https://learn.microsoft.com/en-us/azure/service-bus-messaging/topic-filters

// Create the topic, subscriptions and rules

const string serviceBusEndpoint = "";
await CreateTopicsSubscriptionsAndRules.RunCreateTopicsSubscriptionsAndRules(serviceBusEndpoint, "GreekGods");
await PublishToAllSubscriptions.RunPublishToAllSubscriptions(serviceBusEndpoint,"GreekGods");


await ConsumerCorrelation.ConsumerWithCorrection(serviceBusEndpoint, "GreekGods", "ByCorrelation");
await ConsumerProperties.ConsumWithProperties(serviceBusEndpoint, "GreekGods", "ByProperties");
await ConsumerSQLFilter.ConsumSQLFilter(serviceBusEndpoint, "GreekGods", "BySqlFiler");
await ConsumerNoFilter.ConsumNoFilter(serviceBusEndpoint, "GreekGods", "NoFilter");
await PeekMessages.PeekMessagesAsync(serviceBusEndpoint, "GreekGods", "NoFilter", MaxMessages: 10);
await ConsumerUsingProcessor.ConsumingUsingProcessor(serviceBusEndpoint, "GreekGods", "NoFilter");

