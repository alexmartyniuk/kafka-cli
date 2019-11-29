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
        public string GroupId { get; } = "kafka-cli";

        [Option("-fm|--forgetme", Description = "Group ID")]
        public bool ForgetMe { get; } = false;

        [Option("-c|--commit", Description = "Commit the message. This prevents from receiving the same message twice for the one Group ID.")]
        public bool Commit { get; } = false;

        private async Task<int> OnExecute(IConsole console)
        {
            var config = ConfigService.Get();

            var consumerGroupId = ForgetMe ?
                GroupId + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"):
                GroupId;

            console.WriteLine($"Waiting for messages in {TopicName}:");
            KafkaClient.ReceivedMessage(TopicName, consumerGroupId, Commit, (string topicInfo, string message) =>
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