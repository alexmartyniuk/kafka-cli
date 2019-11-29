using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Config;
using Scania.Kafka.Tool.Cli.Kafka;

namespace Scania.Kafka.Tool.Cli.Message
{

    [Command("receive", Description = "Receive a message",
            AllowArgumentSeparator = true,
            ThrowOnUnexpectedArgument = false)]
    public class MessageReceiveCommand
    {
        [Required(ErrorMessage = "You must specify the topic")]
        [Option("-t|--topic", Description = "Topic name")]
        public string TopicName { get; }

        [Option("-g|--group", Description = "Group ID")]
        public string GroupId { get; }

        private async Task<int> OnExecute(IConsole console)
        {
            var config = ConfigService.Get();
            var consumerGroupId = !string.IsNullOrWhiteSpace(GroupId) ?
                 GroupId :
                 Guid.NewGuid().ToString();

            console.WriteLine($"Waiting for messages in {TopicName}:");
            KafkaClient.ReceivedMessage(TopicName, consumerGroupId, (string topicInfo, string message) =>
            {
                console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.ff")}] at {topicInfo}: {message}");
            },
            (string error) => 
            {
                console.WriteLine($"Error occured: {error}");
            });

            return await Task.FromResult(0);
        }        
    }
}