using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Kafka;

namespace Scania.Kafka.Tool.Cli.Topic
{

    [Command("list", Description = "List add topics",
            AllowArgumentSeparator = true,
            ThrowOnUnexpectedArgument = false)]
    public class TopicListCommand
    {
        [Option("--filter|-f", Description = "Text that should be included in a topic name")]
        public string Filter { get; }

        private async Task<int> OnExecute(IConsole console)
        {
            try
            {
                var topics = KafkaClient.ListTopics(Filter);
                foreach (var topic in topics)
                {
                    console.WriteLine(topic);
                }

                return await Task.FromResult(0);
            }
            catch (Exception e)
            {
                console.WriteLine($"An error occured listing topics: {e.Message}");
                return await Task.FromResult(1);
            }
        }
    }
}