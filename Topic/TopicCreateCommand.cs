using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Kafka.Tool.Cli.Kafka;

namespace Kafka.Tool.Cli.Topic
{

    [Command("create", Description = "Create Topic"), HelpOption]
    public class TopicCreateCommand
    {
        [Required(ErrorMessage = "You must specify a topic name")]
        [Option("-t|--topic", Description = "Topic name")]
        public string TopicName { get; }
        
        [Option("-p|--partitions", Description = "Number of partitions")]
        public int Partitions { get; } = 1;

        private async Task<int> OnExecute(IConsole console)
        {
            try
            {                
                await KafkaClient.CreateTopicAsync(TopicName, Partitions);
                console.WriteLine($"Topic '{TopicName}' was created.");
                return await Task.FromResult(0);
            }
            catch (Exception e)
            {
                console.WriteLine($"An error occured created topic '{TopicName}': {e.Message}");
                return await Task.FromResult(1);
            }
        }
    }
}