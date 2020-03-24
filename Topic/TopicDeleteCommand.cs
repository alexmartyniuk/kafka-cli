using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Kafka.Tool.Cli.Kafka;

namespace Kafka.Tool.Cli.Topic
{

    [Command("delete", Description = "Delete a topic",
            AllowArgumentSeparator = true)]
    public class TopicDeleteCommand
    {
        [Required(ErrorMessage = "You must specify a topic name")]
        [Option("-t|--topic", Description = "Topic name")]
        public string TopicName { get; }

        private async Task<int> OnExecute(IConsole console)
        {
            try
            {                
                await KafkaClient.DeleteTopicAsync(TopicName);
                console.WriteLine($"Topic '{TopicName}' was deleted.");
                return await Task.FromResult(0);
            }
            catch (Exception e)
            {
                console.WriteLine($"An error occured deleting topic '{TopicName}': {e.Message}");
                return await Task.FromResult(1);
            }
        }     
    }
}