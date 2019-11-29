using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Scania.Kafka.Tool.Cli.Config;

namespace Scania.Kafka.Tool.Cli.Kafka
{
    public static class KafkaClient
    {
        public static async Task DeleteTopicAsync(string topicName)
        {
            var config = ConfigService.Get();

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = config.BrokerHost,
                SocketTimeoutMs = config.TimeoutInMs
            })
            .Build())
            {
                try
                {
                    await adminClient.DeleteTopicsAsync(new[] { topicName });
                }
                catch (DeleteTopicsException e)
                {
                    throw new Exception($"{e.Results[0].Error.Reason}");
                }
            }
        }

        public static IEnumerable<string> ListTopics(string filter = "")
        {
            var config = ConfigService.Get();

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = config.BrokerHost,
                SocketTimeoutMs = config.TimeoutInMs
            })
            .Build())
            {
                try
                {
                    var metaData = adminClient.GetMetadata(TimeSpan.FromMilliseconds(config.TimeoutInMs));

                    var topics = metaData.Topics.Select(tm => tm.Topic);
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        topics = topics.Where(t => t.Contains(filter, StringComparison.OrdinalIgnoreCase));
                    }
                    return topics.OrderBy(t => t);
                }
                catch (Exception e)
                {
                    throw new Exception($"{e.Message}");
                }
            }
        }

        public static async Task<string> SendMessageAsync(string topicName, string message)
        {
            var config = ConfigService.Get();

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config.BrokerHost,
                SocketTimeoutMs = config.TimeoutInMs
            };

            try
            {
                using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
                {
                    var result = await p.ProduceAsync(topicName, new Message<Null, string> { Value = message });
                    return result.TopicPartitionOffset.ToString();
                }
            }
            catch (ProduceException<Null, string> e)
            {
                throw new Exception($"{e.Error.Reason}");
            }
        }

        public static void ReceivedMessage(
            string topicName, 
            string consumerGroupId,
            bool commit,
            Action<string, string> messageHandler, 
            Action<string> errorHandler)
        {
            var config = ConfigService.Get();

            var conf = new ConsumerConfig
            {
                GroupId = consumerGroupId,
                BootstrapServers = config.BrokerHost,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = commit
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe(topicName);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);                            
                            messageHandler(cr.TopicPartitionOffset.ToString(), cr.Value);                                                    
                        }
                        catch (ConsumeException e)
                        {
                            errorHandler(e.Error.Reason);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
    }
}